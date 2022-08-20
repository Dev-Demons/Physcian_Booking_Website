using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Binke.Models;
using Binke.Utility;
using Binke.ViewModels;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using Microsoft.AspNet.Identity;

namespace Binke.Controllers
{
    [Authorize]
    public class AdvertisementController : Controller
    {
        private readonly IAddressService _address;
        private readonly IDoctorService _doctor;
        private readonly IPharmacyService _pharmacy;
        private readonly IFacilityService _facility;
        private readonly ISeniorCareService _seniorCare;
        private readonly IAdvertisementService _advertisement;
        private readonly IAdvertisementLocationService _location;
        private ApplicationUserManager _userManager;
        private readonly IStateService _state;
        private readonly IUserService _appUser;

        // , IAdvertisementService advertisement, IAdvertisementLocationService location
        private Dictionary<int, int> UserTypeDict = new Dictionary<int, int> { {1005,3 }, { 1006, 4 }, { 1007, 6 } };

        public AdvertisementController(IAddressService address, IDoctorService doctor, IPharmacyService pharmacy, IFacilityService facility, ISeniorCareService seniorCare, IAdvertisementService advertisement, IAdvertisementLocationService location, IStateService state, IUserService appUser, ApplicationUserManager applicationUserManager)
        {
            _address = address;
            _doctor = doctor;
            _pharmacy = pharmacy;
            _facility = facility;
            _seniorCare = seniorCare;
            _advertisement = advertisement;
            _location = location;
            _state = state;
            _appUser = appUser;
            _userManager = applicationUserManager;
        }

