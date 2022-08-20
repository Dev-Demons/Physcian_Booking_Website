using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Transactions;
using System.Web.Mvc;
using Doctyme.Model;
using Doctyme.Repository.Interface;
//using Binke.Utility;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Doctyme.Repository.Enumerable;
using Binke.Utility;
using Binke.Models;
using Binke.ViewModels;
using Doctyme.Model.ViewModels;
using System.Net;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Web;
using System.Configuration;
using Binke.Constant;
using Newtonsoft.Json.Linq;

namespace Binke.Controllers
{

    public class SearchController : Controller
    {
        private static List<String> listEntity = new List<String>() { "pharmacy", "seniorcare", "facility" };
        private readonly IAddressService _address;
        private readonly IDoctorInsuranceAcceptedService _doctorInsuranceAccepted;
        private readonly IDoctorInsuranceService _doctorInsuranceService;
        private readonly IInsuranceTypeRepository _insuranceTypeService;
        private readonly IAgeGroupService _agegroup;
        private readonly IGenderService _genderservice;
        private readonly IDoctorService _doctor;
        private readonly IFacilityService _facility;
        //private readonly IFacilityTypeService _facilitytype;
        private readonly ILanguageService _language;
        private readonly IPatientService _patient;
        private readonly IPharmacyService _pharmacy;
        private readonly ISeniorCareService _seniorCare;
        private readonly ISpecialityService _speciality;
        private readonly IUserService _appUser;
        private readonly ISlotService _slot;
        private readonly IReviewService _reviewService;

        // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
        //private readonly ITabletService _tabletService;
        private readonly IDrugService _drugService;

        private readonly IDrugTypeService _drugTypeService;
        private readonly IDrugQuantityService _drugQuantityService;
        private readonly IDrugManufacturerService _drugManufacturerService;
        private readonly IFeaturedService _featuredService;
        private readonly ITempSlotBookingService _tempSlotBookingService;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IOpeningHourService _openingHour;
        private static int _bookHour = 0;
        private readonly IpInfo _ipInfo;
        //private readonly IStateService _state;
        //private readonly ICityService _city;
        private readonly IRepository _repo;
        private readonly string _ipAddress;
        public SearchController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager,
            IAuthenticationManager authenticationManager, IDoctorInsuranceService doctorInsuranceService, IAddressService address, IAgeGroupService ageGroup, IDoctorService doctor, IFacilityService facility, /*IFacilityTypeService facilitytype,*/ ILanguageService language,
            IPatientService patient, IPharmacyService pharmacy, IGenderService genderService, IInsuranceTypeRepository insuranceTypeService, ISeniorCareService seniorCare, ISpecialityService speciality, IUserService appUser, ISlotService slot, IReviewService reviewService,
            ITempSlotBookingService tempSlotBookingService, IDrugService drugService, /*ITabletService tabletService,*/ IDrugTypeService drugTypeService,
            IDrugQuantityService drugQuantityService, IDrugManufacturerService drugManufacturerService,/*, IStateService state, ICityService city*/
            IFeaturedService featuredService, IOpeningHourService openingHour, IRepository repo)
        {
            _insuranceTypeService = insuranceTypeService;
            _address = address;
            _agegroup = ageGroup;
            _doctorInsuranceService = doctorInsuranceService;
            _genderservice = genderService;
            _doctor = doctor;
            _reviewService = reviewService;
            _facility = facility;
            //_facilitytype = facilitytype;
            _language = language;
            _patient = patient;
            _pharmacy = pharmacy;
            _seniorCare = seniorCare;
            _speciality = speciality;
            _appUser = appUser;
            _slot = slot;
            _tempSlotBookingService = tempSlotBookingService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _authenticationManager = authenticationManager;
            _drugService = drugService;
            //_tabletService = tabletService;
            _drugManufacturerService = drugManufacturerService;
            _drugQuantityService = drugQuantityService;
            _drugTypeService = drugTypeService;
            //_state = state;
            //_city = city;
            _featuredService = featuredService;
            _openingHour = openingHour;
            _repo = repo;

        }
        // GET: Search
        #region Doctor Search
        public ActionResult Index(string type, string search, string slocation, string skey)
        {
            try
            {

                if (!IsValidType(type))
                {
                    return RedirectToAction("Index", "Home");
                }
                //logs for IP Address 
                string strRemoteLocation = "";
                if (string.IsNullOrEmpty(slocation))
                {
                    strRemoteLocation = AssignLocationValues(slocation);
                    slocation = strRemoteLocation;
                }

                SearchParameterModel searchParameterModel = new SearchParameterModel()
                { locationSearch = slocation == null ? strRemoteLocation : slocation, Specialization = search };
                var model = BindFilter(searchParameterModel);
                model.Location = new Location();
                if (!string.IsNullOrEmpty(slocation))
                {
                    var lstcityZipCode = slocation.Split('|');
                    if (lstcityZipCode.Length > 0)
                    {
                        model.Location.City = lstcityZipCode[0];
                        model.Location.State = lstcityZipCode[1];
                        model.Location.ZipCode = lstcityZipCode[2];
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Doctor Search-Index");
                return View(new SearchViewModel());
            }
        }
        private List<string> GetZipCityState(string cityZipCode)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ZipcodeCity", SqlDbType.VarChar, 50) { Value = cityZipCode });

            var results = _doctor.GetDataList<CityStateZipDetail>(StoredProcedureList.spGetCityStateZipByZipcode, parameters);
            //var result = ds.Tables[0].Rows.Cast<DataRow>().ToArray();
            var list = results.Select(x => x.ZipCityState).ToList();
            return list;
        }

        [HttpPost, Route("_Search")]
        public PartialViewResult _Search(SearchParameterModel model)
        {
            try
            {
                //var latOfUser = string.Empty;
                //var longOfUser = string.Empty;

                //try
                //{
                //    if (!string.IsNullOrEmpty(_ipInfo.Loc))
                //    {
                //        latOfUser = _ipInfo.Loc.Split(',')[0];
                //        longOfUser = _ipInfo.Loc.Split(',')[1];
                //    }
                //}
                //catch
                //{
                //    latOfUser = null;
                //    longOfUser = null;
                //}

                //var doctorIds = new List<int>();
                //var paras = new List<SqlParameter>()
                //{
                //    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.Search??""},
                //    new SqlParameter("@SearchLocationKey", SqlDbType.NVarChar) { Value = model.locationSearch??""}
                //};
                //if (model.Distance == null) //removed Distance section
                //    model.Distance = new Distance();
                //decimal lat2 = 0;
                //decimal.TryParse(latOfUser, out lat2);
                //decimal long2 = 0;
                //decimal.TryParse(longOfUser, out long2);
                //var doctorIdsWithDistanceList = new List<DoctorWithDistance>();
                /*
                var featureDoctorIds = _doctor.GetFeaturedDoctorIdsBySearchText(StoredProcedureList.GetFeaturedDoctorIdS_v1, paras);
                if (model.AgeGroup != null && model.AgeGroup.Count > 0)
                {
                    var ageGrpDoctorIds = _doctor.GetAgeGroupReferenceId(model.AgeGroup);
                    doctorIds.AddRange(ageGrpDoctorIds);
                    if (doctorIds.Count > 0)
                        doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(ageGrpDoctorIds, lat2, long2));
                }
                if (model.Affiliations != null && model.Affiliations.Count > 0)
                {
                    var affDoctorIds = _doctor.GetAffiliationDoctorId(model.Affiliations);
                    doctorIds.AddRange(affDoctorIds);
                    if (doctorIds.Count > 0)
                        doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(affDoctorIds, lat2, long2));
                }
                if (model.Insurance != null && model.Insurance.Count > 0)
                {
                    var insDoctorIds = _doctor.GetInsuranceDoctorId(model.Insurance);
                    doctorIds.AddRange(insDoctorIds);
                    if (doctorIds.Count > 0)
                        doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(insDoctorIds, lat2, long2));
                }
                if (!string.IsNullOrEmpty(model.Search))
                {
                    var specDoctorIds = _doctor.GetDoctorIdBySpecialityName(model.Search);
                    doctorIds.AddRange(specDoctorIds);
                    if (doctorIds.Count > 0)
                        doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(specDoctorIds, lat2, long2));
                }

                doctorIdsWithDistanceList.AddRange(_doctor.GetDoctorIdByNameOrAddress(model.Search, model.NTPA, model.ANP, model.PrimaryCare, "", lat2, long2, model.locationSearch));

                doctorIdsWithDistanceList = doctorIdsWithDistanceList.DistinctBy(x => x.DoctorId).OrderBy(m => m.DistanceCount).ToList();

                var docIDsToFetch = new List<int>();
                var pageIndex = model.PageIndex == 0 ? 0 : model.PageIndex - 1;
                if (featureDoctorIds != null && featureDoctorIds.Count > 0)
                {
                    if (featureDoctorIds.Count > (pageIndex * model.PageSize))
                    {
                        docIDsToFetch.AddRange(featureDoctorIds.Skip(pageIndex * model.PageSize).Take(model.PageSize).Select(m => m.ReferenceId).ToList());
                    }
                }

                if (docIDsToFetch.Count < model.PageSize)
                    docIDsToFetch.AddRange(doctorIdsWithDistanceList.Skip(pageIndex * model.PageSize).Take(model.PageSize - docIDsToFetch.Count).Select(x => x.DoctorId).ToList());

               

                var listSearchparameters = GetDoctorSearchParameters(model, false, docIDsToFetch);
                var allslotList = _doctor.GetDoctorSearchListPagination(StoredProcedureList.GETDoctorListPagination_v1, listSearchparameters);
                */
                var listAllDoc = GetAllDoctorSearchParameters(model);
                int totalRecord = 0;

                //   var allslotList = _doctor.GetDoctorSearchList(StoredProcedureList.SearchDoctorList, parameters.ToArray<object>());
                var allslotList = _doctor.GetDoctorSearchListPagination(StoredProcedureList.GETDoctorListPagination_v3, listAllDoc);
                totalRecord = allslotList.Count;
                var docIdsForDetails = new List<SqlParameter>()
                {
                    new SqlParameter("@DoctorIds",SqlDbType.NVarChar){ Value = string.Join(",",(allslotList != null && allslotList.Count > 0) ? allslotList.Select(x=>x.DoctorId) : new List<int>())}
                };
                var doctorDetails = _doctor.GetDoctorDetails(StoredProcedureList.GetDoctorDetails_v1, docIdsForDetails);
                if (allslotList.Any())
                {
                    List<int> tempFacility;
                    var specities = _speciality.GetAll(x => x.IsActive && !x.IsDeleted)
                        .Select(x => new SelectIdValueModel { Id = x.SpecialityId, Value = x.SpecialityName }).ToList();

                    foreach (var item in allslotList)
                    {
                        if (doctorDetails != null && doctorDetails.Count > 0)
                        {
                            var lstSpecialities = doctorDetails.First(x => x.DoctorId == item.DoctorId).ListSpecities;
                            if (!string.IsNullOrEmpty(lstSpecialities))
                            {
                                item.Specities = lstSpecialities.Split(',').ToList();
                            }
                            //item.SpecitiesIds = doctorDetails.First(x => x.DoctorId == item.DoctorId).SpecitiesIds;
                            item.FacilityIds = doctorDetails.First(x => x.DoctorId == item.DoctorId).FacilityIds;
                            item.ReviewCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).ReviewCount;
                            item.InsuranceCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).InsuaranceCount;
                            item.RatingNos = doctorDetails.First(x => x.DoctorId == item.DoctorId).RatingNos;
                            item.OfficeCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).OfficeCount;
                        }
                        /*if (!string.IsNullOrEmpty(item.SpecitiesIds)) commented Specialites from Taxonomy table
                        {
                            tempSpecities = new List<int>();
                            item.SpecitiesIds.Split(',').Select(x => Convert.ToInt32(x)).ToList().ForEach(x => tempSpecities.Add(x));
                            item.Specities = specities.Where(x => tempSpecities.Contains(x.Id)).Select(x => x.Value).ToList();
                        }
                        */
                        if (!string.IsNullOrEmpty(item.FacilityIds))
                        {
                            tempFacility = new List<int>();
                            item.FacilityIds.Split(',').Select(x => Convert.ToInt32(x)).ToList().ForEach(x => tempFacility.Add(x));

                            item.Facility = _facility.GetAll(x => x.IsActive && !x.IsDeleted && tempFacility.Contains(x.OrganisationId))
                            .Select(x => new OrgFacitiesInfo { Id = x.OrganisationId, Name = x.OrganisationName, OrgNpi = x.NPI, OrgType = x.OrganizationTypeID }).ToList();

                        }
                    }
                }
                var searchResult = new Doctyme.Model.ViewModels.SearchDoctorResult() { listDoctors = allslotList };
                if (allslotList != null & allslotList.Count > 0)
                    searchResult.TotalRecord = allslotList.FirstOrDefault().TotalRecordCount;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;

