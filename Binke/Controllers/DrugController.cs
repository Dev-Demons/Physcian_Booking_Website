using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Binke.Model;
using Binke.Model.ViewModels;
using Binke.Models;
using Binke.Repository.Enumerable;
using Binke.Repository.Interface;
using Binke.Utility;
using Binke.ViewModels;

namespace Binke.Controllers
{
    public class DrugController : Controller
    {
        private readonly IDrugDetailService _drugDetailService;
        private readonly IDrugTypeService _drugTypeService;
        private readonly ITabletService _tabletService;
        private readonly IDrugQuantityService _drugQuantityService;
        private readonly IDrugManufacturerService _drugManufacturerService;
        private readonly IDrugPharmacyDetailService _drugPharmacyDetailService;
        private readonly IPharmacyService _pharmacy;

        public DrugController(IDrugDetailService drugDetailService, IDrugTypeService drugTypeService, ITabletService tabletService, IDrugQuantityService drugQuantityService, IDrugManufacturerService drugManufacturerService, IDrugPharmacyDetailService drugPharmacyDetailService, IPharmacyService pharmacy)
        {
            _drugDetailService = drugDetailService;
            _drugTypeService = drugTypeService;
            _tabletService = tabletService;
            _drugQuantityService = drugQuantityService;
            _drugManufacturerService = drugManufacturerService;
            _drugPharmacyDetailService = drugPharmacyDetailService;
            _pharmacy = pharmacy;
        }

        // GET: Drug

        [Route("DrugQuantity")]
        public ActionResult DrugQuantity()
        {
            return View();
        }

        [Route("DrugType")]
        public ActionResult DrugType()
        {
            return View();
        }

        [Route("Tablet")]
        public ActionResult Tablet()
        {
            return View();
        }

        [Route("DrugDetails")]
        public ActionResult DrugDetails()
        {
            return View();
        }

        [Route("Manufacturer")]
        public ActionResult Manufacturer()
        {
            return View();
        }

        [HttpGet, Route("DrugInformation/{id?}")]
        public ActionResult DrugInformation(int id = 0)
        {
            var drugDetail = _drugDetailService.GetById(id);
            return View(drugDetail);
        }