        [HttpPost]
        [Route("GetReference")]
        public JsonResult GetReference(string Prefix, int Rtype)
        {
            var infoList = new List<AdvertisementReferenceViewModel>();
            try
            {
                if (Rtype == 2)
                {

                    var InfoDoc = _doctor.GetAll(x => (x.LastName.ToLower().StartsWith(Prefix.ToLower()) || x.FirstName.ToLower().StartsWith(Prefix.ToLower()) || x.MiddleName.ToLower().StartsWith(Prefix.ToLower())) && x.IsActive == true && x.IsDeleted == false).ToList();
                    foreach (var item in InfoDoc)
                    {
                        infoList.Add(new AdvertisementReferenceViewModel() { ReferenceName = item.LastName + " " + item.MiddleName + " " + item.FirstName + " " + item.Credential, ID = item.DoctorId });
                    }
                    return Json(infoList, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (Rtype == 1005)
                    {
                        var Info = _pharmacy.GetAll(x => x.OrganizationTypeID == 1005 && x.IsActive == true && x.IsDeleted == false && x.OrganisationName.ToLower().StartsWith(Prefix.ToLower())).ToList();
                        foreach (var item in Info)
                        {
                            infoList.Add(new AdvertisementReferenceViewModel() { ReferenceName = item.OrganisationName, ID = item.OrganisationId });
                        }
                        return Json(infoList, JsonRequestBehavior.AllowGet);
                    }
                    else if (Rtype == 1006)
                    {
                        var Info = _facility.GetAll(x => x.OrganizationTypeID == 1006 && x.IsActive == true && x.IsDeleted == false && x.OrganisationName.ToLower().StartsWith(Prefix.ToLower())).ToList();
                        foreach (var item in Info)
                        {
                            infoList.Add(new AdvertisementReferenceViewModel() { ReferenceName = item.OrganisationName, ID = item.OrganisationId });
                        }
                        return Json(infoList, JsonRequestBehavior.AllowGet);
                    }
                    else if (Rtype == 1007)
                    {

                        var Info = _seniorCare.GetAll(x => x.OrganizationTypeID == 1007 && x.IsActive == true && x.IsDeleted == false && x.OrganisationName.ToLower().StartsWith(Prefix.ToLower())).ToList();
                        foreach (var item in Info)
                        {
                            infoList.Add(new AdvertisementReferenceViewModel() { ReferenceName = item.OrganisationName, ID = item.OrganisationId });
                        }
                        return Json(infoList, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetReference/Advertisement-POST");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Advertisement
        public ActionResult Index()
        {
            //var locations = _location.GetAll();
            //ViewBag.LocationList = new SelectList(locations.OrderBy(o => o.AdvertisementLocationId), "AdvertisementLocationId", "Name");

            return View();
        }


        // POST: Advertisement/CreateAdvertisement
        [HttpPost]
        [Route("AddEditAdvertisement")]
        public ActionResult AddEditAdvertisement(AdvertisementViewModel model, HttpPostedFileBase Image1)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    string imagePath = "";

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/Advertisement/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/Advertisement"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "Advertisement-" + DateTime.Now.Ticks.ToString() + extension;
                        if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png" || extension.ToLower() == ".bmp")
                        {
                            if (Image1.ContentLength <= 5242880)
                            {
                                var path = Path.Combine(Server.MapPath("~/Uploads/Advertisement"), newImageName);
                                Image1.SaveAs(path);
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, "Invalid size cannot be more than 5 MB !");
                                return Json(new JsonResponse() { Status = -1, Message = "Invalid size cannot be more than 5 MB !" });
                                
                            }
                            imagePath = newImageName;
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Invalid Image !");
                            return Json(new JsonResponse() { Status = -1, Message = "Invalid Image !" });
                        }
                    }

                    if (model.AdvertisementId > 0 && imagePath == "")
                    {
                        imagePath = model.ImagePath;
                    }

                    if (model.AdvertisementId > 0)
                    {
                        var adv = _advertisement.GetById(model.AdvertisementId);
                        adv.StartDate = model.StartDate;
                        adv.EndDate = model.EndDate;
                        adv.UpdatedDate = DateTime.UtcNow;
                        adv.UpdatedBy = User.Identity.GetUserId<int>();
                        adv.ReferenceId = model.ReferenceId;
                        adv.Title = model.Title;
                        adv.ImagePath = imagePath;
                        adv.UserTypeId = UserTypeDict.ContainsKey(model.UserTypeId.GetValueOrDefault()) ? UserTypeDict[model.UserTypeId.GetValueOrDefault()] : model.UserTypeId;
                        adv.AdvertisementTypeId = model.AdvertisementTypeId;
                        adv.CityStateZipCodeId = model.CityStateZipCodeId;
                        adv.PaymentTypeId = model.PaymentTypeId;
                        adv.IsActive = model.IsActiveString == "on" ? true : false;
                        _advertisement.UpdateData(adv);
                    }
                    else
                    {

                        int currentAdvertisementId = 0;

                        int adCount = _advertisement.GetAll().Count();
                        if (adCount == 0)
                        {
                            currentAdvertisementId = 1;
                        }
                        else
                        {
                            currentAdvertisementId = _advertisement.GetAll().Select(x => x.AdvertisementId).Max() + 1;
                        }


                        var add = new Advertisement();
                        add.AdvertisementId = currentAdvertisementId;
                        add.StartDate = model.StartDate;
                        add.EndDate = model.EndDate;
                        add.CreatedDate = DateTime.UtcNow;
                        add.CreatedBy = User.Identity.GetUserId<int>();
                        add.IsActive = model.IsActiveString == "on" ? true : false; ;
                        add.IsDeleted = false;
                        add.ReferenceId = model.ReferenceId;
                        add.Title = model.Title;
                        add.ImagePath = imagePath;
                        add.UserTypeId = UserTypeDict.ContainsKey(model.UserTypeId.GetValueOrDefault()) ? UserTypeDict[model.UserTypeId.GetValueOrDefault()] : model.UserTypeId;
                        add.AdvertisementTypeId = model.AdvertisementTypeId;
                        add.CityStateZipCodeId = model.CityStateZipCodeId;
                        add.PaymentTypeId = model.PaymentTypeId;
                        _advertisement.InsertData(add);
                    }
                    _advertisement.SaveData();
                    scope.Complete();
                    if (model.AdvertisementId > 0)
                        return Json(new JsonResponse() { Status = 1, Message = "Advertisement updated successfully" });
                    else
                        return Json(new JsonResponse() { Status = 1, Message = "Advertisement created successfully" });
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    Common.LogError(ex, "AddEditAdvertisement/Advertisement-POST");
                    return Json(new JsonResponse() { Status = 0, Message = "Error in saving advertisement info" });
                }
            }
        }

        public string GetCityStateInfoById(int id, string type)
        {
            string result = "";

            var info = _facility.SQLQuery<CityStateInfoByZipCodeViewModel>("spGetCityStateZipInfoByID @ID", new SqlParameter("ID", System.Data.SqlDbType.Int) { Value = id }).ToList();

            if (type == "zip")
                result = info[0].ZipCode;
            if (type == "city")
                result = info[0].City;
            if (type == "state")
                result = info[0].State;
            if (type == "country")
                result = info[0].Country;

            return result;
        }

        // GET: GetAdvertisementList
        public ActionResult GetAdvertisementList(bool flag, JQueryDataTableParamModel param)
        {
            try
            {
                if (param == null)
                {
                    param = new JQueryDataTableParamModel();
                }
                DateTime dt = DateTime.Now;
                int IsDateFilter = 0;
                if (param.sSearch != null && DateTime.TryParseExact(param.sSearch, "dd MMM yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                {
                    param.sSearch=" ";
                    IsDateFilter = 1;
                }
                var adList = _advertisement.ExecWithStoreProcedure<AdvertisementListViewModel>("spAdvertisementList_Search @Search, @PageIndex, @PageSize, @Sort,@SortBy", //Added by Reena
                            new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                            new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                            new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                            new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" },
                            new SqlParameter("@SortBy", System.Data.SqlDbType.Int) { Value = param.iSortCol_0}
                            ).ToList();
                int TotalRecordCount = 0;

                if (adList != null && adList.Count > 0)
                {
                    if(IsDateFilter ==1)
                    {
                        adList = adList.Where(x =>(x.StartDate!=null && x.StartDate.Trim() != "" && DateTime.Parse(x.StartDate) == dt) || (x.EndDate!=null && x.EndDate.Trim()!="" && DateTime.Parse(x.EndDate) == dt)).ToList();
                        if (adList != null && adList.Count > 0)
                        {
                            TotalRecordCount = adList.Count();
                        }
                    }
                    else
                    { 
                        TotalRecordCount = adList.ElementAt(0).TotalRecordCount ?? 0;
                    }
                }
                //if (param.sSortDir_0 != null)
                //{
                //    if (param.sSortDir_0.ToLower() == "asc" && param.iSortCol_0 == 0)
                //    {
                //        adList = adList.ToList().OrderBy(x => x.AdvertisementTypeName).ToList();
                //    }
                //    else if (param.sSortDir_0.ToLower() == "desc" && param.iSortCol_0 == 0)
                //    {

                //        adList = adList.ToList().OrderByDescending(x => x.AdvertisementTypeName).ToList();

                //    }
                //    else if (param.sSortDir_0.ToLower() == "asc" && param.iSortCol_0 == 1)
                //    {
                //        adList = adList.ToList().OrderBy(x => x.AdvertiserName).ToList();
                //    }
                //    else if (param.sSortDir_0.ToLower() == "desc" && param.iSortCol_0 == 1)
                //    {

                //        adList = adList.ToList().OrderByDescending(x => x.AdvertiserName).ToList();

                //    }
                //    else if (param.sSortDir_0.ToLower() == "asc" && param.iSortCol_0 == 2)
                //    {
                //        adList = adList.ToList().OrderBy(x => x.AdvertiserType).ToList();
                //    }
                //    else if (param.sSortDir_0.ToLower() == "desc" && param.iSortCol_0 == 2)
                //    {

                //        adList = adList.ToList().OrderByDescending(x => x.AdvertiserType).ToList();

                //    }
                //    else if (param.sSortDir_0.ToLower() == "asc" && param.iSortCol_0 == 3)
                //    {
                //        adList = adList.ToList().OrderBy(x => x.Title).ToList();
                //    }
                //    else if (param.sSortDir_0.ToLower() == "desc" && param.iSortCol_0 == 3)
                //    {

                //        adList = adList.ToList().OrderByDescending(x => x.Title).ToList();

                //    }
                //    else if (param.sSortDir_0.ToLower() == "asc" && param.iSortCol_0 == 5)
                //    {
                //        adList = adList.ToList().OrderBy(x => x.StartDate).ToList();
                //    }
                //    else if (param.sSortDir_0.ToLower() == "desc" && param.iSortCol_0 == 5)
                //    {

                //        adList = adList.ToList().OrderByDescending(x => x.StartDate).ToList();

                //    }
                //    else if (param.sSortDir_0.ToLower() == "asc" && param.iSortCol_0 == 6)
                //    {
                //        adList = adList.ToList().OrderBy(x => x.EndDate).ToList();
                //    }
                //    else if (param.sSortDir_0.ToLower() == "desc" && param.iSortCol_0 == 6)
                //    {

                //        adList = adList.ToList().OrderByDescending(x => x.EndDate).ToList();

                //    }
                //}

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = adList
                }, JsonRequestBehavior.AllowGet);

                //var data = _advertisement.GetAll().Where(x=>x.IsDeleted != true).ToList();
                //var allAds = data.Select(x => new AdvertisementListViewModel()
                //{
                //    AdvertisementId = x.AdvertisementId,
                //    UserTypeId = x.UserTypeId,
                //    PaymentTypeId = x.PaymentTypeId,
                //    PaymentTypeName = x.PaymentType?.Name,
                //    AdvertiserType = x.UserTypeId == 2 ? "Doctor" : (x.UserTypeId == 1005 ? "Pharmacy" : (x.UserTypeId == 1006 ? "Facility" : (x.UserTypeId == 1007 ? "Senior Care" : ""))),
                //    ReferenceId = x.ReferenceId,
                //    CityStateZipCodeId = x.CityStateZipCodeId,
                //    State = x.CityStateZipCodeId.HasValue ? GetCityStateInfoById(x.CityStateZipCodeId.Value,"state") : "",
                //    City = x.CityStateZipCodeId.HasValue ? GetCityStateInfoById(x.CityStateZipCodeId.Value, "city") : "",
                //    ZipCode = x.CityStateZipCodeId.HasValue ? GetCityStateInfoById(x.CityStateZipCodeId.Value, "zip") : "",
                //    AdvertiserName = x.UserTypeId == 2 && (_doctor.GetAll().Where(t => t.DoctorId == x.ReferenceId).Count() > 0) ? (_doctor.GetAll().FirstOrDefault(t => t.DoctorId == x.ReferenceId)?.FirstName + " "
                //    + _doctor.GetAll().FirstOrDefault(t => t.DoctorId == x.ReferenceId)?.MiddleName + " "
                //    + _doctor.GetAll().FirstOrDefault(t => t.DoctorId == x.ReferenceId)?.LastName + " "
                //    + _doctor.GetAll().FirstOrDefault(t => t.DoctorId == x.ReferenceId)?.Credential) :
                //    x.UserTypeId == 1005 && (_pharmacy.GetAll().Where(t => t.OrganizationTypeID == 1005 && t.OrganisationId == x.ReferenceId)?.Count() > 0) ? (_pharmacy.GetAll().FirstOrDefault(t => t.OrganizationTypeID == 1005 && t.OrganisationId == x.ReferenceId)?.OrganisationName) :
                //    x.UserTypeId == 1006 && (_facility.GetAll().Where(t => t.OrganizationTypeID == 1006 && t.OrganisationId == x.ReferenceId)?.Count() > 0) ? (_facility.GetAll().FirstOrDefault(t => t.OrganizationTypeID == 1006 && t.OrganisationId == x.ReferenceId)?.OrganisationName) :
                //    x.UserTypeId == 1007 && (_seniorCare.GetAll().Where(t => t.OrganizationTypeID == 1007 && t.OrganisationId == x.ReferenceId)?.Count() > 0) ? (_seniorCare.GetAll().FirstOrDefault(t => t.OrganizationTypeID == 1007 && t.OrganisationId == x.ReferenceId)?.OrganisationName) : "",
                //    Title = x.Title,
                //    AdvertisementTypeId = x.AdvertisementTypeId,
                //    AdvertisementTypeName = x.AdvertisementType?.AdvertisementTypeName,
                //    ImagePath = x.ImagePath,
                //    IsActive = x.IsActive != null ? x.IsActive.Value : false,
                //    IsDeleted = x.IsDeleted != null ? x.IsDeleted.Value : false,
                //    StartDate = x.StartDate != null ? x.StartDate.ToDefaultFormate() : "",
                //    EndDate = x.EndDate != null ? x.EndDate.ToDefaultFormate() : "",
                //    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                //    CreatedBy = x.CreatedBy != null ? x.CreatedBy.Value : 0,
                //    UpdatedDate = x.UpdatedDate != null ? x.UpdatedDate.ToDefaultFormate() : "",
                //    UpdatedBy = x.UpdatedBy
                //}).ToList();



                //if (!string.IsNullOrEmpty(param.sSearch))
                //{
                //    allAds = allAds.Where(x => x.AdvertiserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                //                                    || x.Title.ToString().ToLower().Contains(param.sSearch.ToLower())
                //                                    || x.StartDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                //                                    || x.EndDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                //                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                //                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
                //}

                //var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                //var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                //Func<AdvertisementListViewModel, string> orderingFunction =
                //    c => sortColumnIndex == 1 ? c.Title
                //        : sortColumnIndex == 3 ? c.AdvertiserName
                //        : c.Title;
                //allAds = sortDirection == "asc" ? allAds.OrderBy(orderingFunction).ToList() : allAds.OrderByDescending(orderingFunction).ToList();

                //var display = allAds.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                //var total = allAds.Count();



                //return Json(new
                //{
                //    param.sEcho,
                //    iTotalRecords = total,
                //    iTotalDisplayRecords = total,
                //    aaData = display
                //}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetAdvertisementList/Advertisement");
                return Json(new JsonResponse() { Status = 0, Message = "Exception" });
                throw ex;
            }
        }