                return PartialView(@"Partial/_Search", searchResult);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "_Search-POST");
                return PartialView(@"Partial/_Search", new ViewModels.SearchDoctorResult());
            }
        }

        [HttpPost, Route("GetDoctorSearchFilter/")]
        public JsonResult GetDoctorSearchFilter(SearchParameterModel request)
        {
            try
            {
                var model = SearchBindFilter(request);
                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetDoctorSearchFilter-POST");
                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = new SearchViewModel() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost, Route("MapSearch")]
        public ActionResult MapSearch(string searchText)
        {
            try
            {
                var latOfUser = string.Empty;
                var longOfUser = string.Empty;

                try
                {
                    if (_ipInfo != null && !string.IsNullOrEmpty(_ipInfo.Loc))
                    {
                        latOfUser = _ipInfo.Loc.Split(',')[0];
                        longOfUser = _ipInfo.Loc.Split(',')[1];
                    }
                }
                catch
                {
                    latOfUser = null;
                    longOfUser = null;
                }
                var doctorIds = new List<int>();
                decimal lat2 = 0;
                decimal.TryParse(latOfUser, out lat2);
                decimal long2 = 0;
                decimal.TryParse(longOfUser, out long2);
                var model = new SearchParameterModel() { Search = searchText };
                var paras = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = searchText??""}
                };
                var doctorIdsWithDistanceList = new List<DoctorWithDistance>();
                var featureDoctorIds = _doctor.GetFeaturedDoctorIdsBySearchText(StoredProcedureList.GetFeaturedDoctorIdS, paras);
                if (!string.IsNullOrEmpty(model.Search))
                {
                    var specDoctorIds = _doctor.GetDoctorIdBySpecialityName(model.Search);
                    doctorIds.AddRange(specDoctorIds);
                    doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(specDoctorIds, lat2, long2));
                }

                //doctorIdsWithDistanceList.AddRange(_doctor.GetDoctorIdByNameOrAddress(model.Search, model.NTPA, model.ANP, model.PrimaryCare, model.Distance.SearchBox, lat2, long2));
                doctorIdsWithDistanceList.AddRange(_doctor.GetDoctorIdByNameOrAddress(model.Search, model.NTPA, model.ANP, model.PrimaryCare, "", lat2, long2));
                doctorIdsWithDistanceList = doctorIdsWithDistanceList.DistinctBy(x => x.DoctorId).OrderBy(m => m.DistanceCount).ToList();

                var docIDsToFetch = new List<int>();
                var pageIndex = model.PageIndex == 0 ? 0 : model.PageIndex - 1;
                if (featureDoctorIds != null && featureDoctorIds.Count > 0)
                {
                    if (featureDoctorIds.Count > (pageIndex * model.PageSize))
                    {
                        docIDsToFetch.AddRange(featureDoctorIds.Skip(pageIndex * model.PageSize).Take(model.PageSize).Select(m => m.ReferenceId).ToList());
                    }
                }

                if (docIDsToFetch.Count < model.PageSize)
                    docIDsToFetch.AddRange(doctorIdsWithDistanceList.Skip(pageIndex * model.PageSize).Take(model.PageSize - docIDsToFetch.Count).Select(x => x.DoctorId).ToList());


                // get all match result for map .. no pagination
                int totalRecords_Map = doctorIdsWithDistanceList.Count;
                var mapSearchparameters = GetSearchParameters(model, true, doctorIds);
                var mapResult = _doctor.GetDoctorSearchListPagination(StoredProcedureList.SearchDoctorList, mapSearchparameters);
                var docIdsForDetails = new List<SqlParameter>()
                {
                    new SqlParameter("@DoctorIds",SqlDbType.NVarChar){ Value = string.Join(",",(mapResult != null && mapResult.Count > 0) ? mapResult.Select(x=>x.DoctorId) : new List<int>())}
                };
                var doctorDetails = _doctor.GetDoctorDetails(StoredProcedureList.GetDoctorDetails, docIdsForDetails);
                if (mapResult.Any())
                {
                    foreach (var item in mapResult)
                    {
                        if (doctorDetails != null && doctorDetails.Count > 0)
                        {
                            item.SpecitiesIds = doctorDetails.First(x => x.DoctorId == item.DoctorId).SpecitiesIds;
                            item.FacilityIds = doctorDetails.First(x => x.DoctorId == item.DoctorId).FacilityIds;
                            item.ReviewCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).ReviewCount;
                            item.InsuranceCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).InsuaranceCount;
                            item.RatingNos = doctorDetails.First(x => x.DoctorId == item.DoctorId).RatingNos;
                            item.OfficeCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).OfficeCount;
                        }
                    }
                    return Json(new { result = mapResult.ToList() });
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "MapSearch-POST");
                return null;
            }
        }

        private List<SqlParameter> GetDoctorSearchParameters(SearchParameterModel searchModel, bool forMap, List<int> doctorIds)
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@Latitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@Longitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@Language", SqlDbType.Int) {Value = searchModel.Language},
                new SqlParameter("@AGS",SqlDbType.NVarChar) {Value = searchModel.AGS == null ? "" : string.Join(",", searchModel.AGS )},
                new SqlParameter("@DoctorIds", SqlDbType.NVarChar) { Value = string.Join(",", doctorIds) },
                new SqlParameter("@SearchLocationKey",SqlDbType.NVarChar,250) {Value = searchModel.locationSearch == null ? "" : searchModel.locationSearch },
            };

            return parameters;
        }
        private List<SqlParameter> GetAllDoctorSearchParameters(SearchParameterModel searchModel)
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@SearchName", SqlDbType.NVarChar)  {Value = searchModel.Search == null ? "" : searchModel.Search },
                new SqlParameter("@SearchLocationKey",SqlDbType.NVarChar,250) {Value = searchModel.locationSearch == null ? "" : searchModel.locationSearch },
                new SqlParameter("@Latitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@Longitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@AcceptNewPatient", SqlDbType.Int) {Value = searchModel.ANP==true ? 1 :0 },
                new SqlParameter("@NtPcp", SqlDbType.Int) {Value = searchModel.NTPA==true ? 1 :0 },
                new SqlParameter("@PrimaryCare", SqlDbType.Int) {Value = searchModel.PrimaryCare==true ? 1 :0 },
                new SqlParameter("@Specialities", SqlDbType.NVarChar) {Value = searchModel.Specialization== null ? "":searchModel.Specialization},
                new SqlParameter("@SpecialitiesId", SqlDbType.Int) {Value = searchModel.Specialties},
                new SqlParameter("@InsuranceTypeId",SqlDbType.NVarChar) {Value = searchModel.Insurance == null ? "" : string.Join(",", searchModel.Insurance )},
                new SqlParameter("@GenderTypeId",SqlDbType.NVarChar) {Value = searchModel.GenderTypeIds == null ? "" : string.Join(",", searchModel.GenderTypeIds )},
                new SqlParameter("@Language", SqlDbType.Int) {Value = searchModel.Language},
                new SqlParameter("@AgeGroupId",SqlDbType.NVarChar) {Value = searchModel.AgeGroup == null ? "" : string.Join(",", searchModel.AgeGroup )},
               // new SqlParameter("@OrganisationId",SqlDbType.NVarChar)  {Value = searchModel.Affiliations == null ? "" : string.Join(",", searchModel.Affiliations )},
                new SqlParameter("@pageIndex",SqlDbType.Int) {Value = searchModel.PageIndex == 0 ? 1 : searchModel.PageIndex },
                new SqlParameter("@pageNumber",SqlDbType.Int) {Value = searchModel.PageSize == 0 ? 10 : searchModel.PageSize }
            };

            return parameters;
        }
        private List<SqlParameter> GetFilterSearchParameters(SearchParameterModel searchModel)
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@SearchName", SqlDbType.NVarChar)  {Value = searchModel.Search == null ? "" : searchModel.Search },
                new SqlParameter("@SearchLocationKey",SqlDbType.NVarChar,250) {Value = searchModel.locationSearch == null ? "" : searchModel.locationSearch },
                new SqlParameter("@Latitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@Longitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@AcceptNewPatient", SqlDbType.Int) {Value = searchModel.ANP==true ? 1 :0 },
                new SqlParameter("@NtPcp", SqlDbType.Int) {Value = searchModel.NTPA==true ? 1 :0 },
                new SqlParameter("@PrimaryCare", SqlDbType.Int) {Value = searchModel.PrimaryCare==true ? 1 :0 },
                new SqlParameter("@Specialities", SqlDbType.NVarChar) {Value = searchModel.Specialization== null ? "":searchModel.Specialization},
                new SqlParameter("@SpecialitiesId", SqlDbType.Int) {Value = searchModel.Specialties},
                new SqlParameter("@InsuranceTypeId",SqlDbType.NVarChar) {Value = searchModel.Insurance == null ? "" : string.Join(",", searchModel.Insurance )},
                new SqlParameter("@GenderTypeId",SqlDbType.NVarChar) {Value = searchModel.GenderTypeIds == null ? "" : string.Join(",", searchModel.GenderTypeIds )},
                new SqlParameter("@Language", SqlDbType.Int) {Value = searchModel.Language},
                new SqlParameter("@AgeGroupId",SqlDbType.NVarChar) {Value = searchModel.AgeGroup == null ? "" : string.Join(",", searchModel.AgeGroup )},

            };

            return parameters;
        }
        private List<SqlParameter> GetAllDoctorSearchFilterParameters(SearchParameterModel searchModel)
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@SearchName", SqlDbType.NVarChar)  {Value = searchModel.Search == null ? "" : searchModel.Search },
                new SqlParameter("@SearchLocationKey",SqlDbType.NVarChar,250) {Value = searchModel.locationSearch == null ? "" : searchModel.locationSearch },
                new SqlParameter("@Latitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@Longitude",SqlDbType.NVarChar) {Value =  "0"},
                new SqlParameter("@AcceptNewPatient", SqlDbType.Int) {Value = searchModel.ANP==true ? 1 :0 },
                new SqlParameter("@NtPcp", SqlDbType.Int) {Value = searchModel.NTPA==true ? 1 :0 },
                new SqlParameter("@PrimaryCare", SqlDbType.Int) {Value = searchModel.PrimaryCare==true ? 1 :0 },
                new SqlParameter("@SpecialitiesId", SqlDbType.Int) {Value = searchModel.Specialties},
                new SqlParameter("@InsuranceTypeId",SqlDbType.NVarChar) {Value = searchModel.Insurance == null ? "" : string.Join(",", searchModel.Insurance )},
                new SqlParameter("@GenderTypeId",SqlDbType.NVarChar) {Value = searchModel.GenderTypeIds == null ? "" : string.Join(",", searchModel.GenderTypeIds )},
                new SqlParameter("@Language", SqlDbType.Int) {Value = searchModel.Language},
                new SqlParameter("@AgeGroupId",SqlDbType.NVarChar) {Value = searchModel.AgeGroup == null ? "" : string.Join(",", searchModel.AgeGroup )},
               // new SqlParameter("@OrganisationId",SqlDbType.NVarChar)  {Value = searchModel.Affiliations == null ? "" : string.Join(",", searchModel.Affiliations )},
            };

            return parameters;
        }
        private List<SqlParameter> GetSearchParameters(SearchParameterModel searchModel, bool forMap, List<int> doctorIds)
        {
            //string ags = string.Empty;
            //if (searchModel.AGS.Female && searchModel.AGS.Male)
            //    ags = "Female,Male";
            //else if (searchModel.AGS.Female)
            //    ags = "Female";
            //else if (searchModel.AGS.Male)
            //    ags = "Male";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@Search", SqlDbType.NVarChar) { Value = searchModel.Search??""},
                new SqlParameter("@Distance",SqlDbType.Int) {Value = searchModel.Distance.DistanceType == null ? 0 : Convert.ToInt32(searchModel.Distance.DistanceType)},
                new SqlParameter("@DistanceSearch",SqlDbType.NVarChar) {Value = searchModel.Distance.SearchBox ?? ""},
                new SqlParameter("@Latitude",SqlDbType.NVarChar) {Value = searchModel.Distance.Latitude ?? "0"},
                new SqlParameter("@Longitude",SqlDbType.NVarChar) {Value = searchModel.Distance.Longitude ?? "0"},
                new SqlParameter("@IsAllowNewPatient",SqlDbType.Bit) {Value = searchModel.ANP},
                new SqlParameter("@IsNtPcp",SqlDbType.Bit) {Value = searchModel.NTPA},
                new SqlParameter("@IsPrimaryCare", SqlDbType.Bit) {Value = searchModel.PrimaryCare},
                new SqlParameter("@Specialties", SqlDbType.Int) {Value = searchModel.Specialties},
                new SqlParameter("@Language", SqlDbType.Int) {Value = searchModel.Language},
                new SqlParameter("@Affiliations",SqlDbType.NVarChar) {Value = (searchModel.Affiliations == null || searchModel.Affiliations.Count() < 0)  ? "" : string.Join(",", searchModel.Affiliations)},
                new SqlParameter("@Insurance",SqlDbType.NVarChar) {Value = searchModel.Insurance == null ? "" : string.Join(",", searchModel.Insurance )},
                new SqlParameter("@AGS",SqlDbType.NVarChar) {Value = searchModel.AGS == null ? "" : string.Join(",", searchModel.AGS )},
                new SqlParameter("@AGSFull",SqlDbType.NVarChar) {Value = searchModel.AgeGroup == null ? "" : string.Join(",", searchModel.AgeGroup )},
                new SqlParameter("@Sorting", SqlDbType.VarChar) { Value = searchModel.Sorting ?? "" },

        };
            if (!forMap)
            {
                searchModel.PageSize = searchModel.PageSize == 0 ? 10 : searchModel.PageSize;
                searchModel.PageIndex = searchModel.PageIndex == 0 ? 1 : searchModel.PageIndex;
                parameters.Add(new SqlParameter("@PageIndex", SqlDbType.Int) { Value = searchModel.PageIndex == 0 ? 0 : searchModel.PageIndex - 1 });
                parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = searchModel.PageSize });
            }
            else
            {
                parameters.Add(new SqlParameter("@PageIndex", SqlDbType.Int) { Value = 0 });
                parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = short.MaxValue });
            }

            var pId = searchModel.PageIndex == 0 ? 0 : searchModel.PageIndex - 1;
            if (pId > 0)
            {
                //featuredDoctors = featuredDoctors.Skip(searchModel.PageSize).ToList();
            }

            parameters.Add(new SqlParameter("@DoctorIds", SqlDbType.NVarChar) { Value = string.Join(",", doctorIds) });


            return parameters;
        }

        [Route("Doctor-Profile/{npi?}")]
        public ActionResult DoctorProfile(string npi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(npi))
                {
                    return RedirectToAction("Index", "Home");
                }


                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetDoctorProfileData + " " + npi);
                var doctorProfile = Common.ConvertDataTable<DoctorProfileViewModel>(ds.Tables[0]).FirstOrDefault();
                doctorProfile.Reviews = Common.ConvertDataTable<Review>(ds.Tables[8]).ToList();
                doctorProfile.Qualifications = Common.ConvertDataTable<Qualification>(ds.Tables[1]).ToList();
                doctorProfile.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[2]).ToList();
                doctorProfile.Experiences = Common.ConvertDataTable<Experience>(ds.Tables[3]).ToList();
                doctorProfile.DoctorBoardCertifications = Common.ConvertDataTable<DoctorBoardCertification>(ds.Tables[4]).ToList();
                doctorProfile.DoctorLanguages = Common.ConvertDataTable<DoctorLanguage>(ds.Tables[5]).ToList();
                doctorProfile.SocialMediaLinks = Common.ConvertDataTable<SocialMedia>(ds.Tables[6]).ToList();
                doctorProfile.DoctorImages = Common.ConvertDataTable<SiteImage>(ds.Tables[7]).ToList();

                return View(doctorProfile);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "DoctorProfile-Search");
                return View(new DoctorProfileViewModel());
            }
        }

        [HttpGet, Route("_RequestAppointment/{id?}")]
        public PartialViewResult RequestAppointment(int id = 0)
        {
            try
            {
                ViewBag.DoctorId = id;
                ViewBag.LocationList = _address.GetAll(x => x.IsActive && !x.IsDeleted && x.ReferenceId == id)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Address1 /*+ x.CityStateZip.StateName + x.City.CityName + x.Country.CountryName*/ + x.ZipCode, // instead of FullAddress
                        Value = x.AddressId.ToString(),
                        //Selected = x.IsDefault
                    }).ToList();
                return PartialView(@"Partial/_RequestAppointment", new RequestSlotViewModel());
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "RequestAppointment-GET");
                return PartialView(@"Partial/_RequestAppointment", new RequestSlotViewModel());
            }
        }

        [HttpGet, Route("LoadTimeSlots/{id}/{selectedDate}")]
        public PartialViewResult LoadTimeSlots(int id, string selectedDate)
        {
            try
            {
                var date = DateTime.Now.Date;
                var slotList = _slot.GetAll(x => x.IsActive && !x.IsDeleted && x.ReferenceId == id && Convert.ToDateTime(x.SlotDate) >= date).OrderBy(d => d.SlotDate).ToList();

                //slotList.Sort((a, b) => a.SlotDate.CompareTo(b.SlotDate));

                var result = new List<DateSlotModel>();
                DateSlotModel tempDateSlot = null;
                var timeSlotList = new List<TimeSlotModel>();

                var prevDate = string.Empty;
                var currDate = string.Empty;
                foreach (var slot in slotList)
                {
                    currDate = Convert.ToDateTime(slot.SlotDate).ToString("MM'/'dd'/'yyyy");
                    if (currDate != prevDate)
                    {
                        tempDateSlot = new DateSlotModel
                        {
                            Date = currDate,
                            TimeSlots = new List<TimeSlotModel>()
                        };
                    }

                    tempDateSlot.TimeSlots.Add(new TimeSlotModel
                    {
                        Id = slot.SlotId,
                        Time = Convert.ToDateTime(slot.SlotTime).ToString("hh:mm tt"),
                        IsBook = slot.IsActive
                    });

                    if (currDate != prevDate)
                    {
                        result.Add(tempDateSlot);
                    }

                    prevDate = currDate;
                }

                return PartialView(@"Partial/LoadTimeSlots", result);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "LoadTimeSlots-GET");
                return PartialView(@"Partial/LoadTimeSlots", new List<DateSlotModel>());
            }
        }

        [HttpGet, Route("RequestAppointmentScreen2/{slotId}/{id?}")]
        public PartialViewResult RequestAppointmentScreen2(long slotId, int id = 0)
        {
            var model = new RequestAppointmentViewModel();
            if (Request.IsAuthenticated && User.IsInRole(UserRoles.Patient))
            {
                int userId = User.Identity.GetUserId<int>();
                var result = _patient.GetSingle(x => x.IsActive && !x.IsDeleted && x.UserId == userId);
                model.Id = result.PatientId;
                model.FirstName = result.PatientUser.FirstName;
                model.MiddleName = result.PatientUser.MiddleName;
                model.LastName = result.PatientUser.LastName;
                model.DateOfBirth = result.PatientUser.DateOfBirth.ToDefaultFormate("dd-MMM-yyyy");
                model.Email = result.PatientUser.Email;
                model.PhoneNumber = result.PatientUser.PhoneNumber;
                var address = result.Address.FirstOrDefault();
                model.AddressView.Address1 = address.Address1;
                model.AddressView.CityId = address.CityId;
                model.AddressView.StateId = address.StateId;
                model.AddressView.ZipCode = address.ZipCode;
                var requestSlot = _slot.GetById(slotId);
                model.RequestSlot.AppTime = Convert.ToDateTime(requestSlot.SlotTime).ToString("hh: mm tt");
                model.RequestSlot.AppDate = Convert.ToDateTime(requestSlot.SlotDate).ToDefaultFormate("MM-dd-yyyy");
                model.RequestSlot.Location = "";
            }
            else
            {
                TempData["SlotId"] = slotId;
                TempData["LocationId"] = id;
                TempData.Keep();
                model.GroupTypeId = Common.GetUserTypes().FirstOrDefault(x => x.Text.ToLower().Equals(("Patient").ToLower()))?.Value;
            }
            return PartialView(@"Partial/_RequestAppointmentScreen2", model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("AddEditRequestAppointmentScreen2")]
        public async System.Threading.Tasks.Task<JsonResult> AddEditRequestAppointmentScreen2Async(RequestAppointmentViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (Request.IsAuthenticated && User.IsInRole(UserRoles.Patient))
                        {
                        }
                        else
                        {
                            var slotDetail = (dynamic)TempData["SlotDetail"];

                            var user = new ApplicationUser
                            {
                                FirstName = model.FirstName,
                                MiddleName = model.MiddleName,
                                LastName = model.LastName,
                                UserName = model.Email,
                                Email = model.Email,
                                ProfilePicture = StaticFilePath.ProfilePicture,
                                CreatedDate = DateTime.UtcNow,
                                LastResetPassword = DateTime.UtcNow,
                                IsActive = true,
                                IsDeleted = false,
                                RegisterViewModel = JsonConvert.SerializeObject(model)
                            };

                            var isExist = await _userManager.FindByEmailAsync(user.Email);
                            if (isExist != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Username already exist.." }, JsonRequestBehavior.AllowGet);
                            }

                            var result = await _userManager.CreateAsync(user, model.Password);
                            if (!result.Succeeded)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                            }
                            await _userManager.AddToRoleAsync(user.Id, "Patient");

                            var tempSlot = new TempSlotBooking()
                            {
                                PatientId = user.Id,
                                SlotId = Convert.ToInt32(TempData["SlotId"]),
                                IsActive = true,
                                LocationId = Convert.ToInt32(TempData["LocationId"]),
                            };
                            _tempSlotBookingService.InsertData(tempSlot);
                            _tempSlotBookingService.SaveData();

                            txscope.Complete();
                            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            string body = Common.ReadEmailTemplate(EmailTemplate.ConfirmEmail);
                            body = body.Replace("{UserName}", user.FullName)
                                .Replace("{action_url}", callbackUrl);
                            SendMail.SendEmail(user.Email, "", "", "", body, "Confirm your account");

                            return Json(new JsonResponse { Status = 1, Message = "Please Check your email account for account confirmation." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.LogError(ex, "AddEditRequestAppointmentScreen2Async-POST");
                    return Json(new JsonResponse { Status = 0, Message = "Something wrong happened, Please try again after sometime." }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new JsonResponse { }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_DoctorReview/{id?}")]
        public PartialViewResult DoctorReview(int id)
        {
            return PartialView(@"Partial/_DoctorReview", new ReviewViewModel() { DoctorId = id });
        }
        #endregion

        [Route("Filter/Pharmacy/{text?}")]
        public ActionResult Pharmacy(string text, string skey, string slocation)
        {
            //ViewBag.DistanceList = DistanceList();
            ViewBag.SearchBox = skey;
            AssignLocationValues(slocation);
            ViewBag.OrganisationType = OrganisationTypeConstants.Pharmacy;
            return View();
        }
        [Route("Filter/Facility/{text?}")]
        public ActionResult Facility(string text, string skey, string slocation)
        {
            try
            {
                //ViewBag.DistanceList = DistanceList();
                ViewBag.SearchBox = skey;
                AssignLocationValues(slocation);
                ViewBag.OrganisationType = OrganisationTypeConstants.Facility;
                //ViewBag.Facilityoption = _facility.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new KeyValuePairModel
                //{
                //    Id = x.OrganisationId.ToString(),
                //    Name = x.OrganisationName,
                //    Count = 2
                //}).ToList();

                //ViewBag.Facilitytype = _facility.GetDrpFacilityTypeList(SqlQuery.GeFacilityTypeDropDownList, new object[] { }).Select(x => new KeyValuePairModel
                //{
                //    Id = x.OrganizationTypeID.ToString(),
                //    Name = x.OrganizationTypeName,
                //    Count = 2
                //}).ToList();

                //ViewBag.Facilitytype = _facility.GetAll(x => x.IsActive && !x.IsDeleted).Where(q => q.OrganisationType != null).Select(x => new KeyValuePairModel
                //{
                //    Id = x.OrganizationTypeID.ToString(),
                //    Name = x.OrganisationType.Org_Type_Name,
                //    Count = 2
                //}).ToList();

                return View();
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Facility-GET");
                return View();
            }
        }
        [Route("Filter/SeniorCare/{text?}")]
        public ActionResult SeniorCare(string text, string skey, string slocation)
        {
            //ViewBag.DistanceList = DistanceList();
            ViewBag.SearchBox = skey;
            AssignLocationValues(slocation);
            ViewBag.OrganisationType = OrganisationTypeConstants.SeniorCare;
            return View();
        }

        [HttpPost, Route("OrganisationMapSearch")]
        public ActionResult OrganisationMapSearch(SearchParamModel searchModel, int typeId, int userTypeId)
        {
            try
            {
                var searchResult = GetOrganizationSearchResult(searchModel, typeId);
                if (searchResult.OrganizationProviderList.Any())
                    return Json(new { result = searchResult.OrganizationProviderList });
                else
                    return null;
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "OrganisationMapSearch-POST");
                return null;
            }
        }

        #region Pharmacy Search
        private OrganizationProviderListModel GetOrganizationSearchResult(SearchParamModel model, int organizationTypeId)
        {
            try
            {
                var latOfUser = string.Empty;
                var longOfUser = string.Empty;

                try
                {
                    if (!string.IsNullOrEmpty(_ipInfo.Loc))
                    {
                        latOfUser = _ipInfo.Loc.Split(',')[0];
                        longOfUser = _ipInfo.Loc.Split(',')[1];
                    }
                }
                catch
                {
                    latOfUser = null;
                    longOfUser = null;
                }

                decimal lat2 = 0;
                decimal.TryParse(latOfUser, out lat2);
                decimal long2 = 0;
                decimal.TryParse(longOfUser, out long2);
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;
                var parameters = new List<SqlParameter>();
                var paras = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox??""},
                    new SqlParameter("@OrganizationTypeID",SqlDbType.Int) {Value = organizationTypeId}
                };
                var featureFacilityIds = _doctor.GetFeaturedFacilityIdsBySearchText(StoredProcedureList.GetFeaturedFacilityIds, paras);
                var pageIndex = model.PageIndex == 0 ? 0 : model.PageIndex - 1;
                var lstOrgIds = new List<int>();
                if (featureFacilityIds != null && featureFacilityIds.Count > 0)
                    if (featureFacilityIds.Count > (pageIndex * model.PageSize))
                        lstOrgIds.AddRange(featureFacilityIds.Skip(pageIndex * model.PageSize).Take(model.PageSize).Select(m => m.ReferenceId).ToList());

                int totalRecord = 0;
                model.Distance.Latitude = string.IsNullOrEmpty(latOfUser) ? model.Distance.Latitude : latOfUser;
                model.Distance.Longitude = string.IsNullOrEmpty(longOfUser) ? model.Distance.Longitude : longOfUser;
                if (lstOrgIds.Count < model.PageSize)
                {
                    var take = model.PageSize - lstOrgIds.Count;
                    var skip = 0;
                    if (featureFacilityIds == null || featureFacilityIds.Count == 0)
                        skip = pageIndex * model.PageSize;
                    else if (featureFacilityIds != null && featureFacilityIds.Count > 0 && lstOrgIds.Count > 0)
                        skip = 0;
                    else
                        skip = (pageIndex == 0 ? 0 : pageIndex - 1) - featureFacilityIds.Count;
                    var orgIds = _featuredService.GetOrganisationIdsFromSearchText(model.SearchBox, organizationTypeId, Convert.ToDecimal(model.Distance.Latitude), Convert.ToDecimal(model.Distance.Longitude),
                        model.Distance.DistanceType == null ? 0 : Convert.ToInt32(model.Distance.DistanceType), skip, take, out totalRecord);
                    lstOrgIds.AddRange(orgIds.Select(m => m.OrganisationId).ToList());
                }

                totalRecord = totalRecord + featureFacilityIds.Count;
                //var advertisementList = _featuredService.GetAdvertisementsFromSearchText(lstOrgIds, organizationTypeId);//Added by Reena
                //var advertisementList = _patient.ExecWithStoreProcedure<Doctyme.Model.ViewModels.Advertisements>("spGetAdvertisements_ByOrgTypeId @organisationId, @userTypeId",
                //   new SqlParameter("organisationId", System.Data.SqlDbType.VarChar) { Value = string.Join(",", lstOrgIds) },
                //   new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = organizationTypeId }).ToList();
                parameters.Add(new SqlParameter("@OrganisationIds", SqlDbType.NVarChar) { Value = string.Join(",", lstOrgIds) });
                var advList = _patient.ExecWithStoreProcedure<ProviderAdvertisements>("spGetAdvertisements_ByOrgTypeId @userTypeId",
                   new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = organizationTypeId }).ToList();
                var advertisementList = _doctor.GetDistanceMilebyOrgIds(advList, lat2, long2);
                var allslotList = _featuredService.GetOrganisationListByTypeId_HomePageSearch(StoredProcedureList.GetOrganisationListByTypeId_HomePageSearch, parameters).ToList();
                var searchResult = new OrganizationProviderListModel() { OrganizationProviderList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";
                searchResult.TotalRecord = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;
                ViewBag.AdvList = advertisementList;//Added by Reena
                return searchResult;
            }
            catch (Exception ex)
            {
                //throw;
                Common.LogError(ex, "GetOrganizationSearchResult-Search");
                return new OrganizationProviderListModel();
            }
        }
        private OrganizationProviderListModel GetOrganizationSearchResult(SearchParamModel model, int organizationTypeId, int userTypeId)
        {
            try
            {
                var latOfUser = string.Empty;
                var longOfUser = string.Empty;

                try
                {
                    if (_ipInfo != null && !string.IsNullOrEmpty(_ipInfo.Loc))
                    {
                        latOfUser = _ipInfo.Loc.Split(',')[0];
                        longOfUser = _ipInfo.Loc.Split(',')[1];
                    }
                }
                catch
                {
                    latOfUser = null;
                    longOfUser = null;
                }

                decimal lat2 = 0;
                decimal.TryParse(latOfUser, out lat2);
                decimal long2 = 0;
                decimal.TryParse(longOfUser, out long2);
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;
                var parameters = new List<SqlParameter>();
                var paras = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox??""},
                    new SqlParameter("@OrganizationTypeID",SqlDbType.Int) {Value = organizationTypeId}
                };
                // var featureFacilityIds = _doctor.GetFeaturedFacilityIdsBySearchText(StoredProcedureList.GetFeaturedFacilityIds, paras);
                var pageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.Distance.Latitude = string.IsNullOrEmpty(latOfUser) ? model.Distance.Latitude : latOfUser;
                model.Distance.Longitude = string.IsNullOrEmpty(longOfUser) ? model.Distance.Longitude : longOfUser;
                var advList = _patient.ExecWithStoreProcedure<ProviderAdvertisements>("spGetAdvertisements_ByOrgTypeId @userTypeId",
                   new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = organizationTypeId }).ToList();
                var advertisementList = _doctor.GetDistanceMilebyOrgIds(advList, lat2, long2);
                var allslotList = GetOrganisationListByTypeId(model, organizationTypeId, userTypeId);
                var searchResult = new OrganizationProviderListModel() { OrganizationProviderList = allslotList.ToList() };
                ViewBag.SearchBox = model.SearchBox ?? "";
                searchResult.TotalRecord = allslotList.FirstOrDefault() == null ? 0 : allslotList.FirstOrDefault().TotalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;
                ViewBag.AdvList = advertisementList;//Added by Reena
                return searchResult;
            }
            catch (Exception ex)
            {
                //throw;
                Common.LogError(ex, "GetOrganizationSearchResult-Search");
                return new OrganizationProviderListModel();
            }
        }

        private IList<OrganizationProviderModel> GetOrganisationListByTypeId(SearchParamModel model, int organizationTypeId, int userTypeId)
        {
            if (!string.IsNullOrEmpty(model.SearchLocation))
            {
                var splitarr = model.SearchLocation.Split(',').Select(p => p.Trim()).ToList();
                model.SearchLocation = string.Join("|", splitarr.ToArray());
            }
            else
            {
                model.SearchLocation = "||";
            }

            var paras = new List<SqlParameter>()
                {
                   new SqlParameter("@OrganizationTypeId",SqlDbType.Int) {Value = organizationTypeId},
                   new SqlParameter("@userTypeId",SqlDbType.Int) {Value = userTypeId},
                   new SqlParameter("@SearchName", SqlDbType.NVarChar) { Value = model.SearchBox??""},
                   new SqlParameter("@SearchLocationKey", SqlDbType.NVarChar,250) { Value = model.SearchLocation??""},
                   new SqlParameter("@Specialities", SqlDbType.NVarChar,250) { Value = model.Specialities??""},
                   new SqlParameter("@pageIndex", SqlDbType.Int) { Value = model.PageIndex},
                   new SqlParameter("@pageSize", SqlDbType.Int) { Value = model.PageSize},
              };
            var result = _featuredService.GetOrganisationListByTypeId_HomePageSearch(StoredProcedureList.GetListOrganisationByType_v1, paras).ToList();
            return result;
        }

        [HttpPost, Route("_SearchPharmacy")]
        public PartialViewResult _SearchPharmacy(SearchParamModel model)
        {
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.Pharmacy, UserTypes.Pharmacy);//GetOrganizationSearchResult(model, OrganisationTypes.Pharmacy);
            return PartialView(@"Partial/_SearchPharmacy", searchResult);
        }

        [Route("home/Pharmacy/{id?}")]
        public ActionResult PharmacyProfile(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index", "Pharmacy");
                }
                var pharmacy = _pharmacy.GetSingle(x => x.OrganisationId == id);
                return View(pharmacy);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "PharmacyProfile-GET");
                return View(new Organisation());
            }
        }
        #endregion

        #region SeniorCare
        [HttpPost, Route("_SearchSeniorCare")]
        public PartialViewResult _SeniorCare(SearchParamModel model)
        {
            //var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.SeniorCare);
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.SeniorCare, UserTypes.SeniorCare);
            return PartialView(@"Partial/_SearchSeniorCare", searchResult);
        }

        [Route("home/SeniorCare/{id?}")]
        public ActionResult SeniorCareProfile(int id)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index", "SeniorCare");
                }
                var seniorCare = _seniorCare.GetSingle(x => x.OrganisationId == id);

                return View(seniorCare);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "PharmacyProfile-GET");
                return View(new Organisation());
            }
        }
        [HttpPost]
        public void setBookingHour(int id)
        {
            _bookHour = id;
        }
        #endregion

        #region Facility
        [HttpPost, Route("_SearchFacility")]
        public PartialViewResult _Facility(SearchParamModel model)
        {
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.Facility); //GetOrganizationSearchResult(model, OrganisationTypes.Facility, UserTypes.Facility); // GetOrganizationSearchResult(model, OrganisationTypes.Facility);
            return PartialView(@"Partial/_SearchFacility", searchResult);
        }

        [Route("home/Facility/{id?}")]
        public ActionResult FacilityProfile(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index", "SeniorCare");
                }
                var facility = _facility.GetSingle(x => x.OrganisationId == id);
                return View(facility);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "FacilityProfile-GET");
                return View(new Organisation());
            }
        }
        #endregion

        #region Drugs


        #region Search Drug 
        // Created by Ajit 




        #endregion
        [HttpGet, Route("SearchDrug/{search?}")]
        public ActionResult SearchDrug(string search)
        {
            try
            {
                ViewBag.SearchBox = search;
                return View();
            }
            catch (Exception ex)
            {
                //Common.LogError(ex, "SearchDrug-GET");
                //return View();
                throw;
            }
        }

        [HttpGet, Route("SearchDrug/GetDrugFliterDetails/{search?}/{strength?}/{type?}/{quantity?}/{manufacturer?}")]
        public JsonResult GetDrugFliterDetails(String search, string strength, string type, string quantity, string manufacturer)
        {
            try
            {
                ViewBag.SearchBox = search;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = search}
                };

                var searchRecords = new List<SearchDrugRecord>();
                var searchRecordWithPageSize = new Records();

                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetDrugDetailsPageData + " '" + search + "'");



                DrugDetailsViewModel drugDetail = new DrugDetailsViewModel();
                drugDetail.DrugInfo = Common.ConvertDataTable<DrugInfoViewModel>(ds.Tables[0]).FirstOrDefault();
                drugDetail.DrugTabsInfo = Common.ConvertDataTable<DrugTabsInfoViewModel>(ds.Tables[1]).ToList();
                drugDetail.DrugTypeTabletFilter = Common.ConvertDataTable<DrugTypeFilterViewModel>(ds.Tables[2]).OrderBy(x => x.Name).ToList();
                drugDetail.DrugManufacturerFilter = Common.ConvertDataTable<DrugManufacturerFilterViewModel>(ds.Tables[3]).OrderBy(x => x.CompanyName).ToList();
                drugDetail.DrugStrengthFilter = Common.ConvertDataTable<DrugStrengthFilterViewModel>(ds.Tables[4]).OrderBy(x => x.Name).ToList();
                drugDetail.DrugRelatedInfo = Common.ConvertDataTable<DrugInfoViewModel>(ds.Tables[6]).OrderBy(x => x.DrugName).ToList();
                drugDetail.DrugFilter = new DrugFilterViewModel();
                foreach (var strengthObj in drugDetail.DrugStrengthFilter)
                {
                    if (!string.IsNullOrEmpty(strengthObj.Name))
                    {
                        int qty = 0;
                        if (int.TryParse(strengthObj.Name.Trim().Split(' ')[0], out qty))
                            strengthObj.Quantity = qty;
                        else strengthObj.Quantity = 0;
                    }
                }
                drugDetail.DrugStrengthFilter = drugDetail.DrugStrengthFilter.OrderBy(x => x.Quantity).ToList();


                drugDetail.DrugQuantityPriceFilter = new List<DrugQuantityPriceFilterViewModel>();
                if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[5].Rows.Count; i++)
                    {
                        var row = ds.Tables[5].Rows[i];

                        for (int j = 0; j < 10; j++)
                        {
                            if (row["Quantity" + j].GetType() == typeof(System.DBNull) || string.IsNullOrEmpty(row["Quantity" + j].ToString().Trim()))
                                continue;


                            drugDetail.DrugQuantityPriceFilter.Add(new DrugQuantityPriceFilterViewModel()
                            {
                                DrugPriceID = row["DrugPriceID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugPriceID"]) : 0,
                                DrugId = row["DrugID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugID"]) : 0,
                                DrugStrengthID = row["DrugStrengthID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugStrengthID"]) : 0,
                                OrganisationId = row["OrganisationId"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["OrganisationId"]) : 0,
                                DrugTypeID = row["DrugTypeID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugTypeID"]) : 0,
                                Quantity = row["Quantity" + j].GetType() != typeof(System.DBNull) ? row["Quantity" + j].ToString() : "",
                                Price = row["Price" + j].GetType() != typeof(System.DBNull) ? row["Price" + j].ToString() : "", 
                            });
                        }
                    }
                }
                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = drugDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
                //Common.LogError(ex, "GetDrugFliterDroplist-GET");
                //return Json(new JsonResponse() { Status = 200, Message = "Data", Data = new FilterDroplist() }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet, Route("SearchDrug/{search?}/{strength?}/{type?}/{quantity?}/{manufacturer?}")]
        public ActionResult SearchDrug(String search, string strength, string type, string quantity, string manufacturer)
        {
            ViewBag.SearchBox = search;

            var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = search}
                };

            var searchRecords = new List<SearchDrugRecord>();
            var searchRecordWithPageSize = new Records();
            try
            {
                //var drugDetails = _drugService.GetAll(x => x.IsActive == true && !x.IsDeleted);

                //if (!string.IsNullOrEmpty(search))
                //{
                //    // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                //    var drug = _drugService.GetAll(x => x.IsActive == true && !x.IsDeleted && x.DrugName.ToLower().Contains(search.ToLower())).FirstOrDefault();                        
                //}

                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetDrugDetailsPageData + " '" + search + "'");

                //var drug = _drugService.GetAll(x => x.IsActive == true && !x.IsDeleted && x.DrugName.ToLower().Contains(search.ToLower())).FirstOrDefault();

                DrugDetailsViewModel drugDetail = new DrugDetailsViewModel();
                drugDetail.DrugInfo = Common.ConvertDataTable<DrugInfoViewModel>(ds.Tables[0]).FirstOrDefault();
                drugDetail.DrugTabsInfo = Common.ConvertDataTable<DrugTabsInfoViewModel>(ds.Tables[1]).ToList();
                drugDetail.DrugTypeTabletFilter = Common.ConvertDataTable<DrugTypeFilterViewModel>(ds.Tables[2]).OrderBy(x => x.Name).ToList();
                drugDetail.DrugManufacturerFilter = Common.ConvertDataTable<DrugManufacturerFilterViewModel>(ds.Tables[3]).OrderBy(x => x.CompanyName).ToList();
                drugDetail.DrugStrengthFilter = Common.ConvertDataTable<DrugStrengthFilterViewModel>(ds.Tables[4]).OrderBy(x => x.Name).ToList();
                drugDetail.DrugFilter = new DrugFilterViewModel();
                foreach (var strengthObj in drugDetail.DrugStrengthFilter)
                {
                    if (!string.IsNullOrEmpty(strengthObj.Name))
                    {
                        int qty = 0;
                        if (int.TryParse(strengthObj.Name.Trim().Split(' ')[0], out qty))
                            strengthObj.Quantity = qty;
                        else strengthObj.Quantity = 0;
                    }
                }
                drugDetail.DrugStrengthFilter = drugDetail.DrugStrengthFilter.OrderBy(x => x.Quantity).ToList();


                drugDetail.DrugQuantityPriceFilter = new List<DrugQuantityPriceFilterViewModel>();
                if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[5].Rows.Count; i++)
                    {
                        var row = ds.Tables[5].Rows[i];

                        for (int j = 0; j < 10; j++)
                        {
                            if (row["Quantity" + j].GetType() == typeof(System.DBNull) || string.IsNullOrEmpty(row["Quantity" + j].ToString().Trim()))
                                continue;

                            //drugDetail.DrugQuantityPriceFilter.Add(new DrugQuantityPriceFilterViewModel()
                            //{
                            //    DrugPriceID = row["DrugPriceID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugPriceID"]) : 0,
                            //    DrugId = row["DrugID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugID"]) : 0,
                            //    DrugStrengthID = row["DrugStrengthID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugStrengthID"]) : 0,
                            //    OrganisationId = row["OrganisationId"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["OrganisationId"]) : 0,
                            //    DrugTypeID = row["DrugTypeID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugTypeID"]) : 0,
                            //    Quantity = row["Quantity" + j].GetType() != typeof(System.DBNull) ? row["Quantity" + j].ToString() : "",
                            //    Price = row["Price" + j].GetType() != typeof(System.DBNull) ? row["Price" + j].ToString() : "",
                            //});

                            if (i == 0)
                            {
                                drugDetail.DrugQuantityPriceFilter.Add(new DrugQuantityPriceFilterViewModel()
                                {
                                    DrugPriceID = row["DrugPriceID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugPriceID"]) : 0,
                                    DrugId = row["DrugID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugID"]) : 0,
                                    DrugStrengthID = row["DrugStrengthID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugStrengthID"]) : 0,
                                    OrganisationId = row["OrganisationId"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["OrganisationId"]) : 0,
                                    DrugTypeID = row["DrugTypeID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugTypeID"]) : 0,
                                    Quantity = row["Quantity" + j].GetType() != typeof(System.DBNull) ? row["Quantity" + j].ToString() : "",
                                    Price = row["Price" + j].GetType() != typeof(System.DBNull) ? row["Price" + j].ToString() : "",
                                    IsChecked = true,
                                });
                            }
                            else
                            {
                                drugDetail.DrugQuantityPriceFilter.Add(new DrugQuantityPriceFilterViewModel()
                                {
                                    DrugPriceID = row["DrugPriceID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugPriceID"]) : 0,
                                    DrugId = row["DrugID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugID"]) : 0,
                                    DrugStrengthID = row["DrugStrengthID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugStrengthID"]) : 0,
                                    OrganisationId = row["OrganisationId"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["OrganisationId"]) : 0,
                                    DrugTypeID = row["DrugTypeID"].GetType() != typeof(System.DBNull) ? Convert.ToInt32(row["DrugTypeID"]) : 0,
                                    Quantity = row["Quantity" + j].GetType() != typeof(System.DBNull) ? row["Quantity" + j].ToString() : "",
                                    Price = row["Price" + j].GetType() != typeof(System.DBNull) ? row["Price" + j].ToString() : "",
                                    IsChecked = false,
                                });
                            }

                        }
                    }
                }


                // Searching
                bool isSearched = false;
                if (!string.IsNullOrEmpty(strength)) isSearched = true;
                if (!string.IsNullOrEmpty(type)) isSearched = true;
                if (!string.IsNullOrEmpty(quantity)) isSearched = true;
                if (!string.IsNullOrEmpty(manufacturer)) isSearched = true;
                drugDetail.DrugFilter = new DrugFilterViewModel()
                {
                    DrugName = search,
                    Price = "",
                    Strength = strength,
                    Quantity = quantity,
                    Type = type,
                    Maunfacturer = manufacturer,
                };


                if (!isSearched)
                {
                    if (drugDetail.DrugStrengthFilter != null && drugDetail.DrugStrengthFilter.Count > 0)
                    {
                        if (drugDetail.DrugStrengthFilter.FirstOrDefault() != null)
                        {
                            drugDetail.DrugFilter.DrugStrengthId = drugDetail.DrugStrengthFilter.FirstOrDefault().DrugStrengthID;
                            drugDetail.DrugFilter.Strength = drugDetail.DrugStrengthFilter.FirstOrDefault().Name;
                        }
                        else drugDetail.DrugFilter.DrugStrengthId = 0;
                    }
                    else drugDetail.DrugFilter.DrugStrengthId = 0;

                    if (drugDetail.DrugQuantityPriceFilter != null && drugDetail.DrugQuantityPriceFilter.Count > 0)
                    {
                        if (drugDetail.DrugQuantityPriceFilter.FirstOrDefault() != null)
                            drugDetail.DrugFilter.Quantity = drugDetail.DrugQuantityPriceFilter.FirstOrDefault().Quantity;
                        else drugDetail.DrugFilter.Quantity = "0";
                    }
                    else drugDetail.DrugFilter.Quantity = "0";
                }

                if (drugDetail.DrugQuantityPriceFilter != null && drugDetail.DrugQuantityPriceFilter.Count > 0)
                {
                    List<DrugQuantityPriceFilterViewModel> searchField = drugDetail.DrugQuantityPriceFilter;

                    if (!string.IsNullOrEmpty(drugDetail.DrugFilter.Strength))
                    {
                        if (drugDetail.DrugStrengthFilter != null && drugDetail.DrugStrengthFilter.Count > 0)
                        {
                            var searchedDrug = drugDetail.DrugStrengthFilter.Where(x => x.Name.Trim().ToLower() == drugDetail.DrugFilter.Strength.Trim().ToLower()).FirstOrDefault();
                            if (searchedDrug != null) drugDetail.DrugFilter.DrugStrengthId = searchedDrug.DrugStrengthID;
                            else drugDetail.DrugFilter.DrugStrengthId = 0;
                        }
                        searchField = searchField.Where(d => d.DrugStrengthID == drugDetail.DrugFilter.DrugStrengthId).ToList();
                    }

                    if (!string.IsNullOrEmpty(drugDetail.DrugFilter.Type))
                    {
                        if (drugDetail.DrugTypeTabletFilter != null && drugDetail.DrugTypeTabletFilter.Count > 0)
                        {
                            var searchedDrug = drugDetail.DrugTypeTabletFilter.Where(x => x.Name.Trim().ToLower() == drugDetail.DrugFilter.Type.Trim().ToLower()).FirstOrDefault();
                            if (searchedDrug != null) drugDetail.DrugFilter.DrugTypeId = searchedDrug.DrugTypeId;
                            else drugDetail.DrugFilter.DrugTypeId = 0;
                        }
                        searchField = searchField.Where(d => d.DrugTypeID == drugDetail.DrugFilter.DrugTypeId).ToList();
                    }

                    if (!string.IsNullOrEmpty(drugDetail.DrugFilter.Maunfacturer))
                    {
                        if (drugDetail.DrugManufacturerFilter != null && drugDetail.DrugManufacturerFilter.Count > 0)
                        {
                            var searchedDrug = drugDetail.DrugManufacturerFilter.Where(x => x.CompanyName.ToLower().Trim() == drugDetail.DrugFilter.Maunfacturer.Trim().ToLower()).FirstOrDefault();
                            if (searchedDrug != null) drugDetail.DrugFilter.ManufacturerId = searchedDrug.DrugManufacturerID;
                            else drugDetail.DrugFilter.ManufacturerId = 0;
                        }
                        searchField = searchField.Where(d => d.OrganisationId == drugDetail.DrugFilter.ManufacturerId).ToList();
                    }

                    if (!string.IsNullOrEmpty(drugDetail.DrugFilter.Quantity))
                        searchField = searchField.Where(d => d.Quantity.Trim().ToLower() == drugDetail.DrugFilter.Quantity.Trim().ToLower()).ToList();

                    if (searchField != null && searchField.Count > 0) drugDetail.DrugFilter.Price = searchField[0].Price;
                    else drugDetail.DrugFilter.Price = "";
                }

                //Set Default Tab Open
                if (drugDetail.DrugTabsInfo.Any(d => d.Name.Trim().ToLower() == "indications and usage"))
                    drugDetail.OpenDefaultTabId = drugDetail.DrugTabsInfo.Where(d => d.Name.Trim().ToLower() == "indications and usage").First().DrugTabsId;
                else if (drugDetail.DrugTabsInfo.Any(d => d.Name.Trim().ToLower() == "dosage"))
                    drugDetail.OpenDefaultTabId = drugDetail.DrugTabsInfo.Where(d => d.Name.Trim().ToLower() == "dosage").First().DrugTabsId;
                else drugDetail.OpenDefaultTabId = drugDetail.DrugTabsInfo.FirstOrDefault() != null ? drugDetail.DrugTabsInfo.FirstOrDefault().DrugTabsId : 0;

                //var searchRecord = new SearchDrugRecord
                //        {
                //            // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                //            //Dosage = drug.Dosage,
                //            //Professional = drug.Professional,
                //            //DrugDetailId = drug.DrugDetailId,
                //            //Tips = drug.Tips,
                //            //Interaction = drug.Interaction,
                //            LongDescription = drug.Description,
                //            MedicineName = drug.DrugName,
                //            ShortDescription = drug.ShortDescription,
                //            //PharmacyDetail = SpSearchRecord(searchDrugRecordsParam, drug.DrugId).ToList()
                //        };
                //        if (drug.DrugTabs != null)
                //        {
                //            searchRecord.SideEffects = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.SideEffects)?.Description;
                //            searchRecord.DosageformsandStrengths = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.DosageformsandStrengths)?.Description;
                //            searchRecord.DrugInteractions = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.DrugInteractions)?.Description;
                //            searchRecord.Symptoms = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.Symptoms)?.Description;
                //            searchRecord.IndicationandUsage = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.IndicationandUsage)?.Description;
                //            searchRecord.DosageandAdministration = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.DosageandAdministration)?.Description;
                //            searchRecord.Contradictions = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.Contradictions)?.Description;
                //        }
                //        searchRecords.Add(searchRecord);


                //if (string.IsNullOrEmpty(search))
                //{
                //    searchRecords = searchRecords.Where(item => item.PharmacyDetail.Count != 0).ToList();
                //}
                // if search parameter contains the field for search

                // return View(drugs);
                return View(drugDetail);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "SearchDrug-GET");
                return View();
            }
        }

        [HttpPost, Route("_SearchDrugs")]
        public PartialViewResult _Drugs(SearchDrugModel model)
        {
            try
            {
                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = ""},
                    new SqlParameter("@Sorting",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                };
                var drugs = new List<Drug>()
                      {new Drug() { DrugId=1,DrugName=""}  };//_doctor.GetFacilitySearchList(StoredProcedureList.SearchDrug, parameters.ToArray<object>());
                return PartialView(drugs);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "_Drugs-POST");
                return PartialView(new List<Drug>());
            }

        }
        static string GetPublicIPOfServer()
        {
            String address = "";
            JObject jObj = new JObject();
            jObj.Add("ip", "");
            WebRequest request = WebRequest.Create("https://api.ipify.org/?format=json");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                jObj = JObject.Parse(stream.ReadToEnd().ToString());
            }

            //int first = address.IndexOf("Address: ") + 9;
            //int last = address.LastIndexOf("</body>");
            //address = address.Substring(first, last - first);

            return jObj["ip"].ToString();
        }

        //Modified by AKM
        private string GetIPString(string VisitorsIPAddr)//Added by Reena
        {
            JObject jobj = new JObject();
            //string VisitorsIPAddr = string.Empty;
            //if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            //{
            //    //To get the IP address of the machine and not the proxy
            //    VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            //}
            //else if (System.Web.HttpContext.Current.Request.UserHostAddress.Length != 0)
            //{
            //    VisitorsIPAddr = System.Web.HttpContext.Current.Request.UserHostAddress;
            //}
            //else if (System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
            //{
            //    VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            try
            {
                if (VisitorsIPAddr.Split('.').Length == 4)
                {
                    //string info = new System.Net.WebClient().DownloadString("http://ipinfo.io/" + VisitorsIPAddr);
                    var ApiUrl = ConfigurationManager.AppSettings["GetIpAddressLocation"].Replace("ipaddress", VisitorsIPAddr);
                    string info = new System.Net.WebClient().DownloadString(ApiUrl);
                    if (!string.IsNullOrEmpty(info))
                    {
                        return info;
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(jobj);

                    }

                }

            }
            catch (Exception ex)
            {

            }
            return JsonConvert.SerializeObject(jobj);
        }

        /// <summary>
        /// Returns ZIP and City for the input IPAddress if exists
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> getCityZipByIP(string IPAddress)
        {
            List<Dictionary<string, object>> CityZipList = null;
            DataSet ds = null;

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@IPAddress", SqlDbType.NVarChar)  {Value = IPAddress },               

            };

            ds = _doctor.GetDataSetList(StoredProcedureList.GetIPZIPMapping , parameters);

            CityZipList = Common.ConvertToList(ds.Tables[0]);

            return CityZipList;

        }


        /// <summary>
        /// Add Mapping data for city, zip and ipaddress
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <param name="City"></param>
        /// <param name="Zip"></param>
        private void AddIPCityZip(string IPAddress, string City,string Zip,string RegionCode,string LocationJson)
        {
           var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@IPAddress", SqlDbType.NVarChar) { Value = IPAddress });
            parameters.Add(new SqlParameter("@ZipCode", SqlDbType.NVarChar) { Value = Zip });
            parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = City });
            parameters.Add(new SqlParameter("@RegionCode", SqlDbType.NVarChar) { Value = RegionCode });
            parameters.Add(new SqlParameter("@LocationJson", SqlDbType.NVarChar) { Value = LocationJson });
            try
            {
                _doctor.AddOrUpdateExecuteProcedure("prc_IPCityZip_Insert", parameters);
            }
            catch (Exception ex)
            {

            }
        }

        private List<Dictionary<string, object>> getAdds(string strParams, int IsDoc)
        {
            List<Dictionary<string, object>> AddList = null;
            DataSet ds = null;
            if (IsDoc == 1)
            {
                ds = _doctor.GetQueryResult(StoredProcedureList.GetDocAddList + " " + strParams);
            }
            else
            {
                ds = _doctor.GetQueryResult(StoredProcedureList.GetAddList + " " + strParams);
            }
            AddList = Common.ConvertToList(ds.Tables[0]);
            if (AddList != null && AddList.Count() > 0)
            {
                foreach (var dict in AddList)
                {
                    if (IsDoc == 1)
                    {
                        dict["ProfileUrl"] = "/Profile/Doctor/" + dict["OrganisationName"].ToString().Replace(" ", "-") + "-" + dict["NPI"].ToString();
                        dict["OrganizationTypeId"] = "2";
                    }
                    else
                    {
                        if (dict["OrganizationTypeId"].ToString() == "1005")
                        {
                            dict["ProfileUrl"] = "/Profile/Pharmacy/" + dict["OrganisationName"].ToString().Replace(" ", "-") + "-" + dict["NPI"].ToString();
                        }
                        else if (dict["OrganizationTypeId"].ToString() == "1006")
                        {
                            dict["ProfileUrl"] = "/Profile/Facility/" + dict["OrganisationName"].ToString().Replace(" ", "-") + "-" + dict["NPI"].ToString();
                        }
                        else if (dict["OrganizationTypeId"].ToString() == "1007")
                        {
                            dict["ProfileUrl"] = "/Profile/SeniorCare/" + dict["OrganisationName"].ToString().Replace(" ", "-") + "-" + dict["NPI"].ToString();
                        }
                        else
                        {
                            dict["ProfileUrl"] = "#";
                        }
                    }
                }

            }
            return AddList;
        }


        //[ValidateAntiForgeryToken]
        [HttpGet, Route("SearchDrug/GetAddList")]
        public JsonResult GetAddList(int AddTypeId, int OrgTypeId, string IpAddress = "", string City = "", string Zip = "")
        {
            int intStatus = 0;
            string strMsg = "";
            JObject objLoc = new JObject();
            JObject objLocServer = new JObject();
            Dictionary<string, object> returnDict = new Dictionary<string, object>();
            var AddList = new List<Dictionary<string, object>>();
            var CityZipList = new List<Dictionary<string, object>>();
            var strLoc = "";
            int IsDocProfile = OrgTypeId == 0 ? 1 : 0;
            try
            {
                
                if (!string.IsNullOrEmpty(City))
                {
                    objLoc.Add("city", City.Trim());
                    objLoc.Add("zip", Zip.Trim());
                }
                else
                {
                    if (string.IsNullOrEmpty(IpAddress))
                        IpAddress = Common.GetPublicIPOfServer();

                    CityZipList =getCityZipByIP(IpAddress);

                    if (CityZipList != null && CityZipList.Count > 0)
                    {
                        objLoc.Add("city", CityZipList[0]["City"].ToString());
                        objLoc.Add("zip", CityZipList[0]["ZipCode"].ToString());
                        objLoc.Add("region_code", CityZipList[0]["RegionCode"].ToString());
                    }
                    else
                    {                        
                       strLoc = Common.GetIPString(IpAddress);                       
                        objLoc = JObject.Parse(strLoc);

                        if (objLoc["city"] != null && objLoc["zip"] != null && objLoc["region_code"] !=null)
                            AddIPCityZip(IpAddress, objLoc["city"].ToString(), objLoc["zip"].ToString(), objLoc["region_code"].ToString(), strLoc);
                    }
                }
                string strParams = AddTypeId + "," + OrgTypeId + ",'" + (objLoc["city"] != null ? objLoc["city"].ToString() : "") + "','" + (objLoc["zip"] != null ? objLoc["zip"].ToString() : "") + "'";// + "," + PostalCode + "," + City;
                AddList = getAdds(strParams, IsDocProfile);
                if (AddList != null && AddList.Count() > 0)
                {
                    intStatus = 1;
                    strMsg = "Success";
                }
                else
                {
                    intStatus = 0;
                    strMsg = "Data not found!";
                }

                foreach (var item in AddList)
                {
                    var x = Request.Url.Scheme;
                    var upLoadPath = Request.Url.Scheme + "://" + Request.Url.Authority + "/Uploads/";
                    if (item["ImagePath"] == null || string.IsNullOrEmpty(item["ImagePath"].ToString()))
                    {
                        item["ImagePath"] = upLoadPath + "ProfilePic/no_picture.png";
                    }
                    else
                    {
                        if (!System.IO.File.Exists(Server.MapPath("~/Uploads/Advertisement/" + item["ImagePath"].ToString())))
                        {
                            item["ImagePath"] = upLoadPath + "ProfilePic/no_picture.png";
                        }
                        else
                        {
                            item["ImagePath"] = upLoadPath + "Advertisement/" + item["ImagePath"].ToString();
                        }
                    }
                }
                returnDict.Add("AdsList", AddList);
                returnDict.Add("UserLocation", JsonConvert.SerializeObject(objLoc));
            }
            catch (Exception ex)
            {
                intStatus = -1;
                strMsg = ex.Message + ":" + ex.StackTrace;
                Common.LogError(ex, "GetAddList");
            }

            return Json(new { JsonStatus = intStatus, JsonMessage = strMsg, JsonResponse = returnDict }, JsonRequestBehavior.AllowGet);
        }




        [HttpPost, Route("SearchDrug/GetDrugTabList/{DrugId?}")]
        //[ValidateAntiForgeryToken]

        public JsonResult GetDrugTabList(int DrugId)
        {
            int intStatus = 0;
            string strMsg = "";
            var drugTabList = new List<Dictionary<string, object>>();
            try
            {
                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetDrugTabList + " " + DrugId);
                //drugTabList = Common.ConvertToList(ds.Tables[0]);
                if (drugTabList != null && drugTabList.Count() > 0)
                {

                    intStatus = 1;
                    strMsg = "Success";
                }
                else
                {
                    intStatus = 0;
                    strMsg = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                intStatus = -1;
                strMsg = ex.Message;
                Common.LogError(ex, "GetDrugList");
            }

            return Json(new { JsonStatus = intStatus, JsonMessage = strMsg, JsonResponse = drugTabList }, JsonRequestBehavior.AllowGet);
        }

        [Route("home/Drug/{id?}")]
        public ActionResult DrugProfile(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index", "SeniorCare");
                }
                var drug = _facility.GetSingle(x => x.OrganisationId == id);
                return View(drug);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "DrugProfile-GET");
                return View(new Organisation());
            }
        }
        #endregion

        [HttpPost, ValidateAntiForgeryToken, Route("AddReview")]
        public JsonResult AddReview(ReviewViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var review = new Review
                    {
                        Rating = (short)model.Rating,
                        ReferenceId = model.DoctorId,
                        Description = model.Review,
                        //Title = model.Subject
                    };
                    _reviewService.InsertData(review);
                    _reviewService.SaveData();
                    txscope.Complete();
                    return Json(new JsonResponse() { Status = 1, Message = "Review added successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddReview-Post");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #region Controller Common
        private List<SelectListItem> DistanceList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Text = "5 Mile", Value = "5" },
                new SelectListItem() { Text = "10 Mile", Value = "10" },
                new SelectListItem() { Text = "15 Mile", Value = "15" },
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="searchLocationKey"></param>
        /// <returns></returns>
        private SearchViewModel BindFilter(SearchParameterModel searchFilter)
        {
            try
            {
                var listAllDocParams = GetFilterSearchParameters(searchFilter);
                var dataset = _doctor.GetDataSetList(StoredProcedureList.GetDoctorFilterSearch_v3, listAllDocParams);
                var dt = dataset.Tables[2];
                List<DrpKeyValueModel> insuranceDrpKeyValueModel = new List<DrpKeyValueModel>();
                foreach (DataRow dr in dt.Rows)
                {
                    insuranceDrpKeyValueModel.Add(new DrpKeyValueModel { Id = Convert.ToInt32(dr[0]), Name = dr[1].ToString(), Count = Convert.ToInt32(dr[2]) });
                }

                ViewBag.InsuranceList = insuranceDrpKeyValueModel.Select(x => new KeyValuePairModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Count = x.Count
                }).ToList();


                int acceptNewPatient = 0;
                int nearTermPcpAvailability = 0;
                int primaryCare = 0;
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    var patientAccepts = dataset.Tables[0].Rows[0];
                    if (patientAccepts != null)
                    {
                        acceptNewPatient = patientAccepts["AcceptingNewPatients"] == DBNull.Value ? 0 : Convert.ToInt32(patientAccepts["AcceptingNewPatients"]);
                        nearTermPcpAvailability = patientAccepts["NearTermPcpAvailability"] == DBNull.Value ? 0 : Convert.ToInt32(patientAccepts["NearTermPcpAvailability"]);
                        primaryCare = patientAccepts["PrimaryCare"] == DBNull.Value ? 0 : Convert.ToInt32(patientAccepts["PrimaryCare"]);
                    }
                }
                int pediatrics = 0;
                int teenage = 0;
                int adults = 0;
                int geriatrics = 0;
                int female = 0;
                int male = 0;

                if (dataset.Tables[1].Rows.Count > 0)
                {
                    var agegroup = dataset.Tables[1].Rows[0];
                    if (agegroup != null)
                    {
                        pediatrics = Convert.ToInt32(agegroup["Pediatrics"]);
                        teenage = Convert.ToInt32(agegroup["Teenage"]);
                        adults = Convert.ToInt32(agegroup["Adults"]);
                        geriatrics = Convert.ToInt32(agegroup["Geriatrics"]);
                        female = Convert.ToInt32(agegroup["Female"]);
                        male = Convert.ToInt32(agegroup["Male"]);
                    }
                }
                else
                {
                    pediatrics = 0;
                    teenage = 0;
                    adults = 0;
                    geriatrics = 0;
                    female = 0;
                    male = 0;
                }

                AgeGroupsSeen agegrp = new AgeGroupsSeen() { Pediatrics = pediatrics, Adults = adults, Geriatrics = geriatrics, Female = female, Male = male, Teenagers = teenage };
                var model = new SearchViewModel
                {
                    AcceptingNewPatients = acceptNewPatient,
                    NearTermPcpAvailability = nearTermPcpAvailability,
                    PrimaryCare = primaryCare,
                    AgeGroupsSeen = agegrp,
                    IpInfo = _ipInfo
                };
                List<KeyValuePairModel> lstAgeGroup = new List<KeyValuePairModel>();
                var dtAgeGroup = dataset.Tables[3];
                foreach (DataRow dr in dtAgeGroup.Rows)
                {
                    lstAgeGroup.Add(new KeyValuePairModel { Id = dr[0].ToString(), Name = dr[1].ToString(), Count = Convert.ToInt32(dr[2]) });
                }

                ViewBag.AgegroupList = lstAgeGroup;
                List<KeyValuePairModel> lstGenders = new List<KeyValuePairModel>();
                var dtGenders = dataset.Tables[4];
                foreach (DataRow dr in dtGenders.Rows)
                {
                    lstGenders.Add(new KeyValuePairModel { Id = dr[0].ToString(), Name = dr[1].ToString(), Count = Convert.ToInt32(dr[2]) });
                }

                ViewBag.AgegroupList = lstAgeGroup;
                ViewBag.GenderType = lstGenders;
                ViewBag.LanguageList = _language.GeLanguageDropDownList(SqlQuery.GeLanguageDropDownList, new object[] { }).Select(x => new SelectListItem
                {
                    Text = x.LanguageName,
                    Value = x.LanguageId.ToString()
                }).ToList();


                return model;
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "BindDropdown-Search");
                return new SearchViewModel();
            }
        }
        private SearchViewModel SearchBindFilter(SearchParameterModel searchFilter)
        {
            try
            {

                var listAllDocParams = GetFilterSearchParameters(searchFilter);
                var dataset = _doctor.GetDataSetList(StoredProcedureList.GetDoctorFilterSearch_v3, listAllDocParams);
                var dt = dataset.Tables[2];
                List<DrpKeyValueModel> insuranceDrpKeyValueModel = new List<DrpKeyValueModel>();
                foreach (DataRow dr in dt.Rows)
                {
                    insuranceDrpKeyValueModel.Add(new DrpKeyValueModel { Id = Convert.ToInt32(dr[0]), Name = dr[1].ToString(), Count = Convert.ToInt32(dr[2]) });
                }

                var insuranceList = insuranceDrpKeyValueModel.Select(x => new KeyValuePairModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Count = x.Count
                }).ToList();


                int acceptNewPatient = 0;
                int nearTermPcpAvailability = 0;
                int primaryCare = 0;
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    var patientAccepts = dataset.Tables[0].Rows[0];
                    if (patientAccepts != null)
                    {
                        acceptNewPatient = patientAccepts["AcceptingNewPatients"] == DBNull.Value ? 0 : Convert.ToInt32(patientAccepts["AcceptingNewPatients"]);
                        nearTermPcpAvailability = patientAccepts["NearTermPcpAvailability"] == DBNull.Value ? 0 : Convert.ToInt32(patientAccepts["NearTermPcpAvailability"]);
                        primaryCare = patientAccepts["PrimaryCare"] == DBNull.Value ? 0 : Convert.ToInt32(patientAccepts["PrimaryCare"]);
                    }
                }
                int pediatrics = 0;
                int teenage = 0;
                int adults = 0;
                int geriatrics = 0;
                int female = 0;
                int male = 0;

                if (dataset.Tables[1].Rows.Count > 0)
                {
                    var agegroup = dataset.Tables[1].Rows[0];
                    if (agegroup != null)
                    {
                        pediatrics = Convert.ToInt32(agegroup["Pediatrics"]);
                        teenage = Convert.ToInt32(agegroup["Teenage"]);
                        adults = Convert.ToInt32(agegroup["Adults"]);
                        geriatrics = Convert.ToInt32(agegroup["Geriatrics"]);
                        female = Convert.ToInt32(agegroup["Female"]);
                        male = Convert.ToInt32(agegroup["Male"]);
                    }
                }
                else
                {
                    pediatrics = 0; teenage = 0; adults = 0;
                    geriatrics = 0; female = 0; male = 0;
                }

                AgeGroupsSeen agegrp = new AgeGroupsSeen() { Pediatrics = pediatrics, Adults = adults, Geriatrics = geriatrics, Female = female, Male = male, Teenagers = teenage };
                var model = new SearchViewModel
                {
                    AcceptingNewPatients = acceptNewPatient,
                    NearTermPcpAvailability = nearTermPcpAvailability,
                    PrimaryCare = primaryCare,
                    AgeGroupsSeen = agegrp,
                    IpInfo = _ipInfo,
                    Insurance = insuranceList
                };

                return model;
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "BindDropdown-Search");
                return new SearchViewModel();
            }
        }
        private void BindDropdowns()
        {
            try
            {
                ViewBag.DistanceList = DistanceList();
                ViewBag.AffiliationList = _facility.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new KeyValuePairModel
                {
                    Id = x.OrganisationId.ToString(),
                    Name = x.OrganisationName,
                    Count = x.DoctorAffiliations.Count(y => y.IsActive && !y.IsDeleted)
                }).ToList();

                //var alldata = _doctorInsuranceAccepted.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
                var insData = _doctorInsuranceService.GetDrpInsuranceList(StoredProcedureList.GetDrpInsuranceList, new object[] { }).ToList();
                ViewBag.InsuranceList = insData.Select(x => new KeyValuePairModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Count = x.Count
                }).ToList();

                int a = 0;
                //ViewBag.InsuranceList = _insuranceTypeService.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new KeyValuePairModel
                //{
                //    Id = x.InsuranceTypeId.ToString(),
                //    Name = x.Name,
                //    Count = x.InsurancePlans.Where(y => y.IsActive && !y.IsDeleted).Aggregate(0, (l, m) => l += m.DocOrgInsurances.Count())
                //}).ToList();

                ViewBag.AgegroupList = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new KeyValuePairModel
                {
                    Id = x.AgeGroupId.ToString(),
                    Name = x.Name,
                    Count = x.DoctorAgeGroups.Count(y => y.IsActive && !y.IsDeleted)
                }).ToList();

                ViewBag.GenderType = _genderservice.GetAll().Select(x => new KeyValuePairModel
                {
                    Id = x.GenderTypeId.ToString(),
                    Name = x.Name,
                    Count = x.DoctorGender.Count()
                }).ToList();

                ViewBag.SpecialityList = _speciality.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                {
                    Text = x.SpecialityName,
                    Value = x.SpecialityId.ToString()
                }).ToList();

                ViewBag.LanguageList = _language.GeLanguageDropDownList(SqlQuery.GeLanguageDropDownList, new object[] { }).Select(x => new SelectListItem
                {
                    Text = x.LanguageName,
                    Value = x.LanguageId.ToString()
                }).ToList();

                //ViewBag.LanguageList = _language.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.LanguageName,
                //    Value = x.LanguageId.ToString()
                //}).ToList();
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "BindDropdown-Search");
            }
        }

        private bool IsValidType(string type)
        {
            type = type.ToUpper();
            switch (type)
            {
                case "DOCTOR":
                    return true;
                case "FACILITY":
                    return true;
                case "PATIENT":
                    return true;
                case "PHARMACY":
                    return true;
                case "SENIORCARE":
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Coomon Method

        #region Doctor 

        public void GetDoctorList(SearchParameterModel model, string textSearch)
        {
            if (!string.IsNullOrEmpty(textSearch))
            {

            }
        }

        #endregion

        #endregion

        #region SearchDrug

        [HttpGet, Route("SearchDrug/GetDrugFliterDroplist")]
        public JsonResult GetDrugFliterDroplist()
        {
            try
            {
                // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                var filterDroplist = new FilterDroplist();
                //filterDroplist.Tablet = _tabletService.GetAll(x => x.IsActive == true && !x.IsDeleted).ToList();
                //filterDroplist.DrugType = _drugTypeService.GetAll(x => x.IsActive == true && !x.IsDeleted).ToList();
                //filterDroplist.DrugQuantity = _drugQuantityService.GetAll(x => x.IsActive == true && !x.IsDeleted).ToList();
                //filterDroplist.DrugManufacturer = _drugManufacturerService.GetAll(x => x.IsActive == true && !x.IsDeleted).ToList();
                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = filterDroplist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetDrugFliterDroplist-GET");
                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = new FilterDroplist() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, Route("SearchDrug/GetDrugDetails")]
        public JsonResult GetDrugDetails(SearchDrugRecordsParam searchDrugRecordsParam)
        {
            var searchRecords = new List<SearchDrugRecord>();
            var searchRecordWithPageSize = new Records();
            try
            {
                var drugDetails = _drugService.GetAll(x => x.IsActive == true && !x.IsDeleted);

                if (!string.IsNullOrEmpty(searchDrugRecordsParam.MedicineName))
                {
                    // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                    drugDetails = _drugService.GetAll(x => x.IsActive == true && !x.IsDeleted && x.DrugName.ToLower().Contains(searchDrugRecordsParam.MedicineName.ToLower()));
                }

                if (!string.IsNullOrEmpty(searchDrugRecordsParam.StartWithAlphabetically))
                {
                    // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                    drugDetails = _drugService.GetAll(x => x.IsActive == true && !x.IsDeleted && x.DrugName.StartsWith(searchDrugRecordsParam.StartWithAlphabetically));
                }

                int skip = 0;
                int totalItemCount = drugDetails.Count();
                int pageSize = searchDrugRecordsParam.PageSize;
                int pageCount = System.Convert.ToInt32(System.Math.Ceiling(totalItemCount / System.Convert.ToDouble(pageSize)));

                if (searchDrugRecordsParam.PageNumber != 1)
                {
                    skip = (searchDrugRecordsParam.PageNumber - 1) * searchDrugRecordsParam.PageSize;
                }
                drugDetails = drugDetails.Skip(skip).Take(searchDrugRecordsParam.PageSize).ToList();

                foreach (var drug in drugDetails)
                {
                    var searchRecord = new SearchDrugRecord
                    {
                        // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                        //Dosage = drug.Dosage,
                        //Professional = drug.Professional,
                        //DrugDetailId = drug.DrugDetailId,
                        //Tips = drug.Tips,
                        //Interaction = drug.Interaction,
                        LongDescription = drug.Description,
                        MedicineName = drug.DrugName,
                        ShortDescription = drug.ShortDescription,
                        PharmacyDetail = SpSearchRecord(searchDrugRecordsParam, drug.DrugId).ToList()
                    };
                    if (drug.DrugTabs != null)
                    {
                        searchRecord.SideEffects = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.SideEffects)?.Description;
                        searchRecord.DosageformsandStrengths = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.DosageformsandStrengths)?.Description;
                        searchRecord.DrugInteractions = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.DrugInteractions)?.Description;
                        searchRecord.Symptoms = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.Symptoms)?.Description;
                        searchRecord.IndicationandUsage = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.IndicationandUsage)?.Description;
                        searchRecord.DosageandAdministration = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.DosageandAdministration)?.Description;
                        searchRecord.Contradictions = drug.DrugTabs.FirstOrDefault(x => x.DrugTabs_LookUpID == UserTabsLookup.Contradictions)?.Description;
                    }
                    searchRecords.Add(searchRecord);
                }

                if (string.IsNullOrEmpty(searchDrugRecordsParam.MedicineName))
                {
                    searchRecords = searchRecords.Where(item => item.PharmacyDetail.Count != 0).ToList();
                }
                // if search parameter contains the field for search

                searchRecordWithPageSize.Total = pageCount;
                searchRecordWithPageSize.SearchDrugRecord = searchRecords;

                return Json(new JsonResponse() { Status = 200, Message = pageCount.ToString(), Data = searchRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetDrugDetails-POST");
                return Json(new JsonResponse() { Status = 200, Message = "0", Data = new List<SearchDrugRecord>() }, JsonRequestBehavior.AllowGet);
            }
        }

        private IList<SpSearchDrugViewModel> SpSearchRecord(SearchDrugRecordsParam searchDrugRecordsParam, int drugDetailId)
        {
            IList<SpSearchDrugViewModel> searchDrugRecords = new List<SpSearchDrugViewModel>();
            try
            {
                searchDrugRecords = _drugService.SearchDrug(StoredProcedureList.SearchDrug, drugDetailId).ToList();
                if (searchDrugRecordsParam.DrugManufacturerId != null && searchDrugRecordsParam.DrugManufacturerId.Count > 0)
                {
                    searchDrugRecords = searchDrugRecords.Where(item => searchDrugRecordsParam.DrugManufacturerId.Contains(item.DrugManufacturerId)).ToList();
                }
                if (searchDrugRecordsParam.DrugQuantityId != null && searchDrugRecordsParam.DrugQuantityId.Count > 0)
                {
                    searchDrugRecords = searchDrugRecords.Where(item => searchDrugRecordsParam.DrugQuantityId.Contains(item.DrugQuantityId)).ToList();
                }
                if (searchDrugRecordsParam.DrugTypeId != null && searchDrugRecordsParam.DrugTypeId.Count > 0)
                {
                    searchDrugRecords = searchDrugRecords.Where(item => searchDrugRecordsParam.DrugTypeId.Contains(item.DrugTypeId)).ToList();
                }
                if (searchDrugRecordsParam.TabletId != null && searchDrugRecordsParam.TabletId.Count > 0)
                {
                    searchDrugRecords = searchDrugRecords.Where(item => searchDrugRecordsParam.TabletId.Contains(item.TabletId)).ToList();
                }

                if (searchDrugRecords.Count > 0)
                {
                    searchDrugRecords = searchDrugRecords.AsEnumerable().DistinctBy(x => x.PharmacyName).ToList();
                }
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "SpSearchRecord");
            }
            return searchDrugRecords;
        }

        [HttpGet, Route("SearchDrug/MostlySearchedDrug")]
        public JsonResult MostlySearchedDrug()
        {
            string text = "{ 'MostlySearchedDrug': '";
            DataSet mostlySearchedMedicines = _doctor.GetQueryResult(StoredProcedureList.GetMostlySearchedMedicines);
            if (mostlySearchedMedicines != null && mostlySearchedMedicines.Tables.Count > 0)
            {
                DataTable dt = mostlySearchedMedicines.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    text += row.Field<string>("Description") + ",";
                }
            }

            text = text.TrimEnd(',');
            text += "' }";
            //string text = System.IO.File.ReadAllText(Server.MapPath(StaticFilePath.WebSettings)).NullToString();
            var jsonNetworkCount = JsonConvert.DeserializeObject<NetworkCount>(text);

            return Json(new JsonResponse() { Status = 200, Message = "Data", Data = jsonNetworkCount }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost, Route("GetTimeShlots/")]
        public JsonResult GetTimeShlots(TimeSlotRequest request)
        {
            try
            {
                int skip = 0;
                var records = new DateSlotRecords();
                var date = DateTime.Now;
                var result = new List<DateSlotModel>();
                DateSlotModel tempDateSlot = null;
                var timeSlotList = new List<TimeSlotModel>();
                var dateRange = DateTime.Now.AddDays(30);
                var slotLists = _slot.GetAll(x => x.IsActive && !x.IsDeleted && Convert.ToDateTime(x.SlotDate) >= date.Date && Convert.ToDateTime(x.SlotDate) <= dateRange);

                if (request.DoctorId > 0)
                {
                    slotLists = slotLists.Where(x => x.ReferenceId == request.DoctorId).OrderBy(d => d.SlotDate);
                    var doctor = _doctor.GetById(request.DoctorId);
                    var doctorDetail = new DoctorDetail { Address = new List<AddressViewModel>(), DoctorUser = new ApplicationUser() };
                    doctorDetail.DoctorUser.FirstName = doctor.DoctorUser.FirstName;
                    doctorDetail.DoctorUser.ProfilePicture = doctor.DoctorUser.ProfilePicture;
                    doctorDetail.DoctorUser.LastName = doctor.DoctorUser.LastName;
                    doctorDetail.DoctorUser.PhoneNumber = doctor.DoctorUser.PhoneNumber;
                    var addressRecords = _address.GetAll().Where(x => x.ReferenceId == request.DoctorId).ToList();
                    if (addressRecords.Count > 0)
                    {
                        addressRecords.ForEach(addressRecord =>
                        {
                            var address = new AddressViewModel
                            {
                                Address1 = addressRecord.Address1,
                                Address2 = addressRecord.Address2,
                                CityId = addressRecord.CityId,
                                StateId = addressRecord.StateId
                            };
                            doctorDetail.Address.Add(address);
                        });
                    }
                    //doctorDetail.Address = new List<Address> { address };
                    records.Doctor = doctorDetail;
                }
                else if (request.FacilityId > 0)
                {
                    slotLists = slotLists.Where(x => x.ReferenceId == request.FacilityId).OrderBy(d => d.SlotDate);
                    var facility = _facility.GetById(request.FacilityId);
                    var facilityDetail = new FacilityDetail { Address = new List<AddressViewModel>(), FacilityUser = new ApplicationUser() };
                    facilityDetail.FacilityUser.FirstName = facility.OrganisationUser.FirstName;
                    facilityDetail.FacilityUser.ProfilePicture = facility.OrganisationUser.ProfilePicture;
                    facilityDetail.FacilityUser.LastName = facility.OrganisationUser.LastName;
                    facilityDetail.FacilityUser.PhoneNumber = facility.OrganisationUser.PhoneNumber;
                    //facilityDetail.Address = _address.GetAll().Where(x => x.FacilityId == request.FacilityId).ToList();

                    var addressRecords = _address.GetAll().Where(x => x.ReferenceId == request.FacilityId).ToList();
                    if (addressRecords.Count > 0)
                    {
                        addressRecords.ForEach(addressRecord =>
                        {
                            var address = new AddressViewModel
                            {
                                Address1 = addressRecord.Address1,
                                Address2 = addressRecord.Address2,
                                CityId = addressRecord.CityId,
                                StateId = addressRecord.StateId
                            };
                            facilityDetail.Address.Add(address);
                        });
                    }
                    records.Facility = facilityDetail;
                }
                else if (request.SeniorCareId > 0)
                {
                    slotLists = slotLists.Where(x => x.ReferenceId == request.SeniorCareId).OrderBy(d => d.SlotDate);
                    var seniorCare = _seniorCare.GetById(request.SeniorCareId);
                    var seniorCareDetail = new SeniorCareDetail { Address = new List<AddressViewModel>(), SeniorCareUser = new ApplicationUser() };
                    seniorCareDetail.SeniorCareUser.FirstName = seniorCareDetail.SeniorCareUser.FirstName;
                    seniorCareDetail.SeniorCareUser.ProfilePicture = seniorCareDetail.SeniorCareUser.ProfilePicture;
                    seniorCareDetail.SeniorCareUser.LastName = seniorCareDetail.SeniorCareUser.LastName;
                    seniorCareDetail.SeniorCareUser.PhoneNumber = seniorCareDetail.SeniorCareUser.PhoneNumber;
                    //seniorCareDetail.Address = _address.GetAll().Where(x => x.SeniorCareId == request.SeniorCareId).ToList();

                    var addressRecords = _address.GetAll().Where(x => x.ReferenceId == request.SeniorCareId).ToList();
                    if (addressRecords.Count > 0)
                    {
                        addressRecords.ForEach(addressRecord =>
                        {
                            var address = new AddressViewModel
                            {
                                Address1 = addressRecord.Address1,
                                Address2 = addressRecord.Address2,
                                CityId = addressRecord.CityId,
                                StateId = addressRecord.StateId
                            };
                            seniorCareDetail.Address.Add(address);
                        });
                    }
                    records.SeniorCare = seniorCareDetail;
                }

                var groupSlotList = slotLists.GroupBy(y => y.SlotDate);
                records.Total = groupSlotList.Count();

                if (groupSlotList != null && groupSlotList.Count() > 0)
                {
                    records.Total = (records.Total + request.PageSize - 1) / request.PageSize;
                }

                if (request.PageNumber != 1)
                {
                    skip = (request.PageNumber - 1) * request.PageSize;
                }
                groupSlotList = groupSlotList.Skip(skip).Take(request.PageSize).ToList();

                foreach (var slot in groupSlotList)
                {
                    tempDateSlot = new DateSlotModel
                    {
                        Date = Convert.ToDateTime(slot.Key).ToString("MM'/'dd'/'yyyy"),
                        TimeSlots = new List<TimeSlotModel>(),
                        DayOfWeek = Convert.ToDateTime(slot.Key).DayOfWeek.ToString()
                    };

                    foreach (var time in slot)
                    {
                        tempDateSlot.TimeSlots.Add(new TimeSlotModel
                        {
                            Id = time.SlotId,
                            Time = Convert.ToDateTime(time.SlotTime).ToString("hh:mm tt"),
                            IsBook = time.IsBooked
                        });
                    }

                    result.Add(tempDateSlot);
                }

                records.DateSlotModel = new List<object>(result);

                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = records }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetTimeShlots-POST");
                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = new DateSlotRecords() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet, Route("GetTimeSlotBySlotId/{slotId}")]
        public JsonResult GetTimeSlotBySlotId(int slotId)
        {
            var address = new Address();
            try
            {
                var slotList = _slot.GetSingle(x => x.SlotId == slotId && x.IsActive);
                var user = _userManager.FindByIdAsync(User.Identity.GetUserId<int>());
                //var stateList = _state.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.StateName,
                //    Value = x.StateId.ToString()
                //}).ToList();

                if (user != null && user.Result != null)
                {
                    //if (user?.Result.Doctors != null && user.Result.Doctors.Count > 0)
                    if (user.Result.UserType.UserTypeName == "Doctor" && user.Result.Organisations != null && user.Result.Organisations.Count > 0)
                    {
                        address = _address.GetAll().Where(x => x.ReferenceId == user.Result.Organisations.FirstOrDefault().OrganisationId).FirstOrDefault();
                        //address = _address.GetAll().Where(x => x.ReferenceId == user.Result.Doctors.FirstOrDefault().DoctorId).FirstOrDefault();
                    }
                    else if (user.Result.UserType.UserTypeName == "User" && user.Result.Organisations != null && user.Result.Organisations.Count > 0)
                    {
                        address = _address.GetAll().Where(x => x.ReferenceId == user.Result.Organisations.FirstOrDefault().OrganisationId).FirstOrDefault();
                    }
                    else if (user.Result.UserType.UserTypeName == "Pharmacy" && user.Result.Organisations != null && user.Result.Organisations.Count > 0)
                    {
                        address = _address.GetAll().Where(x => x.ReferenceId == user.Result.Organisations.FirstOrDefault().OrganisationId).FirstOrDefault();
                    }
                    else if (user.Result.UserType.UserTypeName == "Facility" && user.Result.Organisations != null && user.Result.Organisations.Count > 0)
                    {
                        address = _address.GetAll().Where(x => x.ReferenceId == user.Result.Organisations.FirstOrDefault().OrganisationId).FirstOrDefault();
                    }
                    else if (user.Result.UserType.UserTypeName == "Senior Care" && user.Result.Organisations != null && user.Result.Organisations.Count > 0)
                    {
                        address = _address.GetAll().Where(x => x.ReferenceId == user.Result.Organisations.FirstOrDefault().OrganisationId).FirstOrDefault();
                    }
                }

                var timeSlot = new
                {
                    SlotId = slotList.SlotId,
                    SlotDate = Convert.ToDateTime(slotList.SlotDate).ToString("MM'/'dd'/'yyyy"),
                    SlotTime = Convert.ToDateTime(slotList.SlotTime).ToString("hh:mm tt"),
                    DoctorId = slotList.ReferenceId,
                    SeniorCareId = slotList.ReferenceId,
                    FacilityId = slotList.ReferenceId,
                    FirstName = user?.Result?.FirstName,
                    LastName = user?.Result?.LastName,
                    Prefix = user?.Result?.Prefix,
                    Suffix = user?.Result?.Suffix,
                    DateOfBirth = user?.Result?.DateOfBirth,
                    PhoneNumber = user?.Result?.PhoneNumber,
                    Email = user?.Result?.Email,
                    //States = stateList,
                    Address = address?.Address1,
                    StateId = address?.StateId,
                    CityId = address?.CityId,
                    Zipcode = address?.ZipCode,
                    Id = user?.Result?.Id
                };

                return Json(new JsonResponse() { Status = 200, Message = "", Data = timeSlot }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetTimeSlotBySlotId-GET");
                return Json(new JsonResponse() { Status = 200, Message = "Data", Data = null }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, Route("BookTimeSlot/")]
        public JsonResult BookTimeSlot(appSlotViewModel slotBook)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var slotDetails = _slot.GetSingle(x => x.SlotId == slotBook.SlotId);

                    slotDetails.IsBooked = true;
                    slotDetails.ReferenceId = slotBook.PatientUserId > 0 ? slotBook.PatientUserId : 0;

                    _slot.UpdateData(slotDetails);
                    _slot.SaveData();
                    txscope.Complete();
                    txscope.Dispose();
                    return Json(new JsonResponse() { Status = 200, Message = "Appointment is booked successfully", Data = "A" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "BookTimeSlot-POST");
                    return Json(new JsonResponse() { Status = 200, Message = "Exception", Data = "A" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //[HttpGet, Route("GetApplicationUserByEmail/")]
        //public JsonResult GetApplicationUserByEmail(string email)
        //{
        //    try
        //    {
        //        var user = _userManager.FindByIdAsync(User.Identity.GetUserId<int>());

        //        if (user != null)
        //        {
        //            return Json(new JsonResponse() { Status = 200, Message = "", Data = user?.Result }, JsonRequestBehavior.AllowGet);
        //        }

        //        return Json(new JsonResponse() { Status = 200, Message = "User does not exist.", Data = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet, Route("GetStateCity/{stateId}")]
        public JsonResult GetStateCity(int stateId)
        {
            try
            {
                //var cityList = _city.GetAll(x => x.StateId == stateId && x.IsActive && (x.IsDeleted == false)).Select(x => new SelectListItem
                //{
                //    Text = x.CityName,
                //    Value = x.CityId.ToString()
                //}).ToList();

                //return Json(new JsonResponse() { Status = 200, Message = "User does not exist.", Data = cityList }, JsonRequestBehavior.AllowGet);
                return Json(new JsonResponse() { Status = 200, Message = "User does not exist.", Data = null }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetStateCity-GET");
                return Json(new JsonResponse() { Status = 200, Message = "Exception", Data = null }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, Route("SaveApplicationUser/")]
        public JsonResult SaveApplicationUser(UserAndAddress userAndAddress)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var patientId = 0;
                    ApplicationUser applicationUser = new ApplicationUser();
                    Address address = new Address();
                    var applicationUserId = 0;
                    Patient patient = new Patient();
                    var isExist = _userManager.FindByEmailAsync(userAndAddress.Email);

                    if (isExist == null || isExist.Result == null)
                    {
                        applicationUser.CreatedDate = DateTime.Now;
                        applicationUser.LastResetPassword = DateTime.Now;
                        applicationUser.IsActive = true;
                        applicationUser.UserName = userAndAddress.Email;
                        applicationUser.DateOfBirth = userAndAddress.DateOfBirth;
                        applicationUser.Email = userAndAddress.Email;
                        applicationUser.FirstName = userAndAddress.FirstName;
                        applicationUser.LastName = userAndAddress.LastName;
                        applicationUser.PhoneNumber = userAndAddress.PhoneNumber;

                        var result = _userManager.CreateAsync(applicationUser, "Patient@123");
                        if (!result.Result.Succeeded)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        var createUser = _userManager.FindByEmail(userAndAddress.Email);
                        if (createUser != null && createUser.Id != 0)
                        {
                            applicationUserId = createUser.Id;
                            patient.UserId = createUser.Id;
                            patient.IsActive = true;
                            _patient.InsertData(patient);
                            _patient.SaveData();

                            patientId = Convert.ToInt32(_patient.GetSingle(x => x.UserId == applicationUserId)?.PatientId);

                            if (patientId > 0)
                            {
                                address.ReferenceId = patientId;
                                address.Address1 = userAndAddress.Address;
                                address.CityId = userAndAddress.CityId;
                                address.StateId = userAndAddress.StateId;
                                address.ZipCode = userAndAddress.Zipcode;
                                _address.InsertData(address);
                                _address.SaveData();
                            }
                        }

                        txscope.Complete();
                        string code = _userManager.GenerateEmailConfirmationTokenAsync(createUser.Id).Result;
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = createUser.Id, code = code }, protocol: Request.Url.Scheme);
                        string body = Common.ReadEmailTemplate(EmailTemplate.ConfirmEmail);
                        body = body.Replace("{UserName}", createUser.FullName)
                            .Replace("{action_url}", callbackUrl);
                        SendMail.SendEmail(createUser.Email, "", "", "", body, "Confirm your account");
                    }
                    else
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse() { Status = 0, Message = "Email already exist.", Data = "" }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new JsonResponse() { Status = 200, Message = "Data Saved Successfully", Data = patientId }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "Patent-Book-POST");
                    return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #region Organization
        [Route("Profile/Doctor/{doctorName?}-{Npi}")]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Organization(string doctorName, string npi = "1588667638")
        {
            //string npi = doctNamenpi.Split('-')[1].ToString();
            Doctyme.Model.ViewModels.OrganizationViewModel result = new OrganizationViewModel();
            try
            {
                

                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetDoctorProfileData + " " + npi);
                var doctorProfile = Common.ConvertDataTable<OrganizationViewModel>(ds.Tables[0]).Where(x=>x.UserTypeId == UserTypes.Doctor).FirstOrDefault();
                doctorProfile.Reviews = Common.ConvertDataTable<Reviews>(ds.Tables[8]).ToList();
                doctorProfile.Qualifications = Common.ConvertDataTable<Qualification>(ds.Tables[1]).ToList();
                doctorProfile.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[2]).ToList();
                doctorProfile.Experiences = Common.ConvertDataTable<Experiences>(ds.Tables[3]).ToList();
                doctorProfile.DoctorBoardCertifications = Common.ConvertDataTable<Certifications>(ds.Tables[4]).ToList();
                doctorProfile.DoctorLanguages = Common.ConvertDataTable<Languages>(ds.Tables[5]).ToList();
                doctorProfile.SocialMediaLinks = Common.ConvertDataTable<SocialMedia>(ds.Tables[6]).ToList();
                doctorProfile.DoctorImages = Common.ConvertDataTable<SiteImage>(ds.Tables[7]).ToList();
                doctorProfile.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[9]).ToList();
                doctorProfile.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[10]).ToList();
                doctorProfile.lstOrgAddress = Common.ConvertDataTable<OrgAddress>(ds.Tables[12]).ToList();
                doctorProfile.lstDoctorAffiliations = Common.ConvertDataTable<DoctorAffiliations>(ds.Tables[13]).ToList();
                doctorProfile.lstDoctorAdvertisements = Common.ConvertDataTable<DoctorAdvertisements>(ds.Tables[14]).ToList();
                doctorProfile.lstOrgAmenityOptions = Common.ConvertDataTable<OrgAmenityOptions>(ds.Tables[15]).ToList();
                doctorProfile.lstInsuranceAccepted = Common.ConvertDataTable<InsuranceAccepted>(ds.Tables[16]).ToList();
                doctorProfile.Maxslots = doctorProfile.lstslotTimes.Count > 0 ? Convert.ToInt32(doctorProfile.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
                doctorProfile.MaxDays = 1;
                doctorProfile.CalenderDatesCount = doctorProfile.lstslotsDates.Count > 0 ? doctorProfile.lstslotsDates.FirstOrDefault().MaxCount : 0;
                //doctorProfile.CalenderDatesCount = 7;
                doctorProfile.OrganisationId = doctorProfile.lstOrgAddress.Select(i => i.OrganisationId).FirstOrDefault();
                doctorProfile.NPI = npi;
                doctorProfile.ReturUrl = "Profile/Doctor/" + doctorProfile.FullForDoctor + "-" + doctorProfile.NPI;
                doctorProfile.SlotFor = "Doctor";
                if (ConfigurationManager.AppSettings["AgoraKey"] == null)
                {
                    ViewBag.AgoraKey = "0f8f282b530c4232b289867cda582737";
                }
                else
                {
                    ViewBag.AgoraKey = ConfigurationManager.AppSettings["AgoraKey"].ToString();
                }
                return View(doctorProfile);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Organization-GET");
                return View(new OrganizationViewModel());
            }
        }
        [HttpGet]
        [Route("GetSlots/")]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public JsonResult GetSlots(int MaxDays, int DoctorId, int OrganisationId)
        {
            OrganizationViewModel result = new OrganizationViewModel();
            try
            {

                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetTimeSlots + " " + DoctorId + ", " + MaxDays + "," + OrganisationId);
                result.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[0]).ToList();
                result.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[1]).ToList();
                result.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[2]).ToList();
                result.Maxslots = result.lstslotTimes.Count > 0 ? Convert.ToInt32(result.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
                result.MaxDays = MaxDays;
                result.CalenderDatesCount = result.lstslotsDates.Count > 0 ? result.lstslotsDates.FirstOrDefault().MaxCount : 0;
                //result.CalenderDatesCount = 7;
                string opeHoursFial = "";
                if (result.OpeningHours.Count > 0)
                {
                    DateTime dateValue = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                    int weekNumber = (int)dateValue.DayOfWeek;
                    var resultweek = result.OpeningHours.Where(i => i.WeekDay == weekNumber + 1).FirstOrDefault();
                    result.OpeningDay = dateValue.ToString("ddd");
                    if (resultweek != null)
                    {
                        DateTime startDateTIme = DateTime.Parse(resultweek.StartDateTime);
                        DateTime endDateTIme = DateTime.Parse(resultweek.EndDateTime);
                        string strMinFormat = startDateTIme.ToString("hh:mm tt");
                        string endMinFormat = endDateTIme.ToString("hh:mm tt");

                        result.OpeningTime = strMinFormat + " -" + endMinFormat;
                        opeHoursFial = dateValue.ToString("ddd") + "  " + resultweek.StartDateTime + " -" + resultweek.EndDateTime;
                    }
                }

                return Json(new JsonResponse() { Status = 200, Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetSlots-GET");
                return Json(new JsonResponse() { Status = 0, Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Route("rSlotConfirm/")]
        public JsonResult RedirectSlotCofirm(Doctyme.Model.ViewModels.SlotConfirmation _model)
        {
            Session["SlotConfirmation"] = _model;
            //return RedirectToAction("SlotsConfirmation", "Profile");
            return Json(new JsonResponse() { Status = 200, Data = "Sucess" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Search/GetSlotTimes")]
        public JsonResult GetSlotTimes(int DoctorId, string startDate, int usertypeId)
        {
            OrganizationViewModel result = new OrganizationViewModel();
            List<string> timeslot = new List<string>();
            try
            {
                string spName = "EXEC GetTimeSlotsbyDocId ";
                if (usertypeId != 2)
                {
                    spName = "EXEC GetTimeSlotsbyOrgId ";
                }
                DataSet ds = _doctor.GetQueryResult(spName + " " + DoctorId + ", " + 1 + ", '" + startDate + "'");
                result.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[0]).ToList();
                result.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[1]).ToList();
                //result.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[2]).ToList();
                //result.Maxslots = result.lstslotTimes.Count > 0 ? Convert.ToInt32(result.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
                //result.CalenderDatesCount = result.lstslotsDates.Count > 0 ? result.lstslotsDates.FirstOrDefault().MaxCount : 0;
                if (result.lstslotTimes.Count() == 0)
                {
                    if (usertypeId == 2)
                    {
                        spName = "EXEC GetTimeSlotsbyOrgId ";
                    }
                    else
                    {
                        spName = "EXEC GetTimeSlotsbyDocId ";
                    }
                    ds = _doctor.GetQueryResult(spName + " " + DoctorId + ", " + 1 + ", '" + startDate + "'");
                    result.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[0]).ToList();
                    result.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[1]).ToList();
                }
                var list = result.lstslotTimes.Where(x => x.IsBooked == false);
                if (list != null && list.Count() > 0)
                {
                    timeslot = list.Select(x => x.SlotSatrtTime).ToList();
                }
                return Json(new JsonResponse() { Status = 200, Data = timeslot }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetSlotTimes-GET");
                return Json(new JsonResponse() { Status = 0, Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        public void AddLogs(string message)
        {
            var log = new ErrorLog() { Type = "IP_IpAddress", Message = message, AppType = "SEARCH_CONT_LOG" };
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar) { Value = log.Message == null ? "" : log.Message });
            parameters.Add(new SqlParameter("@type", SqlDbType.VarChar) { Value = log.Type });
            parameters.Add(new SqlParameter("@apptype", SqlDbType.VarChar) { Value = log.AppType });

            try
            {
                _doctor.AddOrUpdateExecuteProcedure("AddErrorLog", parameters);
            }
            catch (Exception ex)
            {

            }
        }

        public string AssignLocationValues(string slocation)
        {
            if (string.IsNullOrEmpty(slocation))
            {
                Common cGetIp = new Common(_doctor);
                var _ipInfo = cGetIp.GetRemoteLocation();
                if (_ipInfo.zip != null)
                {
                    var result = GetZipCityState(_ipInfo.zip).FirstOrDefault();
                    slocation = Common.FormatLocation(result, _ipInfo);
                    ViewBag.SearchLocation = slocation;
                    return slocation;
                }
                else
                {
                    ViewBag.SearchLocation = "||";
                    return ViewBag.SearchLocation;
                }
            }
            else
            {
                ViewBag.SearchLocation = slocation.Replace("|", " ,");
                return ViewBag.SearchLocation;
            }

        }

        [Route("Search/AdsView")]
        public ActionResult AdsView(int AddsPerPage,int AddTypeId, int OrgTypeId, string UserTypeIds, string City, string Zipcode)
        {
            ViewBag.AddTypeId = AddTypeId.ToString();
            ViewBag.OrgTypeId = OrgTypeId.ToString();
            ViewBag.UserTypeIds = UserTypeIds.Trim().ToString();
            ViewBag.City = City.ToString().Trim();
            ViewBag.Zipcode = Zipcode.ToString().Trim();
            ViewBag.AddsPerPage = AddsPerPage.ToString();
            return View("_Advertisement");
        }
    }
}