        //Get Methods
        [HttpGet, Route("Drug/GetDrugType")]
        public JsonResult GetDrugType(bool isActive, JQueryDataTableParamModel param)
        {
            try
            {
                var drugType = _drugTypeService.GetAll(x => x.IsActive == isActive && !x.IsDeleted);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    drugType = drugType.Where(x => x.Type.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower()));
                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                Func<DrugType, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.Type : sortColumnIndex == 2 ? c.CreatedDate.ToString().ToLower() : sortColumnIndex == 3 ? c.UpdatedDate.ToString().ToLower() : c.Type;

                drugType = sortDirection == "asc" ? drugType.OrderBy(orderingFunction).ToList() : drugType.OrderByDescending(orderingFunction).ToList();

                var display = drugType.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

                var response = display.Select(x => new
                {
                    x.DrugTypeId,
                    x.Type,
                    x.IsActive,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate()
                }).ToList();

                var total = response?.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = response
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/GetTabletPower")]
        public JsonResult GetTablet(bool isActive, JQueryDataTableParamModel param)
        {
            try
            {
                var tablet = _tabletService.GetAll(x => x.IsActive == isActive && !x.IsDeleted);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    tablet = tablet.Where(x => x.DrugPower.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower()));
                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                Func<Tablet, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.DrugPower : sortColumnIndex == 2 ? c.CreatedDate.ToString().ToLower() : sortColumnIndex == 3 ? c.UpdatedDate.ToString().ToLower() : c.DrugPower;

                tablet = sortDirection == "asc" ? tablet.OrderBy(orderingFunction) : tablet.OrderByDescending(orderingFunction);

                var display = tablet.Skip(param.iDisplayStart).Take(param.iDisplayLength);

                var response = display.Select(x => new
                {
                    x.TabletId,
                    x.DrugPower,
                    x.IsActive,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate()
                }).ToList();

                var total = response?.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = response
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/GetDrugQuantity")]
        public JsonResult GetDrugQuantity(bool isActive, JQueryDataTableParamModel param)
        {
            try
            {
                var drugQuantity = _drugQuantityService.GetAll(x => x.IsActive == isActive && !x.IsDeleted);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    drugQuantity = drugQuantity.Where(x => x.Quantity.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower()));
                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                Func<DrugQuantity, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.Quantity : sortColumnIndex == 2 ? c.CreatedDate.ToString().ToLower() : sortColumnIndex == 3 ? c.UpdatedDate.ToString().ToLower() : c.Quantity;

                drugQuantity = sortDirection == "asc" ? drugQuantity.OrderBy(orderingFunction) : drugQuantity.OrderByDescending(orderingFunction);

                var display = drugQuantity.Skip(param.iDisplayStart).Take(param.iDisplayLength);

                var response = display.Select(x => new
                {
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    x.IsActive,
                    x.DrugQuantityId,
                    x.Quantity
                });

                var total = response?.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = response
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/GetDrugManufacturer")]
        public JsonResult GetDrugManufacturer(bool isActive, JQueryDataTableParamModel param)
        {
            try
            {
                var drugManufacturer = _drugManufacturerService.GetAll(x => x.IsActive == isActive && !x.IsDeleted);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    drugManufacturer = drugManufacturer.Where(x => x.Manufacturer.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower()));
                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                Func<DrugManufacturer, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.Manufacturer : sortColumnIndex == 2 ? c.CreatedDate.ToString().ToLower() : sortColumnIndex == 3 ? c.UpdatedDate.ToString().ToLower() : c.Manufacturer;

                drugManufacturer = sortDirection == "asc" ? drugManufacturer.OrderBy(orderingFunction) : drugManufacturer.OrderByDescending(orderingFunction);

                var display = drugManufacturer.Skip(param.iDisplayStart).Take(param.iDisplayLength);

                var response = display.Select(x => new
                {
                    x.DrugManufacturerId,
                    x.Manufacturer,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    x.IsActive
                }).ToList();

                var total = response?.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = response
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/GetDrugDetails")]
        public JsonResult GetDrugDetails(bool isActive, JQueryDataTableParamModel param)
        {
            try
            {
                var drugDetails = _drugDetailService.GetAll(x => x.IsActive == isActive && !x.IsDeleted);
                //var drugDetails1 = _drugDetailService.GetAll(x => x.IsActive == isActive && !x.IsDeleted).ToList();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    drugDetails = drugDetails.Where(x => x.MedicineName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower()));
                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                Func<DrugDetail, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.MedicineName : sortColumnIndex == 2 ? c.CreatedDate.ToString().ToLower() : sortColumnIndex == 3 ? c.UpdatedDate.ToString().ToLower() : c.MedicineName;

                drugDetails = sortDirection == "asc" ? drugDetails.OrderBy(orderingFunction).ToList() : drugDetails.OrderByDescending(orderingFunction).ToList();

                var display = drugDetails.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

                var response = display.Select(x => new
                {
                    x.DrugDetailId,
                    x.MedicineName,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    x.IsActive
                }).ToList();

                var total = response?.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = response
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Add Update Methods
        [HttpPost, Route("Drug/AddOrUpdateDrugType")]
        public JsonResult AddOrUpdateDrugType(DrugType drugType)
        {
            try
            {
                if (drugType.DrugTypeId != 0)
                {
                    var data = _drugTypeService.GetById(drugType.DrugTypeId);
                    data.Type = drugType.Type != null ? drugType.Type : data.Type;
                    data.CreatedDate = data.CreatedDate;
                    data.UpdatedDate = DateTime.Now;
                    data.IsActive = drugType.IsActive;

                    _drugTypeService.UpdateData(data);
                }
                else
                {
                    drugType.CreatedDate = DateTime.Now;
                    _drugTypeService.InsertData(drugType);
                }

                _drugTypeService.SaveData();
                return Json(new JsonResponse() { Status = 200, Message = "Records added successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost, Route("Drug/AddOrUpdateTabletPower")]
        public JsonResult AddOrUpdateTabletPower(Tablet tablet)
        {
            try
            {
                if (tablet.TabletId != 0)
                {
                    var data = _tabletService.GetById(tablet.TabletId);
                    data.IsActive = tablet.IsActive;
                    data.DrugPower = tablet.DrugPower != null ? tablet.DrugPower : data.DrugPower;
                    data.UpdatedDate = DateTime.Now;

                    _tabletService.UpdateData(data);
                }
                else
                {
                    tablet.CreatedDate = DateTime.Now;
                    _tabletService.InsertData(tablet);
                }

                _tabletService.SaveData();
                return Json(new JsonResponse() { Status = 200, Message = "Records added successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost, Route("Drug/AddOrUpdateDrugQuantity")]
        public JsonResult AddOrUpdateDrugQuantity(DrugQuantity drugQuantity)
        {
            try
            {
                if (drugQuantity.DrugQuantityId != 0)
                {
                    var data = _drugQuantityService.GetById(drugQuantity.DrugQuantityId);
                    data.IsActive = drugQuantity.IsActive;
                    data.Quantity = drugQuantity.Quantity != null ? drugQuantity.Quantity : data.Quantity;
                    data.UpdatedDate = DateTime.Now;

                    _drugQuantityService.UpdateData(data);
                }
                else
                {
                    drugQuantity.CreatedDate = DateTime.Now;
                    _drugQuantityService.InsertData(drugQuantity);
                }

                _drugQuantityService.SaveData();
                return Json(new JsonResponse() { Status = 200, Message = "Records added successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost, Route("Drug/AddOrUpdateDrugManufacturer")]
        public JsonResult AddOrUpdateDrugManufacturer(DrugManufacturer drugManufacturer)
        {
            try
            {
                if (drugManufacturer.DrugManufacturerId != 0)
                {
                    var data = _drugManufacturerService.GetById(drugManufacturer.DrugManufacturerId);
                    data.IsActive = drugManufacturer.IsActive;
                    data.Manufacturer = drugManufacturer.Manufacturer != null ? drugManufacturer.Manufacturer : data.Manufacturer;
                    data.UpdatedDate = DateTime.Now;

                    _drugManufacturerService.UpdateData(data);
                }
                else
                {
                    drugManufacturer.CreatedDate = DateTime.Now;
                    _drugManufacturerService.InsertData(drugManufacturer);
                }
                _drugManufacturerService.SaveData();
                return Json(new JsonResponse() { Status = 200, Message = "Records added successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost, Route("Drug/AddDrugDetail")]
        public JsonResult AddDrugDetil(AddDrugDetailParams parameters)
        {
            try
            {
                var drugDetail = new DrugDetail
                {
                    CreatedBy = parameters.CreatedBy,
                    CreatedDate = parameters.CreatedDate,
                    Dosage = parameters.Dosage,
                    Interaction = parameters.Interaction,
                    ModifiedBy = parameters.ModifiedBy,
                    IsDeleted = parameters.IsDeleted,
                    IsActive = parameters.IsActive,
                    LongDescription = parameters.LongDescription,
                    MedicineName = parameters.MedicineName,
                    Professional = parameters.Professional,
                    ShortDescription = parameters.ShortDescription,
                    SideEffects = parameters.SideEffects,
                    Tips = parameters.Tips,
                    UpdatedDate = parameters.UpdatedDate
                };

                _drugDetailService.InsertData(drugDetail);

                _drugDetailService.SaveData();

                var drugDetailId = _drugDetailService.GetAll().Where(x => x.MedicineName == parameters.MedicineName).FirstOrDefault()?.DrugDetailId;

                parameters.DrugDetailId = Convert.ToInt32(drugDetailId);

                SaveDrugPharmacyDetail(parameters);
                //foreach (var pharmacy in parameters.PharmacyList)
                //{
                //    foreach (var drugtype in parameters.DrugTypeList)
                //    {
                //        foreach (var tabletPower in parameters.TabletPowerList)
                //        {
                //            foreach (var drugQuantity in parameters.DrugQuantityList)
                //            {
                //                foreach (var drugManufacturer in parameters.DrugManufacturerList)
                //                {
                //                    var drugPharmacyDetail = new DrugPharmacyDetail
                //                    {
                //                        CreatedBy = parameters.CreatedBy,
                //                        CreatedDate = parameters.CreatedDate,
                //                        UpdatedDate = parameters.UpdatedDate,
                //                        ModifiedBy = parameters.ModifiedBy,
                //                        IsActive = true,
                //                        IsDeleted = parameters.IsDeleted,
                //                        DrugDetailId = Convert.ToInt32(drugDetailId),
                //                        DrugManufacturerId = drugManufacturer,
                //                        DrugQuantityId = drugQuantity,
                //                        DrugTypeId = drugtype,
                //                        TabletId = tabletPower,
                //                        PharmacyId = pharmacy
                //                    };
                //                    _drugPharmacyDetailService.InsertData(drugPharmacyDetail);
                //                }
                //            }
                //        }
                //    }
                //}

                return Json(new JsonResponse() { Status = 200, Message = "Records added successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet, Route("Drug/GetDrugDetailById/{drugDetailId?}")]
        public JsonResult GetDrugDetailById(int drugDetailId)
        {
            var drugDetails = new AddDrugDetailParams();
            try
            {
                var drugDetail = _drugDetailService.GetById(drugDetailId);
                if (drugDetail != null)
                {
                    drugDetails.CreatedBy = drugDetail.CreatedBy;
                    drugDetails.CreatedDate = drugDetail.CreatedDate;
                    drugDetails.Dosage = drugDetail.Dosage;
                    drugDetails.DrugDetailId = drugDetail.DrugDetailId;
                    drugDetails.Interaction = drugDetail.Interaction;
                    drugDetails.IsActive = drugDetail.IsActive;
                    drugDetails.IsDeleted = drugDetail.IsDeleted;
                    drugDetails.LongDescription = drugDetail.LongDescription;
                    drugDetails.MedicineName = drugDetail.MedicineName;
                    drugDetails.ModifiedBy = drugDetail.ModifiedBy;
                    drugDetails.Professional = drugDetail.Professional;
                    drugDetails.ShortDescription = drugDetail.ShortDescription;
                    drugDetails.SideEffects = drugDetail.SideEffects;
                    drugDetails.Tips = drugDetail.Tips;
                    drugDetails.UpdatedDate = drugDetail.UpdatedDate;

                    var drugPharmacyDetail = _drugPharmacyDetailService.GetAll().Where(x => x.DrugDetailId == drugDetailId).ToList();

                    drugDetails.TabletPowerList = drugPharmacyDetail.Select(item => item.TabletId).Distinct().ToList();
                    drugDetails.DrugManufacturerList = drugPharmacyDetail.Select(item => item.DrugManufacturerId).Distinct().ToList();
                    drugDetails.DrugQuantityList = drugPharmacyDetail.Select(item => item.DrugQuantityId).Distinct().ToList();
                    drugDetails.DrugTypeList = drugPharmacyDetail.Select(item => item.DrugTypeId).Distinct().ToList();
                    drugDetails.PharmacyList = drugPharmacyDetail.Select(item => item.PharmacyId).Distinct().ToList();

                }
                return Json(new JsonResponse() { Status = 200, Data = drugDetails }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/GetPharmacy")]
        public JsonResult GetPharmacy(bool isActive)
        {
            try
            {
                var pharmacy = _pharmacy.GetAll(x => x.IsActive == isActive && !x.IsDeleted).Select(x => new PharmacyBasicInformation()
                {
                    PharmacyId = x.PharmacyId,
                    PharmacyName = x.PharmacyName,
                    IsActive = x.IsActive
                }).ToList();

                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = pharmacy }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost, Route("Drug/UpdateDrugDetail")]
        public JsonResult UpdateDrugDetail(AddDrugDetailParams parameters)
        {
            try
            {
                // remove existing drugpharmacydetail records
                var existingDrugDetail = _drugDetailService.GetById(parameters.DrugDetailId);
                existingDrugDetail.Dosage = parameters.Dosage;
                existingDrugDetail.Interaction = parameters.Interaction;
                existingDrugDetail.IsActive = parameters.IsActive;
                existingDrugDetail.IsDeleted = parameters.IsDeleted;
                existingDrugDetail.LongDescription = parameters.LongDescription;
                existingDrugDetail.MedicineName = parameters.MedicineName;
                existingDrugDetail.ModifiedBy = parameters.ModifiedBy;
                existingDrugDetail.Professional = parameters.Professional;
                existingDrugDetail.ShortDescription = parameters.ShortDescription;
                existingDrugDetail.SideEffects = parameters.SideEffects;
                existingDrugDetail.Tips = parameters.Tips;
                existingDrugDetail.UpdatedDate = DateTime.Now;
                _drugDetailService.UpdateData(existingDrugDetail);
                _drugDetailService.SaveData();

                var existingDrugPharmacyDetail = _drugPharmacyDetailService.GetAll().Where(x => x.DrugDetailId == parameters.DrugDetailId);
                _drugPharmacyDetailService.DeleteData(existingDrugPharmacyDetail);
                _drugPharmacyDetailService.SaveData();

                SaveDrugPharmacyDetail(parameters);
                return Json(new JsonResponse { Status = 200, Message = "Drug Details are updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveDrugPharmacyDetail(AddDrugDetailParams parameters)
        {
            try
            {
                foreach (var pharmacy in parameters.PharmacyList)
                {
                    foreach (var drugtype in parameters.DrugTypeList)
                    {
                        foreach (var tabletPower in parameters.TabletPowerList)
                        {
                            foreach (var drugQuantity in parameters.DrugQuantityList)
                            {
                                foreach (var drugManufacturer in parameters.DrugManufacturerList)
                                {
                                    var drugPharmacyDetail = new DrugPharmacyDetail
                                    {
                                        CreatedBy = parameters.CreatedBy,
                                        CreatedDate = parameters.CreatedDate,
                                        UpdatedDate = parameters.UpdatedDate,
                                        ModifiedBy = parameters.ModifiedBy,
                                        IsActive = true,
                                        IsDeleted = parameters.IsDeleted,
                                        DrugDetailId = Convert.ToInt32(parameters.DrugDetailId),
                                        DrugManufacturerId = drugManufacturer,
                                        DrugQuantityId = drugQuantity,
                                        DrugTypeId = drugtype,
                                        TabletId = tabletPower,
                                        PharmacyId = pharmacy
                                    };
                                    _drugPharmacyDetailService.InsertData(drugPharmacyDetail);
                                }
                            }
                        }
                    }
                }

                _drugPharmacyDetailService.SaveData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/CheckDrug")]
        public JsonResult CheckDrug(string drugName)
        {
            try
            {
                var drugDetailId = _drugDetailService.GetAll().Where(x => x.MedicineName == drugName).FirstOrDefault()?.DrugDetailId;
                return Json(new JsonResponse { Data = drugDetailId, Message = "Drug already exist", Status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost, Route("Drug/SavePharmacy")]
        public JsonResult SavePharmacy(AddPharmacyDetail parameters)
        {
            string actionType = "Added";
            try
            {
                if (parameters.DrugPharmacyDetailId != 0)
                {
                    var drugPharmacyDetails = _drugPharmacyDetailService.GetById(parameters.DrugPharmacyDetailId);
                    drugPharmacyDetails.DrugManufacturerId = parameters.DrugManufacturerId;
                    drugPharmacyDetails.DrugQuantityId = parameters.DrugQuantityId;
                    drugPharmacyDetails.DrugTypeId = parameters.DrugTypeId;
                    drugPharmacyDetails.TabletId = parameters.TabletPowerId;
                    drugPharmacyDetails.PharmacyId = parameters.PharmacyId;
                    drugPharmacyDetails.Price = parameters.Price;
                    drugPharmacyDetails.UpdatedDate = DateTime.Now;
                    _drugPharmacyDetailService.UpdateData(drugPharmacyDetails);
                    actionType = "Updated";
                }
                else
                {
                    var drugPharmacyDetail = new DrugPharmacyDetail
                    {
                        CreatedBy = parameters.CreatedBy,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = parameters.UpdatedDate,
                        ModifiedBy = parameters.ModifiedBy,
                        IsActive = true,
                        IsDeleted = parameters.IsDeleted,
                        DrugDetailId = Convert.ToInt32(parameters.DrugDetailId),
                        DrugManufacturerId = parameters.DrugManufacturerId,
                        DrugQuantityId = parameters.DrugQuantityId,
                        DrugTypeId = parameters.DrugTypeId,
                        TabletId = parameters.TabletPowerId,
                        PharmacyId = parameters.PharmacyId,
                        Price = parameters.Price
                    };
                    _drugPharmacyDetailService.InsertData(drugPharmacyDetail);
                }

                _drugPharmacyDetailService.SaveData();
                return Json(new JsonResponse { Message = "Data " + actionType + " successfully", Status = 200 });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/GetDrugPharmacyDetailsById/{id?}")]
        public JsonResult GetDrugPharmacyDetailsById(int id)
        {
            try
            {
                var drugPharmacyDetails = _drugPharmacyDetailService.GetById(Convert.ToInt32(id));
                var drugPharmacyDetail = new
                {
                    drugPharmacyDetails.DrugDetailId,
                    drugPharmacyDetails.DrugManufacturer,
                    drugPharmacyDetails.DrugManufacturerId,
                    drugPharmacyDetails.DrugPharmacyDetailId,
                    drugPharmacyDetails.DrugQuantityId,
                    drugPharmacyDetails.DrugTypeId,
                    drugPharmacyDetails.PharmacyId,
                    drugPharmacyDetails.TabletId,
                    drugPharmacyDetails.Price
                };

                return Json(new JsonResponse() { Status = 200, Message = "", Data = drugPharmacyDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost, Route("Drug/SaveDrugDetail")]
        public JsonResult SaveDrugDetail(DrugDetail drugDetail)
        {
            try
            {
                _drugDetailService.InsertData(drugDetail);
                _drugDetailService.SaveData();
                var drugDetailId = _drugDetailService.GetAll().Where(x => x.MedicineName == drugDetail.MedicineName).FirstOrDefault()?.DrugDetailId;
                return Json(new JsonResponse { Data = drugDetailId, Message = "Drug Detail Saved successfully", Status = 200 });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet, Route("Drug/GetPharmacyByDrugDetailId/{drugDetailId?}")]
        public JsonResult GetPharmacyByDrugDetailId(int drugDetailId, JQueryDataTableParamModel param)
        {
            try
            {
                var pharmacy = _drugDetailService.SearchDrug(StoredProcedureList.SearchDrug, drugDetailId).AsEnumerable();
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    pharmacy = pharmacy.Where(x => x.DrugManufacturer.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || x.Tablet.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || x.DrugQuantity.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || x.DrugType.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || x.PharmacyName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || x.Price.ToString().ToLower().Contains(param.sSearch.ToLower()));
                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                Func<SpSearchDrugViewModel, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.PharmacyName.ToLower() : sortColumnIndex == 2 ? c.DrugType.ToLower() : sortColumnIndex == 3 ? c.Tablet.ToString().ToLower() : sortColumnIndex == 4 ? c.DrugQuantity.ToLower() : sortColumnIndex == 5 ? c.Price.ToString() : c.PharmacyName.ToLower();

                pharmacy = sortDirection == "asc" ? pharmacy.OrderBy(orderingFunction) : pharmacy.OrderByDescending(orderingFunction);

                var display = pharmacy.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

                var total = display?.Count();

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
                throw ex;
            }
        }

        [HttpGet, Route("Drug/DeActivateDrug/{drugDetailId?}")]
        public JsonResult DeActivateDrug(int drugDetailId)
        {
            try
            {
                var drugDetails = _drugDetailService.GetById(drugDetailId);
                drugDetails.IsActive = false;
                _drugDetailService.UpdateData(drugDetails);
                _drugDetailService.SaveData();

                return Json(new JsonResponse() { Data = drugDetails, Message = "Drug deactivated successfully", Status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
