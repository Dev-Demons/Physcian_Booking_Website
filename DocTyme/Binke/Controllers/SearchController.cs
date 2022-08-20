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
        private readonly IDrugDetailService _drugDetailService;

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

        public SearchController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager,
            IAuthenticationManager authenticationManager, IDoctorInsuranceService doctorInsuranceService, IAddressService address, IAgeGroupService ageGroup, IDoctorService doctor, IFacilityService facility, /*IFacilityTypeService facilitytype,*/ ILanguageService language,
            IPatientService patient, IPharmacyService pharmacy, IGenderService genderService, IInsuranceTypeRepository insuranceTypeService, ISeniorCareService seniorCare, ISpecialityService speciality, IUserService appUser, ISlotService slot, IReviewService reviewService,
            ITempSlotBookingService tempSlotBookingService, IDrugDetailService drugDetailService, /*ITabletService tabletService,*/ IDrugTypeService drugTypeService,
            IDrugQuantityService drugQuantityService, IDrugManufacturerService drugManufacturerService,/*, IStateService state, ICityService city*/
            IFeaturedService featuredService, IOpeningHourService openingHour)
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
            _drugDetailService = drugDetailService;
            //_tabletService = tabletService;
            _drugManufacturerService = drugManufacturerService;
            _drugQuantityService = drugQuantityService;
            _drugTypeService = drugTypeService;
            //_state = state;
            //_city = city;
            _featuredService = featuredService;
            _openingHour = openingHour;
            string ipString = GetIPString();
            try
            {
                if (!string.IsNullOrEmpty(ipString))
                    _ipInfo = JsonConvert.DeserializeObject<IpInfo>(ipString);
                else
                    _ipInfo = new IpInfo();
            }
            catch
            {
                _ipInfo = new IpInfo();
            }
        }
        // GET: Search
        #region Doctor Search
        public ActionResult Index(string type, string search)
        {
            if (!IsValidType(type))
            {
                return RedirectToAction("Index", "Home");
            }
            BindDropdowns();

            var ageGroupsSeen = Common.ConvertDataTable<AgeGroupsSeen>(_doctor.GetQueryResult(StoredProcedureList.AgeGroupsSeen).Tables[0]).FirstOrDefault();

            var model = new SearchViewModel
            {
                AcceptingNewPatients = _doctor.GetCount(x => x.IsActive && !x.IsDeleted && x.IsAllowNewPatient),
                NearTermPcpAvailability = _doctor.GetCount(x => x.IsActive && !x.IsDeleted && x.IsNtPcp),
                PrimaryCare = _doctor.GetCount(x => x.IsActive && !x.IsDeleted && x.IsPrimaryCare),
                AgeGroupsSeen = ageGroupsSeen,
                IpInfo = _ipInfo
            };
            return View(model);
        }

        private string GetIPString()
        {
            string VisitorsIPAddr = string.Empty;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                //To get the IP address of the machine and not the proxy
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (System.Web.HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            else if (System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
            {
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (VisitorsIPAddr.Split('.').Length == 4)
            {
                try
                {
                    string info = new WebClient().DownloadString("http://ipinfo.io/" + VisitorsIPAddr);
                    return info;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost, Route("_Search")]
        public PartialViewResult _Search(SearchParameterModel model)
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

                var doctorIds = new List<int>();
                var paras = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.Search??""}
                };
                decimal lat2 = 0;
                decimal.TryParse(latOfUser, out lat2);
                decimal long2 = 0;
                decimal.TryParse(latOfUser, out long2);
                var doctorIdsWithDistanceList = new List<DoctorWithDistance>();
                var featureDoctorIds = _doctor.GetFeaturedDoctorIdsBySearchText(StoredProcedureList.GetFeaturedDoctorIdS, paras);
                if (model.AgeGroup != null && model.AgeGroup.Count > 0)
                {
                    var ageGrpDoctorIds = _doctor.GetAgeGroupReferenceId(model.AgeGroup);
                    doctorIds.AddRange(ageGrpDoctorIds);
                    doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(ageGrpDoctorIds, lat2, long2));
                }
                if (model.Affiliations != null && model.Affiliations.Count > 0)
                {
                    var affDoctorIds = _doctor.GetAffiliationDoctorId(model.Affiliations);
                    doctorIds.AddRange(affDoctorIds);
                    doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(affDoctorIds, lat2, long2));
                }
                if (model.Insurance != null && model.Insurance.Count > 0)
                {
                    var insDoctorIds = _doctor.GetInsuranceDoctorId(model.Insurance);
                    doctorIds.AddRange(insDoctorIds);
                    doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(insDoctorIds, lat2, long2));
                }
                if (!string.IsNullOrEmpty(model.Search))
                {
                    var specDoctorIds = _doctor.GetDoctorIdBySpecialityName(model.Search);
                    doctorIds.AddRange(specDoctorIds);
                    doctorIdsWithDistanceList.AddRange(_doctor.GetDistance(specDoctorIds, lat2, long2));
                }

                doctorIdsWithDistanceList.AddRange(_doctor.GetDoctorIdByNameOrAddress(model.Search, model.NTPA, model.ANP, model.PrimaryCare, model.Distance.SearchBox, lat2, long2));
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

                model.Distance.Latitude = string.IsNullOrEmpty(latOfUser) ? model.Distance.Latitude : latOfUser;
                model.Distance.Longitude = string.IsNullOrEmpty(longOfUser) ? model.Distance.Longitude : longOfUser;
                var listSearchparameters = GetSearchParameters(model, false, docIDsToFetch);


                //var allslotList = _doctor.GetDoctorSearchList(StoredProcedureList.SearchDoctorList, parameters.ToArray<object>());
                var allslotList = _doctor.GetDoctorSearchListPagination(StoredProcedureList.GETDoctorListPagination, listSearchparameters);
                int totalRecord = doctorIdsWithDistanceList.Count;
                var docIdsForDetails = new List<SqlParameter>()
                {
                    new SqlParameter("@DoctorIds",SqlDbType.NVarChar){ Value = string.Join(",",(allslotList != null && allslotList.Count > 0) ? allslotList.Select(x=>x.DoctorId) : new List<int>())}
                };
                var doctorDetails = _doctor.GetDoctorDetails(StoredProcedureList.GetDoctorDetails, docIdsForDetails);
                if (allslotList.Any())
                {
                    List<int> tempSpecities;
                    List<int> tempFacility;
                    var specities = _speciality.GetAll(x => x.IsActive && !x.IsDeleted)
                        .Select(x => new SelectIdValueModel { Id = x.SpecialityId, Value = x.SpecialityName }).ToList();
                    var facility = _facility.GetAll(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Facility")
                        .Select(x => new SelectIdValueModel { Id = x.OrganisationId, Value = x.OrganisationName }).ToList();
                    foreach (var item in allslotList)
                    {
                        if (doctorDetails != null && doctorDetails.Count > 0)
                        {
                            item.SpecitiesIds = doctorDetails.First(x => x.DoctorId == item.DoctorId).SpecitiesIds;
                            item.FacilityIds = doctorDetails.First(x => x.DoctorId == item.DoctorId).FacilityIds;
                            item.ReviewCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).ReviewCount;
                            item.InsuranceCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).InsuranceCount;
                            item.RatingNos = doctorDetails.First(x => x.DoctorId == item.DoctorId).RatingNos;
                            item.OfficeCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).OfficeCount;
                        }
                        if (!string.IsNullOrEmpty(item.SpecitiesIds))
                        {
                            tempSpecities = new List<int>();
                            item.SpecitiesIds.Split(',').Select(x => Convert.ToInt32(x)).ToList().ForEach(x => tempSpecities.Add(x));
                            item.Specities = specities.Where(x => tempSpecities.Contains(x.Id)).Select(x => x.Value).ToList();
                        }

                        if (!string.IsNullOrEmpty(item.FacilityIds))
                        {
                            tempFacility = new List<int>();
                            item.FacilityIds.Split(',').Select(x => Convert.ToInt32(x)).ToList().ForEach(x => tempFacility.Add(x));
                            item.Facility = facility.Where(x => tempFacility.Contains(x.Id))
                                .Select(x => new KeyValueModel { Key = x.Id, Value = x.Value }).ToList();
                        }
                    }
                }
                var searchResult = new Doctyme.Model.ViewModels.SearchDoctorResult() { listDoctors = allslotList };
                searchResult.TotalRecord = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;

                return PartialView(@"Partial/_Search", searchResult);
            }
            catch (Exception ex)
            {
                throw ex;
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
                var doctorIds = new List<int>();
                decimal lat2 = 0;
                decimal.TryParse(latOfUser, out lat2);
                decimal long2 = 0;
                decimal.TryParse(latOfUser, out long2);
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

                doctorIdsWithDistanceList.AddRange(_doctor.GetDoctorIdByNameOrAddress(model.Search, model.NTPA, model.ANP, model.PrimaryCare, model.Distance.SearchBox, lat2, long2));
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
                            item.InsuranceCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).InsuranceCount;
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
                throw ex;
            }
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
        [Route("DrugSearch/{search?}")]
        public ActionResult DrugSearch()
        {
            return View();
        }
        [Route("Doctor-Profile/{npi?}")]
        public ActionResult DoctorProfile(string npi)
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

        [HttpGet, Route("_RequestAppointment/{id?}")]
        public PartialViewResult RequestAppointment(int id = 0)
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
                throw;
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
        public ActionResult Pharmacy(string text)
        {      
            ViewBag.DistanceList = DistanceList();
            ViewBag.SearchBox = text;

            return View();
        }
        [Route("Filter/Facility/{text?}")]
        public ActionResult Facility(string text)
        {
            ViewBag.DistanceList = DistanceList();
            ViewBag.SearchBox = text;

            ViewBag.Facilityoption = _facility.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new KeyValuePairModel
            {
                Id = x.OrganisationId.ToString(),
                Name = x.OrganisationName,
                Count = 2
            }).ToList();

            ViewBag.Facilitytype = _facility.GetDrpFacilityTypeList(SqlQuery.GeFacilityTypeDropDownList, new object[] { }).Select(x => new KeyValuePairModel
            {
                Id = x.OrganizationTypeID.ToString(),
                Name = x.OrganizationTypeName,
                Count = 2
            }).ToList();

            //ViewBag.Facilitytype = _facility.GetAll(x => x.IsActive && !x.IsDeleted).Where(q => q.OrganisationType != null).Select(x => new KeyValuePairModel
            //{
            //    Id = x.OrganizationTypeID.ToString(),
            //    Name = x.OrganisationType.Org_Type_Name,
            //    Count = 2
            //}).ToList();

            return View();
        }
        [Route("Filter/SeniorCare/{text?}")]
        public ActionResult SeniorCare(string text)
        {
            ViewBag.DistanceList = DistanceList();
            ViewBag.SearchBox = text;

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
                throw ex;
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
                decimal.TryParse(latOfUser, out long2);
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
                    var orgIds = _featuredService.GetOrganisationIdsFromSearchText(model.SearchBox, organizationTypeId, lat2, long2,
                        model.Distance.DistanceType == null ? 0 : Convert.ToInt32(model.Distance.DistanceType), skip, take, out totalRecord);
                    lstOrgIds.AddRange(orgIds.Select(m => m.OrganisationId).ToList());
                }

                totalRecord = totalRecord + featureFacilityIds.Count;
                var advertisementList = _featuredService.GetAdvertisementsFromSearchText(lstOrgIds, organizationTypeId);//Added by Reena
                parameters.Add(new SqlParameter("@OrganisationIds", SqlDbType.NVarChar) { Value = string.Join(",", lstOrgIds) });
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
                throw;
            }
        }

        [HttpPost, Route("_SearchPharmacy")]
        public PartialViewResult _SearchPharmacy(SearchParamModel model)
        {
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.Pharmacy);
            return PartialView(@"Partial/_SearchPharmacy", searchResult);
        }

        [Route("home/Pharmacy/{id?}")]
        public ActionResult PharmacyProfile(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Pharmacy");
            }
            var pharmacy = _pharmacy.GetSingle(x => x.OrganisationId == id);
            return View(pharmacy);
        }
        #endregion

        #region SeniorCare
        [HttpPost, Route("_SearchSeniorCare")]
        public PartialViewResult _SeniorCare(SearchParamModel model)
        {
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.SeniorCare);
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
                throw;
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
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.Facility);
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
                throw;
            }
        }
        #endregion

        #region Drugs
        [HttpGet, Route("SearchDrug/{search?}")]
        public ActionResult SearchDrug(String search)
        {
            try
            {
                var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@Search", SqlDbType.NVarChar) { Value = search}
            };

                var drugs = new List<Drug>()
                                        {new Drug()
                                            { DrugId=1,
                                            DrugName ="Amlodipine"
                                        }  };
                // return View(drugs);
                return View();
            }
            catch (Exception ex)
            {
                throw;
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
                throw;
            }

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
                throw;
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

        private void BindDropdowns()
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
                throw ex;
            }
        }

        [HttpPost, Route("DrugSearch/GetDrugDetails")]
        public JsonResult GetDrugDetails(SearchDrugRecordsParam searchDrugRecordsParam)
        {
            var searchRecords = new List<SearchDrugRecord>();
            var searchRecordWithPageSize = new Records();

            try
            {
                var drugDetails = _drugDetailService.GetAll(x => x.IsActive == true && !x.IsDeleted);

                if (!string.IsNullOrEmpty(searchDrugRecordsParam.MedicineName))
                {
                    // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                    //drugDetails = drugDetails.Where(item => item.MedicineName.ToLower().Contains(searchDrugRecordsParam.MedicineName.ToLower()));
                }

                if (!string.IsNullOrEmpty(searchDrugRecordsParam.StartWithAlphabetically))
                {
                    // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
                    //drugDetails = drugDetails.Where(item => item.MedicineName.StartsWith(searchDrugRecordsParam.StartWithAlphabetically));
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
                        //LongDescription = drug.LongDescription,
                        //MedicineName = drug.MedicineName,
                        //ShortDescription = drug.ShortDescription,
                        //SideEffects = drug.SideEffects,
                        //Tips = drug.Tips,
                        //Interaction = drug.Interaction,
                        PharmacyDetail = SpSearchRecord(searchDrugRecordsParam, drug.DrugDetailId).ToList()
                    };
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
                throw ex;
            }
        }

        private IList<SpSearchDrugViewModel> SpSearchRecord(SearchDrugRecordsParam searchDrugRecordsParam, int drugDetailId)
        {
            IList<SpSearchDrugViewModel> searchDrugRecords = new List<SpSearchDrugViewModel>();
            try
            {
                searchDrugRecords = _drugDetailService.SearchDrug(StoredProcedureList.SearchDrug, drugDetailId).ToList();
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
                throw ex;
            }
            return searchDrugRecords;
        }

        [HttpGet, Route("SearchDrug/MostlySearchedDrug")]
        public JsonResult MostlySearchedDrug()
        {
            string text = System.IO.File.ReadAllText(Server.MapPath(StaticFilePath.WebSettings)).NullToString();
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
                throw;
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
                    if (user?.Result.Doctors != null && user.Result.Doctors.Count > 0)
                    {
                        address = _address.GetAll().Where(x => x.ReferenceId == user.Result.Doctors.FirstOrDefault().DoctorId).FirstOrDefault();
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
                throw ex;
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
                    throw ex;
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
                throw ex;
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
        public ActionResult Organization(string doctorName, string npi = "1588667638")
        {
            //string npi = doctNamenpi.Split('-')[1].ToString();
            Doctyme.Model.ViewModels.OrganizationViewModel result = new OrganizationViewModel();
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetDoctorProfileData + " " + npi);
            var doctorProfile = Common.ConvertDataTable<OrganizationViewModel>(ds.Tables[0]).FirstOrDefault();
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
            doctorProfile.Maxslots = doctorProfile.lstslotTimes.Count > 0 ? Convert.ToInt32(doctorProfile.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
            doctorProfile.MaxDays = 1;
            doctorProfile.CalenderDatesCount = doctorProfile.lstslotsDates.Count > 0 ? doctorProfile.lstslotsDates.FirstOrDefault().MaxCount : 0;
            //doctorProfile.CalenderDatesCount = 7;
            doctorProfile.OrganisationId = doctorProfile.lstOrgAddress.Select(i => i.OrganisationId).FirstOrDefault();
            doctorProfile.NPI = npi;
            doctorProfile.ReturUrl = "Profile/Doctor/" + doctorProfile.FullForDoctor + "-" + doctorProfile.NPI;
            doctorProfile.SlotFor = "Doctor";
            return View(doctorProfile);
        }
        [HttpGet]
        [Route("GetSlots/")]
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
                    DateTime startDateTIme = DateTime.Parse(resultweek.StartDateTime);
                    DateTime endDateTIme = DateTime.Parse(resultweek.EndDateTime);
                    string strMinFormat = startDateTIme.ToString("hh:mm tt");
                    string endMinFormat = endDateTIme.ToString("hh:mm tt");

                    result.OpeningTime = strMinFormat + " -" + endMinFormat;
                    opeHoursFial = dateValue.ToString("ddd") + "  " + resultweek.StartDateTime + " -" + resultweek.EndDateTime;
                }

                return Json(new JsonResponse() { Status = 200, Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Route("rSlotConfirm/")]
        public JsonResult RedirectSlotCofirm(Doctyme.Model.ViewModels.SlotConfirmation _model)
        {
            TempData["SlotConfirmation"] = _model;
            //return RedirectToAction("SlotsConfirmation", "Profile");
            return Json(new JsonResponse() { Status = 200, Data = "Sucess" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