        [Route("GetDrpAdvertisementType")]
        public ActionResult GetDrpAdvertisementType()
        {
            try
            {
                var advertisementTypeList = _facility.SQLQuery<DrpAdvertisementType>("select * from AdvertisementType").ToList();
                return Json(advertisementTypeList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetDrpAdvertisementType/Advertisement");
                return Json(new JsonResponse() { Status = 0, Message = "Exception" });
            }
        }

        [Route("GetDrpPaymentType")]
        public ActionResult GetDrpPaymentType()
        {
            try
            {
                var paymentTypeList = _facility.SQLQuery<DrpPaymentType>("select * from PaymentType").ToList();
                return Json(paymentTypeList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetDrpPaymentType/Advertisement");
                return Json(new JsonResponse() { Status = 0, Message = "Exception" });
            }
        }

        // POST: Advertisement/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Edit/Advertisement");
                return View();
            }
        }

        // GET: Advertisement/DeleteAdvertisement/5
        [HttpPost]
        public JsonResult DeleteAdvertisement(int id)
        {
            try
            {
                var ad = _advertisement.GetById(id);
                ad.IsActive = false;
                ad.IsDeleted = true;
                _advertisement.UpdateData(ad);
                _advertisement.SaveData();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "DeleteAdvertisement/Advertisement");
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Advertisement/DeleteAdvertisement/5
        [HttpPost]
        public JsonResult ActivateDeactivateAdvertisement(int id, bool flag)
        {
            try
            {
                var ad = _advertisement.GetById(id);
                if (flag == true)
                {
                    ad.IsActive = true;
                    ad.IsDeleted = true;
                }
                else
                {
                    ad.IsActive = false;
                }

                _advertisement.UpdateData(ad);
                _advertisement.SaveData();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ActivateDeactivateAdvertisement/Advertisement");
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Advertisement/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Delete/Advertisement");
                return View();
            }
        }
    }
}
