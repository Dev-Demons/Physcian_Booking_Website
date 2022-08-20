using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Binke.Models;
using Binke.Utility;
using Binke.ViewModels;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Binke.Controllers
{
    [Authorize]
    public class FacilityController : Controller
    {
        private readonly IAddressService _address;
        private readonly IFacilityService _facility;
        private readonly IFacilityTypeService _facilityType;
        private ApplicationUserManager _userManager;
        private readonly IStateService _state;
        private readonly IFeaturedService _featuredService;
        private readonly IUserService _appUser;
        private readonly IRepository _repo;
        private int _facilityId;
        public FacilityController(IAddressService address, IFacilityService facility, IFeaturedService featuredService, IFacilityTypeService facilityType,
            IStateService state, IUserService appUser, ApplicationUserManager applicationUserManager, IRepository repo)
        {
            _address = address;
            _facility = facility;
            _facilityType = facilityType;
            _state = state;
            _appUser = appUser;
            _userManager = applicationUserManager;
            _featuredService = featuredService;
            _repo = repo;
        }


        #region Admin Section
        // GET: Facility
        public ActionResult Index()
        {
            return View();
        }

        [Route("Facility/Reviews")]
        public ActionResult Reviews()
        {
            return View();
        }

        [Route("Facility/Reviews/{id}")]
        public ActionResult GetReviewList(JQueryDataTableParamModel param, int id = 0)
        {
            var model = new SearchParamModel();
            //model.PageIndex = pageIndex;
            //model.SearchBox = param.sSearch;
            //model.PageSize = param.iDisplayLength;
            var data = GetReviewSearchResult(model, OrganisationTypes.Facility);

            return Json(new
            {
                param.sEcho,
                iTotalRecords = data.TotalRecord,
                iTotalDisplayRecords = data.TotalRecord,
                aaData = data.ReviewProviderList
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("Facility/Specialities")]
        public ActionResult Specialities()
        {
            return View();
        }

        [Route("Facility/Summary")]
        public ActionResult Summary()
        {
            return View();
        }
        [Route("Facility/StateLicense")]
        public ActionResult StateLicense()
        {
            return View();
        }
        [Route("Facility/InsurancePlan")]
        public ActionResult InsurancePlan()
        {
            return View();
        }
        [Route("Facility/OpeningHours")]
        public ActionResult OpeningHours()
        {
            return View();
        }
        [Route("Facility/SocialMedia")]
        public ActionResult SocialMedia()
        {
            return View();
        }
        [Route("Facility/Taxonomy")]
        public ActionResult Taxonomy()
        {
            return View();
        }
        [Route("Facility/Addresses")]
        public ActionResult Addresses()
        {
            return View();
        }
        [Route("Facility/Booking")]
        public ActionResult Booking()
        {
            return View();
        }
        [Route("Facility/Image")]
        public ActionResult Image()
        {
            return View();
        }

        [Route("Facility/Profile/{userId}")]
        public ActionResult Profile(int userId)
        {
            var users = _facility.SQLQuery<DrpUser>("select * from AspNetUsers where Id = " + userId.ToString()).ToList();
            var user = new DrpUser();
            if (users.Count > 0)
                user = users.First();
            return View(user);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditFacilityUser(DrpUser user)
        {
            try
            {
                if (user.Password != user.ConfirmPassword)
                    return Json(new JsonResponse { Status = 0, Message = "Password and confirm password must match" }, JsonRequestBehavior.AllowGet);

                if (!string.IsNullOrEmpty(user.Password))
                {
                    if (user.Password.Length < 8 || user.Password.Length > 100)
                        return Json(new JsonResponse { Status = 0, Message = "Password must be greater than 8 and less than 100 characters long" }, JsonRequestBehavior.AllowGet);

                    var rgx = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,150}$");
                    if(!rgx.IsMatch(user.Password))
                        return Json(new JsonResponse { Status = 0, Message = "Passwords must contain at least one lowercase letter, one uppercase letter, and one number." }, JsonRequestBehavior.AllowGet);

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                    var result = await _userManager.ResetPasswordAsync(user.Id, code, user.Password);
                    if (!result.Succeeded)
                        return Json(new JsonResponse { Status = 0, Message = "Error in updating password.. may be it will not meet password requirements." }, JsonRequestBehavior.AllowGet);
                }
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("FirstName", SqlDbType.VarChar) { Value = user.FirstName });
                parameters.Add(new SqlParameter("MiddleName", SqlDbType.VarChar) { Value = user.MiddleName });
                parameters.Add(new SqlParameter("LastName", SqlDbType.VarChar) { Value = user.LastName });
                parameters.Add(new SqlParameter("PhoneNumber", SqlDbType.NVarChar) { Value = user.PhoneNumber });
                parameters.Add(new SqlParameter("Email", SqlDbType.NVarChar) { Value = user.Email });
               
                _facility.ExecuteSqlCommandForUpdate("AspNetUsers", "Id", user.Id, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Facility user updated successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating facility user" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("GetFacilityReviews/{id?}")]
        public ActionResult GetFacilityReviews(int id, JQueryDataTableParamModel param)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetReviewSearchResult(model, id);
            var allFacilitys = searchResult.ReviewProviderList.AsEnumerable();

            Func<ReviewProviderModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Description
                            //: sortColumnIndex == 3 ? c.Email
                            //    : sortColumnIndex == 4 ? c.
                            : c.Description;
            allFacilitys = sortDirection == "asc" ? allFacilitys.OrderBy(orderingFunction).ToList() : allFacilitys.OrderByDescending(orderingFunction).ToList();

            //var display = allFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var total = searchResult.TotalRecord;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allFacilitys
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("GetFacilityList/{flag}")]
        public ActionResult GetFacilityList(bool flag, JQueryDataTableParamModel param)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.Facility);
            var allFacilitys = searchResult.FacilityProviderList.AsEnumerable();
            Func<FacilityProviderModel, string> orderingFunction =
                c => sortColumnIndex == 0 ? c.OrganisationId.ToString() : c.OrganisationName;
            if (sortColumnIndex == 0)
                sortDirection = "desc";
            allFacilitys = sortDirection == "asc" ? allFacilitys.OrderBy(orderingFunction).ToList() : allFacilitys.OrderByDescending(orderingFunction).ToList();

            //var display = allFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var total = searchResult.TotalRecord;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = model.PageSize,
                iTotalDisplayRecords = total,
                aaData = allFacilitys
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("GetOpeningHours/{id?}")]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult GetOpeningHours(int? id, JQueryDataTableParamModel param)
        {
            try
            {
                var allPharmacy = _facility.ExecWithStoreProcedure<OrgOpeningHoursViewModel>("spOpeningHour_Get @Search, @OrganizationID, @OrganizationTypeID, @UserTypeID, @PageIndex, @PageSize, @Sort",
                         new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                         new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id },
                         new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = User.IsInRole(UserRoles.Admin) ? UserTypes.Admin : UserTypes.Facility },
                         new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1006 },
                         new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                         new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                         new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                         ).ToList();

                var result = allPharmacy
                    .Select(x => new OrgOpeningHoursListViewModel()
                    {
                        OpeningHourID = x.OpeningHourID,
                        OrganizationID = x.OrganizationID,
                        OrganisationName = x.OrganisationName,
                        OrganizatonTypeID = x.OrganizatonTypeID,
                        WeekDay = x.WeekDay,
                        CalendarDate = x.CalendarDate,
                        StartDateTime = x.StartDateTime,
                        EndDateTime = x.EndDateTime,
                        SlotDuration = x.SlotDuration,
                        UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                        WeekDayName = getWeekDayName((int)x.WeekDay),
                        IsActive = x.IsActive,
                        IsHoliday = x.IsHoliday,
                        IsDeleted = x.IsDeleted,
                        Comments = x.Comments,
                        TotalRecordCount = x.TotalRecordCount
                    }).OrderBy(o => o.WeekDay).ToList();

                int TotalRecordCount = 0;

                if (result.Count > 0)
                    TotalRecordCount = result[0].TotalRecordCount;


                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getWeekDayName(int day)
        {
            string DayName = "";

            if (day == 1)
                DayName = "Sunday";
            if (day == 2)
                DayName = "Monday";
            if (day == 3)
                DayName = "Tuesday";
            if (day == 4)
                DayName = "Wednesday";
            if (day == 5)
                DayName = "Thursday";
            if (day == 6)
                DayName = "Friday";
            if (day == 7)
                DayName = "Saturday";

            return DayName;
        }


        [HttpGet, Route("GetStateLicense/{id?}")]
        public ActionResult GetStateLicense(int id, JQueryDataTableParamModel param)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetStateLicenseSearchResult(model, id);
            var allFacilitys = searchResult.StateLicenseProviderList.AsEnumerable();
            ViewBag.StateLicenseList = JsonConvert.SerializeObject(allFacilitys);
            Func<StateLicenseProviderModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.ProviderLicenseNumber
                            //: sortColumnIndex == 3 ? c.Email
                            //    : sortColumnIndex == 4 ? c.
                            : c.ProviderLicenseNumber;
            allFacilitys = sortDirection == "asc" ? allFacilitys.OrderBy(orderingFunction).ToList() : allFacilitys.OrderByDescending(orderingFunction).ToList();

            //var display = allFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var total = searchResult.TotalRecordCount;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allFacilitys
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("GetInsurancePlan/{id?}")]
        public ActionResult GetInsurancePlan(int id, JQueryDataTableParamModel param)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetInsurancePlanSearchResult(model, id);
            var allFacilitys = searchResult.InsurancePlanProviderList.AsEnumerable();

            Func<InsurancePlanProviderModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                            //: sortColumnIndex == 3 ? c.Email
                            //    : sortColumnIndex == 4 ? c.
                            : c.Name;
            allFacilitys = sortDirection == "asc" ? allFacilitys.OrderBy(orderingFunction).ToList() : allFacilitys.OrderByDescending(orderingFunction).ToList();

            //var display = allFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var total = searchResult.TotalRecordCount;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allFacilitys
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("GetSummary/{id?}")]
        public ActionResult GetSummary(int id)
        {
            var summary = this.GetSummaryResult(id);
            return Json(new
            {
                data = summary
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("GetSpecialities/{id?}")]
        public ActionResult GetSpecialities(int id, JQueryDataTableParamModel param)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetSpecilitiesSearchResult(model, id);
            foreach (var item in searchResult.SpecialityProviderModel)
            {
                item.ParentSpecialization = item.ParentID > 0 ? GetTaxonomyCodeById(item.ParentID.Value, "special") : null;
            }
            var allFacilitys = searchResult.SpecialityProviderModel.AsEnumerable();
            Func<SpecialityProviderModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.SpecialtyText
                            //: sortColumnIndex == 3 ? c.Email
                            //    : sortColumnIndex == 4 ? c.
                            : c.SpecialtyText;
            // allFacilitys = sortDirection == "asc" ? allFacilitys.OrderBy(orderingFunction).ToList() : allFacilitys.OrderByDescending(orderingFunction).ToList();

            //var display = allFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var total = searchResult.SpecialityProviderModel.Count > 1 ? searchResult.SpecialityProviderModel[0].TotalRecord : 0;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allFacilitys
            }, JsonRequestBehavior.AllowGet);
        }

        public string GetTaxonomyCodeById(int id, string type)
        {
            string result = "";

            var info = _facility.SQLQuery<TaxonomyCodeViewModel>("spGET_TaxonomyCode_ById @TaxonomyID", new SqlParameter("TaxonomyID", System.Data.SqlDbType.Int) { Value = id }).ToList();

            if (info.Count > 0)
            {
                if (type == "code")
                    result = info[0].Taxonomy_Code;

                if (type == "special")
                    result = info[0].Specialization;
            }
            return result;
        }

        [Route("GetSocialMedia/{id?}")]
        public ActionResult GetSocialMedia(int id, JQueryDataTableParamModel param)
        {
            var socialMediaInfo = _facility.ExecWithStoreProcedure<OrgSocialMediaViewModel>("spSocialMedia_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                     new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                     new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = User.IsInRole(UserRoles.Admin) ? UserTypes.Admin : UserTypes.Facility },
                     new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                     new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = OrganisationTypes.Facility },
                     new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                     new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                     new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                     );

            var result = socialMediaInfo
                .Select(x => new OrgSocialMediaListViewModel()
                {
                    SocialMediaId = x.SocialMediaId,
                    OrganisationId = (int)x.ReferenceId,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeID = x.OrganizationTypeID,
                    UpdatedDate = x.UpdatedDate,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Facebook = x.Facebook,
                    Twitter = x.Twitter,
                    LinkedIn = x.LinkedIn,
                    Instagram = x.Instagram,
                    Youtube = x.Youtube,
                    Pinterest = x.Pinterest,
                    Tumblr = x.Tumblr,
                    TotalRecordCount = x.TotalRecordCount
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("GetTaxonomy/{id?}")]
        public ActionResult GetTaxonomy(int? id, JQueryDataTableParamModel param)
        {
            try
            {
                var taxonomyInfo = _facility.ExecWithStoreProcedure<OrganisationSpecialityTaxonomyViewModel>("GetSpecialitiesFromOrgId @Search, @OrganizationId, @PageIndex, @PageSize, @Sort",
                        new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                        new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = id ?? 0 },
                        new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                        new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                        new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                        ).ToList();

                //var result = taxonomyInfo
                //    .Select(x => new OrganisationTaxonomyListViewModel()
                //    {
                //        TaxonomyID = x.TaxonomyID,
                //        ParentID = x.ParentID,
                //        Taxonomy_Code = x.Taxonomy_Code,
                //        Specialization = x.Specialization,
                //        IsActive = x.IsActive,
                //        TotalRecordCount = x.TotalRecordCount,
                //        Description = x.Description
                //    }).ToList();

                int TotalRecordCount = 0;

                if (taxonomyInfo.Count > 0)
                    TotalRecordCount = taxonomyInfo[0].TotalRecordCount;


                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = taxonomyInfo
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetAddresses/{id?}")]
        public ActionResult GetAddresses(int? id, JQueryDataTableParamModel param)
        {
            var allPharmacyAddress = _facility.ExecWithStoreProcedure<OrganisationAddressViewModel>("spAddress_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Facility },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id ?? 0 },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = OrganisationTypes.Facility },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allPharmacyAddress
                .Select(x => new OrganisationAddressListViewModel()
                {
                    AddressId = x.AddressId,
                    OrganisationId = x.ReferenceId,
                    OrganisationName = x.ReferenceName,
                    AddressTypeId = x.AddressTypeId,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    TotalRecordCount = x.TotalRecordCount,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    ZipCode = GetCityStateInfoById(x.CityStateZipCodeID, "zip"),
                    City = GetCityStateInfoById(x.CityStateZipCodeID, "city"),
                    State = GetCityStateInfoById(x.CityStateZipCodeID, "state"),
                    Country = GetCityStateInfoById(x.CityStateZipCodeID, "country"),
                    Phone = x.Phone,
                    Fax = x.Fax,
                    Email = x.Email,
                    WebSite = x.WebSite,
                    Lat = x.Lat,
                    Lon = x.Lon,
                    CityStateZipCodeID = x.CityStateZipCodeID
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("GetImageList/{id?}")]
        public ActionResult GetImageList(int? id,JQueryDataTableParamModel param)
        {
            var result = _facility.ExecWithStoreProcedure<OrgSiteImageViewModel>("spOrganizatonSiteImage_Get @Search, @ReferenceId, @UserTypeID, @OrganizatonTypeID, @PageIndex, @PageSize, @Sort",
                   new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                   new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                   new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = User.IsInRole(UserRoles.Admin) ? UserTypes.Admin : UserTypes.Facility },
                   new SqlParameter("OrganizatonTypeID", System.Data.SqlDbType.Int) { Value = OrganisationTypes.Facility },
                   new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                   new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                   new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                   ).Select(x => new OrgSiteImageListViewModel
                   {
                       SiteImageId = x.SiteImageId,
                       OrganisationName = x.ReferenceName,
                       OrganisationId = x.ReferenceId,
                       ImagePath = x.ImagePath,
                       Name = x.Name,
                       CreatedDate = x.CreatedDate.ToDefaultFormate(),
                       UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                       IsProfile = x.IsProfile,
                       IsActive = x.IsActive,
                       IsDeleted = x.IsDeleted,
                       TotalRecordCount = x.TotalRecordCount
                   }).ToList();


            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public PartialViewResult FacilitySiteImages(int? id)
        {
            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _facility.ExecWithStoreProcedure<OrgSiteImageViewModel>("spOrganizatonSiteImage_GetById @SiteImageId",
                    new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrgSiteImageUpdateViewModel
                    {
                        SiteImageId = x.SiteImageId,
                        OrganisationId = x.ReferenceId,
                        OrganisationName = x.ReferenceName,
                        Name = x.Name,
                        OrganizatonTypeID = x.OrganizatonTypeID,
                        ImagePath = x.ImagePath,
                        IsProfile = x.IsProfile,
                        IsActive = x.IsActive
                    }).First();

                return PartialView(@"/Views/Facility/Partial/_FacilityImages.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgSiteImageUpdateViewModel();
                return PartialView(@"/Views/Facility/Partial/_FacilityImages.cshtml", orgInfo);
            }
        }


        [HttpPost, Route("AddEditFacilitySiteImage"), ValidateAntiForgeryToken]
        public JsonResult AddEditFacilitySiteImage(OrgSiteImageUpdateViewModel model, HttpPostedFileBase Image1)
        {

            try
            {
                if (model.OrganisationId > 0)
                {
                    string imagePath = "";

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/FacilitySiteImages/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/FacilitySiteImages"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "Facility-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/FacilitySiteImages"), newImageName);
                        Image1.SaveAs(path);

                        imagePath = newImageName;
                    }

                    if (model.SiteImageId > 0 && imagePath == "")
                    {
                        imagePath = model.ImagePath;
                    }



                    int ExecWithStoreProcedure = _facility.ExecuteSQLQuery("spOrganizatonSiteImage_Create " +
                            "@SiteImageId," +
                            "@ReferenceId," +
                            "@ImagePath," +
                            "@IsProfile," +
                            "@IsActive," +
                            "@CreatedBy," +
                            "@UserTypeID," +
                            "@Name",
                                          new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = model.SiteImageId > 0 ? model.SiteImageId : 0 },
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                          new SqlParameter("ImagePath", System.Data.SqlDbType.VarChar) { Value = (object)(imagePath) ?? DBNull.Value },
                                          new SqlParameter("IsProfile", System.Data.SqlDbType.Bit) { Value = (object)model.IsProfile ?? DBNull.Value },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Facility },
                                          new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = model.Name }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Image information uploaded successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Facility Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Image information uploading Error.." });
            }

        }

        [Route("DeleteFacilitySiteImage/{id}/{ImgName}")]
        [HttpPost]
        public JsonResult DeleteFacilitySiteImage(int id, string ImgName)
        {
            try
            {
                if (ImgName != null)
                {
                    var path = Server.MapPath("~/Uploads/FacilitySiteImages/" + ImgName);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                int ExecWithStoreProcedure = _facility.ExecuteSQLQuery("spOrganizatonSiteImage_Remove @SiteImageId, @ModifiedBy",
                    new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Facility site image has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Facility site image deleted Error.." });
            }
        }

        [Route("GetDrpTaxonomyCodes/{id}")]
        public ActionResult GetDrpTaxonomyCodes(int id, string Prefix)
        {
            try
            {
                var codeList = _facility.SQLQuery<DrpTaxonomyCodes>("select Taxonomy_Code from Taxonomy where Taxonomy_Code like '%" + Prefix + "%' and ParentID = '" + id + "'").ToList();
                return Json(codeList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetDrpAddressList/{id}")]
        public ActionResult GetDrpAddressList(string Prefix, int id)
        {
            try
            {
                var addList = _facility.SQLQuery<DrpAddress>("select ad.Address1, ad.Address2, ad.AddressId, csz.City, csz.State, csz.Country, csz.ZipCode from Address ad join CityStateZip csz on csz.CityStateZipCodeID = ad.CityStateZipCodeID where " +
                    "(Address1 like '%" + Prefix + "%'" + " or Address2 like '%" + Prefix + "%') and ad.IsActive = 1 and ad.IsDeleted = 0 and " +
                    " ReferenceId = '" + id + "'").ToList();
                return Json(addList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetDrpInsuranceTypeList")]
        public ActionResult GetDrpInsuranceTypeList()
        {
            try
            {
                var insuranceTypeList = _facility.ExecWithStoreProcedure<DrpInsuranceType>("spInsuranceTypeDropDownList_Get").ToList();
                return Json(insuranceTypeList.OrderBy(m => m.Name).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetDrpDoctorList/{id}")]
        public ActionResult GetDrpDoctorList(int id)
        {
            var doctorsList = _facility.ExecWithStoreProcedure<OrgDoctorsDropDownViewModel>("spDocOrgBooking_GetOrgDoc @OrganizationId",
               new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = id }
               ).Select(x => new OrgDoctorsDropDownViewModel()
               {
                   DoctorId = x.DoctorId,
                   DisplayName = x.DoctorName + " [" + x.Credential + "]"
               }).ToList();

            return Json(doctorsList.OrderBy(m => m.DoctorName).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("GetDrpInsurancePlanByTypeId")]
        public JsonResult GetDrpInsurancePlanByTypeId(int TypeId)
        {
            var planList = _facility.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownListByType_Get @InsuranceTypeId",
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("GetDrpInsurancePlanByPlanId")]
        public JsonResult GetDrpInsurancePlanByPlanId(int PlanId)
        {
            var planList = _facility.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownListByPlanId_Get @InsurancePlanId",
                new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = PlanId }
                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [Route("GetDrpUserList")]
        public ActionResult GetDrpUserList(string Prefix)
        {
            try
            {
                var query = "select p.PatientId, anu.FirstName, anu.MiddleName, anu.LastName, anu.Email, anu.PhoneNumber from Patient p join AspNetUsers anu on p.UserId = anu.Id " +
                    "where p.IsActive = 1 and p.IsDeleted = 0 and anu.IsActive = 1 and anu.IsDeleted = 0 and " +
                    "(anu.FirstName like '%" + Prefix.Trim() + "%'" + " or anu.MiddleName like '%" + Prefix.Trim() + "%'" + " or anu.LastName like '%" + Prefix.Trim() + "%')";
                var userList = _facility.SQLQuery<DrpUser>(query).ToList();
                return Json(userList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("GetDrpInsurancePlanList")]
        public ActionResult GetDrpInsurancePlanList(int insuranceTypeId, int referenceId)
        {
            try
            {
                var planList = _facility.ExecWithStoreProcedure<DrpInsurancePlan>("spOrgInsurancePlanDropDownList_Get @OrganizationId, @InsuranceTypeId",
                 new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = referenceId },
                 new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = insuranceTypeId }
                 ).ToList();
                return Json(planList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetDrpInsuranceProviderList")]
        public ActionResult GetDrpInsuranceProviderList()
        {
            try
            {
                var insuranceProviderList = _facility.SQLQuery<DrpInsuranceProvider>("select InsProviderId, InsCompanyName from InsuranceProviders").ToList();
                return Json(insuranceProviderList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetDrpStateList")]
        public ActionResult GetDrpStateList()
        {
            try
            {
                var stateList = _facility.SQLQuery<DrpState>("select distinct State from CityStateZip Where Country = 'US' order by State asc").ToList();
                return Json(stateList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetBookingList/{id}")]
        public ActionResult GetBookingList(int id, JQueryDataTableParamModel param)
        {
            try
            {

                var orgInfo = _facility.ExecWithStoreProcedure<OrgBookingViewModel>("spDocOrgBooking_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                     new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                     new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = User.IsInRole(UserRoles.Admin) ? UserTypes.Admin : UserTypes.Facility },
                     new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                     new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = OrganisationTypes.Facility },
                     new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                     new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                     new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                     ).ToList();

                var result = orgInfo
                    .Select(x => new OrgBookingListViewModel()
                    {
                        InsurancePlanId = x.InsurancePlanId.HasValue ? x.InsurancePlanId.Value : 0,
                        InsuranceTypeId = x.InsuranceTypeId.HasValue ? x.InsuranceTypeId.Value: 0,
                        AddressId = x.AddressId,
                        SlotId = x.SlotId,
                        DoctorId = x.DoctorId,
                        OrganisationName = x.OrganisationName,
                        OrganisationId = x.OrganisationId.HasValue ? x.OrganisationId.Value : 0,
                        DoctorName = x.DoctorName + " [" + x.Credential + "]",
                        OrganizatonTypeID = x.OrganizatonTypeID.HasValue ? x.OrganizatonTypeID.Value : 0,
                        UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                        SlotDate = x.SlotDate,
                        SlotTime = x.SlotTime,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        BookedFor = x.BookedFor,
                        Description = x.Description,
                        FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                        FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        TotalRecordCount = x.TotalRecordCount,
                        IsBooked = x.IsBooked,
                        IsEmailReminder = x.IsEmailReminder,
                        IsTextReminder = x.IsTextReminder,
                        IsInsuranceChanged = x.IsInsuranceChanged
                    }).ToList();

                int TotalRecordCount = 0;

                if (result.Count() > 0)
                    TotalRecordCount = result[0].TotalRecordCount;


                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private FacilityProviderListModel GetOrganizationSearchResult(SearchParamModel model, int organizationTypeId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 0 : model.PageIndex;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationTypeID",SqlDbType.Int) {Value = organizationTypeId},
                    new SqlParameter("@Sorting",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                };

                if (User.IsInRole(UserRoles.Facility))
                    parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = Convert.ToInt32(User.Identity.GetUserId()) });
                else
                    parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = 0 });
                int totalRecord = 0;
                var allslotList = _facility.GetFacilityListByTypeId(StoredProcedureList.GetFacilitiByTypeId, parameters, out totalRecord).ToList();
                var searchResult = new FacilityProviderListModel() { FacilityProviderList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecord = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ReviewProviderListModel GetReviewSearchResult(SearchParamModel model, int organizationId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationId",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@Sort",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                    new SqlParameter("@UserTypeID",SqlDbType.Int) {Value = UserTypes.Facility},
                };

                int totalRecord = 0;
                var allslotList = _facility.GetReviewListByTypeId(StoredProcedureList.GetReview, parameters, out totalRecord).ToList();
                var searchResult = new ReviewProviderListModel() { ReviewProviderList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecord = totalRecord;
                //searchResult.PageSize = model.PageSize;
                //searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private OpeningHourProviderListModel GetOpeningHourSearchResult(SearchParamModel model, int organizationId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationId",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@Sort",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                };

                int RecordCount = 0;
                var allslotList = _facility.GetOpeningHoursByOrgId(StoredProcedureList.GetOpeningHour, parameters, out RecordCount).ToList();
                var searchResult = new OpeningHourProviderListModel() { OpeningHourProviderList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecordCount = RecordCount;
                //searchResult.PageSize = model.PageSize;
                //searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private StateLicenseProviderListModel GetStateLicenseSearchResult(SearchParamModel model, int organizationId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationId",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@Sort",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                };

                int RecordCount = 0;
                var allslotList = _facility.GetStateLicenseByOrgId(StoredProcedureList.GetStateLicense, parameters, out RecordCount).ToList();
                var searchResult = new StateLicenseProviderListModel() { StateLicenseProviderList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecordCount = RecordCount;
                //searchResult.PageSize = model.PageSize;
                //searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private InsurancePlanProviderListModel GetInsurancePlanSearchResult(SearchParamModel model, int organizationId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex + 1;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationId",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@Sort",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                };

                int RecordCount = 0;
                var allslotList = _facility.GetInsurancePlanByOrgId(StoredProcedureList.GetInsurancePlan, parameters, out RecordCount).ToList();
                var searchResult = new InsurancePlanProviderListModel() { InsurancePlanProviderList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecordCount = RecordCount;
                //searchResult.PageSize = model.PageSize;
                //searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private SummaryProviderModel GetSummaryResult(int organisationId)
        {
            var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@OrganisationId",SqlDbType.Int) {Value = organisationId},
                };

            var allslotList = _facility.GetSummary(StoredProcedureList.GetSummary, parameters).ToList();
            if (allslotList.Count > 0)
                return allslotList.First();
            else
                return new SummaryProviderModel();
        }

        private SpecialityProviderListModel GetSpecilitiesSearchResult(SearchParamModel model, int organizationId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex + 1;
                if (model.PageIndex == 0 || model.Sorting == null)
                    model.Sorting = "desc";
                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    //new SqlParameter("@OrganizationId",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@Sort",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                };

                int totalRecord = 0;
                var allslotList = _facility.GetSpecialityByOrgId(StoredProcedureList.GetSpecialities, parameters, out totalRecord).ToList();
                var searchResult = new SpecialityProviderListModel() { SpecialityProviderModel = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecord = totalRecord;
                //searchResult.PageSize = model.PageSize;
                //searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetTaxonomyParentSpecialization(string Prefix)
        {
            var taxonomyList = _facility.ExecWithStoreProcedure<TaxonomyAutoCompleteDropDownViewModel>("spGetTaxonomyHierarchy @Search",
                new SqlParameter("Search", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );
            return Json(taxonomyList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [Route("FacilityProfile/{id?}")]
        public ActionResult FacilityProfile(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Facility");
            }

            var doctor = _facility.GetSingle(x => x.OrganisationId == id);
            //if (doctor != null)
            //   return View(doctor.First());
            return View(doctor);
        }

        [Route("ActiveDeActiveFacility/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActiveFacility(bool flag, int id)
        {
            try
            {
                var facility = _facility.GetById(id);
                facility.IsDeleted = flag;
                _facility.UpdateData(facility);
                _facility.SaveData();
                return Json(new JsonResponse() { Status = 1, Message = $@"The facility has been {(flag ? "deactivated" : "reactivated")} successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Error occured when trying to {(flag ? "deactivate" : "reactivate")} the facility" });
            }
        }

        [HttpPost]
        public JsonResult UpdateStateLicense(StateLicenseProviderModel editStateLicense)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("HealthCareProviderTaxonomyCode", SqlDbType.VarChar) { Value = editStateLicense.HealthCareProviderTaxonomyCode });
            parameters.Add(new SqlParameter("ProviderLicenseNumber", SqlDbType.VarChar) { Value = editStateLicense.ProviderLicenseNumber });
            parameters.Add(new SqlParameter("ProviderLicenseNumberStateCode", SqlDbType.VarChar) { Value = editStateLicense.ProviderLicenseNumberStateCode });
            parameters.Add(new SqlParameter("HealthcareProviderPrimaryTaxonomySwitch", SqlDbType.Bit) { Value = editStateLicense.HealthcareProviderPrimaryTaxonomySwitchString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editStateLicense.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("DocOrgStateLicenses", "DocOrgStateLicense", editStateLicense.DocOrgStateLicense, parameters);
                return Json(new JsonResponse { Status = 1, Message = "State License Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating state license", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateStateLicense(StateLicenseProviderModel createStateLicense)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", SqlDbType.Int) { Value = createStateLicense.ReferenceId });
            parameters.Add(new SqlParameter("HealthCareProviderTaxonomyCode", SqlDbType.VarChar) { Value = createStateLicense.HealthCareProviderTaxonomyCode });
            parameters.Add(new SqlParameter("ProviderLicenseNumber", SqlDbType.VarChar) { Value = createStateLicense.ProviderLicenseNumber });
            parameters.Add(new SqlParameter("ProviderLicenseNumberStateCode", SqlDbType.VarChar) { Value = createStateLicense.ProviderLicenseNumberStateCode });
            parameters.Add(new SqlParameter("HealthcareProviderPrimaryTaxonomySwitch", SqlDbType.Bit) { Value = createStateLicense.HealthcareProviderPrimaryTaxonomySwitchString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createStateLicense.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = User.IsInRole(UserRoles.Admin) ? UserTypes.Admin : UserTypes.Facility });
            parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("DocOrgStateLicenses", parameters);
                return Json(new JsonResponse { Status = 1, Message = "State License created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating state license", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateInsurancePlan(InsurancePlanProviderModel editInsurancePlan)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = editInsurancePlan.InsurancePlanId });
            parameters.Add(new SqlParameter("InsuranceIdentifierId", SqlDbType.VarChar) { Value = editInsurancePlan.InsuranceIdentifierId });
            parameters.Add(new SqlParameter("StateId", SqlDbType.VarChar) { Value = editInsurancePlan.StateId });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editInsurancePlan.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("DocOrgInsurances", "DocOrgInsuranceId", editInsurancePlan.DocOrgInsuranceId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Insurance Plan Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating insurance plan", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateInsurancePlan(InsurancePlanProviderModel createInsurancePlan)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", SqlDbType.Int) { Value = createInsurancePlan.ReferenceId });
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = createInsurancePlan.InsurancePlanId });
            parameters.Add(new SqlParameter("InsuranceIdentifierId", SqlDbType.VarChar) { Value = createInsurancePlan.InsuranceIdentifierId });
            parameters.Add(new SqlParameter("StateId", SqlDbType.VarChar) { Value = createInsurancePlan.StateId });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createInsurancePlan.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            parameters.Add(new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = User.IsInRole(UserRoles.Admin) ? UserTypes.Admin : UserTypes.Facility });
            try
            {
                _facility.ExecuteSqlCommandForInsert("DocOrgInsurances", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Insurance Plan Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating insurance plan", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult UpdateOpeningHour(OpeningHoursProviderModel editOpeningHours)
        //{
        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("CalendarDate", SqlDbType.DateTime) { Value = editOpeningHours.CalendarDate });
        //    parameters.Add(new SqlParameter("WeekDay", SqlDbType.Int) { Value = editOpeningHours.WeekDay });
        //    //parameters.Add(new SqlParameter("SlotDuration", SqlDbType.Int) { Value = editOpeningHours.SlotDuration });
        //    //parameters.Add(new SqlParameter("StartDateTime", SqlDbType.VarChar) { Value = editOpeningHours.StartDateTime });
        //    //parameters.Add(new SqlParameter("EndDateTime", SqlDbType.VarChar) { Value = editOpeningHours.EndDateTime });
        //    //parameters.Add(new SqlParameter("Comments", SqlDbType.VarChar) { Value = editOpeningHours.Comments });
        //    //parameters.Add(new SqlParameter("IsHoliday", SqlDbType.Bit) { Value = editOpeningHours.IsHolidayString == "on" ? 1 : 0 });
        //    //parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editOpeningHours.IsActiveString == "on" ? 1 : 0 });

        //    try
        //    {
        //        _facility.ExecuteSqlCommandForUpdate("OpeningHour", "OpeningHourID", editOpeningHours.OpeningHourID, parameters);
        //        return Json(new JsonResponse { Status = 1, Message = "Opening Hour Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new JsonResponse { Status = 0, Message = "Error occured in updating opening hour", Data = new { } }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpPost]
        //public JsonResult CreateOpeningHour(OpeningHoursProviderModel createOpeningHours)
        //{
        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("OrganizationID", SqlDbType.DateTime) { Value = createOpeningHours.OrganizationID });
        //    parameters.Add(new SqlParameter("CalendarDate", SqlDbType.DateTime) { Value = createOpeningHours.CalendarDate });
        //    parameters.Add(new SqlParameter("WeekDay", SqlDbType.Int) { Value = createOpeningHours.WeekDay });
        //    //parameters.Add(new SqlParameter("SlotDuration", SqlDbType.Int) { Value = createOpeningHours.SlotDuration });
        //    //parameters.Add(new SqlParameter("StartDateTime", SqlDbType.VarChar) { Value = createOpeningHours.StartDateTime });
        //    //parameters.Add(new SqlParameter("EndDateTime", SqlDbType.VarChar) { Value = createOpeningHours.EndDateTime });
        //    //parameters.Add(new SqlParameter("Comments", SqlDbType.VarChar) { Value = createOpeningHours.Comments });
        //    //parameters.Add(new SqlParameter("IsHoliday", SqlDbType.Bit) { Value = createOpeningHours.IsHolidayString == "on" ? 1 : 0 });
        //    //parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createOpeningHours.IsActiveString == "on" ? 1 : 0 });
        //    parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
        //    parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
        //    parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });


        //    try
        //    {
        //        _facility.ExecuteSqlCommandForInsert("OpeningHour", parameters);
        //        return Json(new JsonResponse { Status = 1, Message = "Opening Hour Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new JsonResponse { Status = 0, Message = "Error occured in creating opening hour", Data = new { } }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public JsonResult AddEditOpeningHours(OpeningHoursProviderModel model)
        {
            try
            {

                if (model.OrganizationID > 0)
                {
                    for (int i = 0; i < 7; i++)
                    {

                        int ExecWithStoreProcedure = _facility.ExecuteSQLQuery("spOrgOpeningHour_Create " +
                                "@OpeningHourID," +
                                "@OrganizationID," +
                                "@CalendarDate," +
                                "@WeekDay," +
                                "@SlotDuration," +
                                "@StartDateTime," +
                                "@EndDateTime," +
                                "@IsHoliday," +
                                "@IsActive," +
                                "@IsDeleted," +
                                "@CreatedBy," +
                                "@Comments",
                                              new SqlParameter("OpeningHourID", System.Data.SqlDbType.Int) { Value = model.OpeningHourID > 0 ? model.OpeningHourID : 0 },
                                              new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = model.OrganizationID },
                                              new SqlParameter("CalendarDate", System.Data.SqlDbType.Date) { Value = (object)model.CalendarDate ?? DBNull.Value },
                                              new SqlParameter("WeekDay", System.Data.SqlDbType.Int) { Value = model.DayNo[i] },
                                              new SqlParameter("SlotDuration", System.Data.SqlDbType.Int) { Value = (object)model.SlotDuration[i] ?? DBNull.Value },
                                              new SqlParameter("StartDateTime", System.Data.SqlDbType.NVarChar) { Value = (object)model.StartTime[i] ?? DBNull.Value },
                                              new SqlParameter("EndDateTime", System.Data.SqlDbType.NVarChar) { Value = (object)model.EndTime[i] ?? DBNull.Value },
                                              new SqlParameter("IsHoliday", System.Data.SqlDbType.Bit) { Value = (object)model.IsHoliday[i] ?? DBNull.Value },
                                              new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive[i] ?? DBNull.Value },
                                              new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = (object)false },
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                              new SqlParameter("Comments", System.Data.SqlDbType.NVarChar) { Value = (object)model.Comments[i] ?? DBNull.Value }
                                          );

                        continue;
                    }

                    return Json(new JsonResponse() { Status = 1, Message = "Facility opening hours saved successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Facility Info Not Found! Should be select Facility Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Error occured in saving opening hour information" });
            }

        }

        [HttpPost]
        public JsonResult UpdateReviews(ReviewProviderModel editReviews)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = editReviews.Description });
            parameters.Add(new SqlParameter("Rating", SqlDbType.Int) { Value = editReviews.Rating });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editReviews.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("Review", "ReviewId", editReviews.ReviewId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Reviews Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating review", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateReview(ReviewProviderModel createReview)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createReview.ReferenceId });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = createReview.Description });
            parameters.Add(new SqlParameter("Rating", SqlDbType.Int) { Value = createReview.Rating });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createReview.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = null });
            parameters.Add(new SqlParameter("Doctor_DoctorId", System.Data.SqlDbType.Int) { Value = null });
            parameters.Add(new SqlParameter("SeniorCare_SeniorCareId", System.Data.SqlDbType.Int) { Value = null });
            parameters.Add(new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Facility });
            parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("Review", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Review Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating review", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateSummary(SummaryProviderModel editSummary)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ShortDescription", SqlDbType.VarChar) { Value = editSummary.ShortDescription });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("Organisation", "OrganisationId", editSummary.OrganisationId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Summary Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating summary", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateSpeciality(SpecialityProviderModel editSpeciality)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Taxonomy_Code", SqlDbType.VarChar) { Value = editSpeciality.Taxonomy_Code });
            parameters.Add(new SqlParameter("Specialization", SqlDbType.VarChar) { Value = editSpeciality.Specialization });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = editSpeciality.Description });
            parameters.Add(new SqlParameter("Taxonomy_Level", SqlDbType.VarChar) { Value = editSpeciality.Taxonomy_Level });
            parameters.Add(new SqlParameter("Taxonomy_Type", SqlDbType.VarChar) { Value = editSpeciality.Taxonomy_Type });
            parameters.Add(new SqlParameter("SpecialtyText", SqlDbType.VarChar) { Value = editSpeciality.SpecialtyText });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editSpeciality.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsSpecialty", SqlDbType.Bit) { Value = editSpeciality.IsSpecialtyString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("Taxonomy", "TaxonomyID", editSpeciality.TaxonomyID, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Speciality Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating speciality", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CreateSpeciality(SpecialityProviderModel createSpeciality)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ParentID", SqlDbType.VarChar) { Value = createSpeciality.ParentID });
            parameters.Add(new SqlParameter("Taxonomy_Code", SqlDbType.VarChar) { Value = createSpeciality.Taxonomy_Code });
            parameters.Add(new SqlParameter("Specialization", SqlDbType.VarChar) { Value = createSpeciality.Specialization });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = createSpeciality.Description });
            parameters.Add(new SqlParameter("Taxonomy_Level", SqlDbType.VarChar) { Value = createSpeciality.Taxonomy_Level });
            parameters.Add(new SqlParameter("Taxonomy_Type", SqlDbType.VarChar) { Value = createSpeciality.Taxonomy_Type });
            parameters.Add(new SqlParameter("SpecialtyText", SqlDbType.VarChar) { Value = createSpeciality.SpecialtyText });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createSpeciality.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsSpecialty", SqlDbType.Bit) { Value = createSpeciality.IsSpecialtyString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreateBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("CreateDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("Taxonomy", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Speciality create Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating speciality", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateSocialMedia(OrgSocialMediaListViewModel editSocialMedia)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Facebook", SqlDbType.VarChar) { Value = editSocialMedia.Facebook });
            parameters.Add(new SqlParameter("Twitter", SqlDbType.VarChar) { Value = editSocialMedia.Twitter });
            parameters.Add(new SqlParameter("LinkedIn", SqlDbType.VarChar) { Value = editSocialMedia.LinkedIn });
            parameters.Add(new SqlParameter("Instagram", SqlDbType.VarChar) { Value = editSocialMedia.Instagram });
            parameters.Add(new SqlParameter("Youtube", SqlDbType.VarChar) { Value = editSocialMedia.Youtube });
            parameters.Add(new SqlParameter("Pinterest", System.Data.SqlDbType.VarChar) { Value = editSocialMedia.Pinterest });
            parameters.Add(new SqlParameter("Tumblr", System.Data.SqlDbType.VarChar) { Value = editSocialMedia.Tumblr });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editSocialMedia.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("SocialMedia", "SocialMediaId", editSocialMedia.SocialMediaId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Social Media Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating social media", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateSocialMedia(OrgSocialMediaListViewModel createSocialMedia)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createSocialMedia.OrganisationId });
            parameters.Add(new SqlParameter("Facebook", SqlDbType.VarChar) { Value = createSocialMedia.Facebook });
            parameters.Add(new SqlParameter("Twitter", SqlDbType.VarChar) { Value = createSocialMedia.Twitter });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createSocialMedia.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("LinkedIn", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.LinkedIn });
            parameters.Add(new SqlParameter("Instagram", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.Instagram });
            parameters.Add(new SqlParameter("Youtube", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.Youtube });
            parameters.Add(new SqlParameter("Pinterest", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.Pinterest });
            parameters.Add(new SqlParameter("Tumblr", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.Tumblr });
            parameters.Add(new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = User.IsInRole(UserRoles.Admin) ? UserTypes.Admin : UserTypes.Facility });
            parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("SocialMedia", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Social Media Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating social media", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateTaxonomy(OrganisationSpecialityTaxonomyViewModel editTaxonomy)
        {
            string description = "";
            if (!string.IsNullOrEmpty(editTaxonomy.Description))
            {
                description = System.Uri.UnescapeDataString(editTaxonomy.Description);
                editTaxonomy.Description = description;
            }
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Taxonomy_Code", SqlDbType.VarChar) { Value = editTaxonomy.Taxonomy_Code });
            parameters.Add(new SqlParameter("Specialization", SqlDbType.VarChar) { Value = editTaxonomy.Specialization });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = editTaxonomy.Description });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editTaxonomy.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("Taxonomy", "TaxonomyID", editTaxonomy.TaxonomyID, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Taxonomy Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating taxonomy", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateTaxonomy(OrganisationSpecialityTaxonomyViewModel createTaxonomy)
        {
            string description = "";
            if (!string.IsNullOrEmpty(createTaxonomy.Description))
            {
                description = System.Uri.UnescapeDataString(createTaxonomy.Description);
                createTaxonomy.Description = description;
            }

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ParentID", System.Data.SqlDbType.Int) { Value = createTaxonomy.ReferenceID });
            parameters.Add(new SqlParameter("Taxonomy_Code", SqlDbType.VarChar) { Value = createTaxonomy.Taxonomy_Code });
            parameters.Add(new SqlParameter("Specialization", SqlDbType.VarChar) { Value = createTaxonomy.Specialization });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = createTaxonomy.Description });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createTaxonomy.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreateBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            //parameters.Add(new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 });
            parameters.Add(new SqlParameter("CreateDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("Taxonomy", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Taxonomy Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating taxonomy", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateAddress(OrganisationAddressListViewModel editAddress)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("AddressTypeId", SqlDbType.Int) { Value = editAddress.AddressTypeId });
            parameters.Add(new SqlParameter("Address1", SqlDbType.VarChar) { Value = editAddress.Address1 });
            parameters.Add(new SqlParameter("Address2", SqlDbType.VarChar) { Value = editAddress.Address2 });
            parameters.Add(new SqlParameter("CityStateZipCodeID", SqlDbType.Int) { Value = editAddress.CityStateZipCodeID });
            parameters.Add(new SqlParameter("UpdatedDate", SqlDbType.DateTime) { Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") });
            parameters.Add(new SqlParameter("Phone", SqlDbType.VarChar) { Value = editAddress.Phone });
            parameters.Add(new SqlParameter("Fax", SqlDbType.NVarChar) { Value = editAddress.Fax });
            parameters.Add(new SqlParameter("Email", SqlDbType.NVarChar) { Value = editAddress.Email });
            parameters.Add(new SqlParameter("WebSite", SqlDbType.NVarChar) { Value = editAddress.WebSite });
            parameters.Add(new SqlParameter("Lat", SqlDbType.Decimal) { Value = editAddress.Lat });
            parameters.Add(new SqlParameter("Lon", SqlDbType.Decimal) { Value = editAddress.Lon });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editAddress.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("Address", "AddressId", editAddress.AddressId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Address Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating address", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateAddress(OrganisationAddressListViewModel createAddress)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createAddress.ReferenceId });
            parameters.Add(new SqlParameter("AddressTypeID", SqlDbType.Int) { Value = createAddress.AddressTypeId });
            parameters.Add(new SqlParameter("Address1", SqlDbType.VarChar) { Value = createAddress.Address1 });
            parameters.Add(new SqlParameter("Address2", SqlDbType.VarChar) { Value = createAddress.Address2 });
            parameters.Add(new SqlParameter("CityStateZipCodeID", SqlDbType.Int) { Value = createAddress.CityStateZipCodeID });
            parameters.Add(new SqlParameter("Phone", SqlDbType.VarChar) { Value = createAddress.Phone });
            parameters.Add(new SqlParameter("Fax", SqlDbType.NVarChar) { Value = createAddress.Fax });
            parameters.Add(new SqlParameter("Email", SqlDbType.NVarChar) { Value = createAddress.Email });
            parameters.Add(new SqlParameter("WebSite", SqlDbType.NVarChar) { Value = createAddress.WebSite });
            parameters.Add(new SqlParameter("Lat", SqlDbType.Decimal) { Value = createAddress.Lat });
            parameters.Add(new SqlParameter("Lon", SqlDbType.Decimal) { Value = createAddress.Lon });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createAddress.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Facility });
            parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("Address", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Address Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating address", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateBooking(OrganisationSlotList editBooking)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SlotDate", SqlDbType.VarChar) { Value = editBooking.SlotDate });
            parameters.Add(new SqlParameter("SlotTime", SqlDbType.VarChar) { Value = editBooking.SlotTime });
            parameters.Add(new SqlParameter("AddressId", SqlDbType.Int) { Value = editBooking.AddressId });
            parameters.Add(new SqlParameter("BookedFor", SqlDbType.Int) { Value = editBooking.BookedFor });
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = editBooking.InsurancePlanId });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = editBooking.Description });
            parameters.Add(new SqlParameter("IsBooked", SqlDbType.Bit) { Value = editBooking.IsBookedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsEmailReminder", SqlDbType.Bit) { Value = editBooking.IsEmailReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsTextReminder", SqlDbType.Bit) { Value = editBooking.IsTextReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsInsuranceChanged", SqlDbType.Bit) { Value = editBooking.IsInsuranceChangedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editBooking.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("Slot", "SlotId", editBooking.SlotId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Booking Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating booking", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateBooking(OrganisationSlotList createSlot)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createSlot.ReferenceId });
            parameters.Add(new SqlParameter("SlotDate", SqlDbType.VarChar) { Value = createSlot.SlotDate });
            parameters.Add(new SqlParameter("SlotTime", SqlDbType.VarChar) { Value = createSlot.SlotTime });
            parameters.Add(new SqlParameter("AddressId", SqlDbType.Int) { Value = createSlot.AddressId });
            parameters.Add(new SqlParameter("BookedFor", SqlDbType.Int) { Value = createSlot.BookedFor });
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = createSlot.InsurancePlanId });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = createSlot.Description });
            parameters.Add(new SqlParameter("IsBooked", SqlDbType.Bit) { Value = createSlot.IsBookedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsEmailReminder", SqlDbType.Bit) { Value = createSlot.IsEmailReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsTextReminder", SqlDbType.Bit) { Value = createSlot.IsTextReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsInsuranceChanged", SqlDbType.Bit) { Value = createSlot.IsInsuranceChangedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createSlot.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("Slot", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Booking Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating booking", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateSwitch(SwitchUpdateViewModel switchUpdateViewModel)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter(switchUpdateViewModel.FieldToUpdateName, SqlDbType.VarChar) { Value = switchUpdateViewModel.FieldToUpdateValue });
            try
            {
                _facility.ExecuteSqlCommandForUpdate(switchUpdateViewModel.TableName, switchUpdateViewModel.PrimaryKeyName, switchUpdateViewModel.PrimaryKeyValue, parameters);
                if (switchUpdateViewModel.FieldToUpdateName.ToLower().Contains("deleted"))
                    return Json(new JsonResponse { Status = 1, Message = "Record deleted successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new JsonResponse { Status = 1, Message = switchUpdateViewModel.FieldToUpdateName.Replace("Is", "") + " Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (switchUpdateViewModel.FieldToUpdateName.ToLower().Contains("deleted"))
                    return Json(new JsonResponse { Status = 0, Message = "Error occured in deleting record", Data = new { } }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new JsonResponse { Status = 0, Message = "Error occured in updating " + switchUpdateViewModel.FieldToUpdateName.ToLower().Replace("is", ""), Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAddressTypes()
        {
            var addressTypes = _facility.ExecWithStoreProcedure<AddressTypeDropDownViewModel>("spAddressTypeDropDown");
            return Json(new JsonResponse { Status = 1, Message = "", Data = addressTypes }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetZipCode(string Prefix)
        {
            var zipcodes = _facility.ExecWithStoreProcedure<CityStateZipCodeViewModel>("spGetCityStateZipCodesAutoComplete @ZipCode",
                new SqlParameter("ZipCode", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(zipcodes.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCityByZipCode(string Prefix, string ZipCode)
        {
            var cityList = _facility.ExecWithStoreProcedure<CityStateInfoByZipCodeViewModel>("spGetCityStateCityByZipCodeAutoComplete @ZipCode, @City",
                new SqlParameter("ZipCode", System.Data.SqlDbType.VarChar) { Value = ZipCode },
                new SqlParameter("City", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(cityList.ToList(), JsonRequestBehavior.AllowGet);
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

        public string GetAddressById(int AddressId)
        {
            var addressList = _facility.SQLQuery<DrpAddress>("select Address1, Address2 from Address where AddressId = " + AddressId).ToList();
            var address = new DrpAddress();
            if (addressList.Count > 0)
                address = addressList[0];
            return address.Address1 + (!string.IsNullOrEmpty(address.Address1) ? ", " : "") + address.Address2;
        }

        #endregion

        #region Facility Profile
        [HttpGet, Route("FacilityBasicInformation/{id?}")]
        public ActionResult BasicInformation(int id = 0)
        {
            ViewBag.Flag = Request.QueryString["flag"];
            ViewBag.FacilityTypeList = _facilityType.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.Org_Type_Name,
                Value = x.OrganizationTypeID.ToString()
            }).ToList();

            if (User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Facility))
                _facilityId = id;
            else
                _facilityId = GetFacilityId();

            if (_facilityId > 0)
            {
                var facility = _facility.GetById(_facilityId);
                return View(facility);
            }
            else
            {
                return View(new Organisation());
            }
        }

        [HttpGet, Route("GetProfileLogo/{id?}")]
        public ActionResult GetProfileLogo(int id = 0)
        {
            if (User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Facility))
                _facilityId = id;
            else
                _facilityId = GetFacilityId();

            if (_facilityId > 0)
            {
                var facility = _facility.GetById(_facilityId);
                return Json(new JsonResponse { Status = 1, Data=facility.LogoFilePath, Message = "Facility basic information saved successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in getting profile image" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("FacilityBasicInformation/{isActiveString}/{enabledBooking}")]
        [HttpPost]
        public JsonResult BasicInformation(Organisation model, string isActiveString, string enabledBooking, HttpPostedFileBase Image1)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    string imagePath = "";

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/FacilitySiteImages/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/FacilitySiteImages"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "Facility-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/FacilitySiteImages"), newImageName);
                        Image1.SaveAs(path);

                        imagePath = newImageName;
                    }

                    if (model.OrganisationId > 0 && imagePath == "")
                    {
                        imagePath = model.LogoFilePath;
                    }

                    string strShortDescription = "";
                    string strLongDescription = "";
                    if (!string.IsNullOrEmpty(model.ShortDescription))
                    {
                        strShortDescription = System.Uri.UnescapeDataString(model.ShortDescription);
                        model.ShortDescription = Regex.Replace(strShortDescription, "<.*?>", String.Empty);
                    }
                    if (!string.IsNullOrEmpty(model.LongDescription))
                    {
                        strLongDescription = System.Uri.UnescapeDataString(model.LongDescription);
                        model.LongDescription = Regex.Replace(strLongDescription, "<.*?>", String.Empty);
                    }

                    model.IsActive = isActiveString.ToLower() == "on" ? true : false;
                    model.EnabledBooking = enabledBooking.ToLower() == "on" ? true : false;
                    if (model.OrganisationId > 0)
                    {
                        model.UpdatedDate = DateTime.Now;
                        var facility = _facility.GetById(model.OrganisationId);

                        #region Facility Basic
                        facility.AliasBusinessName = model.AliasBusinessName;
                        facility.AuthorizedOfficialFirstName = model.AuthorizedOfficialFirstName;
                        facility.AuthorizedOfficialLastName = model.AuthorizedOfficialLastName;
                        facility.AuthorizedOfficialNamePrefix = model.AuthorizedOfficialNamePrefix;
                        facility.AuthorizedOfficialTelephoneNumber = model.AuthorizedOfficialTelephoneNumber;
                        facility.AuthorizedOfficialTitleOrPosition = model.AuthorizedOfficialTitleOrPosition;
                        facility.AuthorisedOfficialCredential = model.AuthorisedOfficialCredential;
                        facility.EnumerationDate = model.EnumerationDate;
                        facility.NPI = model.NPI;
                        facility.OrganisationName = model.OrganisationName;
                        facility.OrganisationSubpart = model.OrganisationSubpart;
                        facility.OrganizatonEIN = model.OrganizatonEIN;
                        facility.ShortDescription = model.ShortDescription;
                        facility.LongDescription = model.LongDescription;
                        facility.Status = !string.IsNullOrEmpty(facility.Status) ? facility.Status : " ";
                        facility.LogoFilePath = imagePath;
                        facility.IsActive = model.IsActive;
                        facility.EnabledBooking = model.EnabledBooking;
                        _facility.UpdateData(facility);
                        #endregion
                    }
                    else
                    {
                        if (User.IsInRole(UserRoles.Admin))
                            model.UserTypeID = UserTypes.Admin;
                        else
                            model.UserTypeID = UserTypes.Facility;
                        model.Status = " ";
                        model.UserId = Convert.ToInt32(User.Identity.GetUserId());
                         model.CreatedBy= Convert.ToInt32(User.Identity.GetUserId()); //sonu
                        model.CreatedDate = DateTime.Now;
                        model.OrganizationTypeID = OrganisationTypes.Facility;
                        model.LogoFilePath = imagePath;
                        _facility.InsertData(model);
                    }

                    _facility.SaveData();
                    txscope.Complete();
                    return Json(new JsonResponse { Status = 1, Message = "Facility basic information saved successfully.", Data = model.OrganisationId }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "BasicInformation-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public PartialViewResult FacilityProfileCreate(int? id)
        {
            Organisation model = new Organisation();
            return PartialView(@"/Views/Facility/Partial/_FacilityProfile.cshtml", model);
        }

        public ActionResult GetOrgId()
        {
            try
            {
                int userId = User.Identity.GetUserId<int>();
                //var orgId = _facility.SQLQuery<int>("select OrganisationId from Organisation where UserId = " + userId.ToString() + " and IsActive = 1 and IsDeleted = 0 order by createdby desc").FirstOrDefault();
                var orgId = _repo.Find<Organisation>(x => x.UserId == userId && x.IsActive == true && x.IsDeleted == false);
                
                return Json(new JsonResponse { Status = 1, Data = orgId.OrganisationId > 0 ? orgId.OrganisationId : 0 }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Controller Common
        private int GetFacilityId()
        {
            int userId = User.Identity.GetUserId<int>();
            var facility = _facility.GetSingle(x => x.UserId == userId);
            if (facility != null)
            {
                return facility.OrganisationId;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Amenity Options
        // GET: StateLicense 
        public ActionResult AmenityOptions(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["FacilityData"] = "Yes";
                ViewBag.FacilityID = id;
                Session["FacilityID"] = id;
            }
            else
            {
                if (User.IsInRole("Facility"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    var facilityInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1006 && x.IsDeleted == false && x.UserId == userId);
                    TempData["FacilityData"] = "Yes";
                    ViewBag.FacilityID = facilityInfo.OrganisationId;
                    Session["FacilityID"] = facilityInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetFacilityAmenityOptionsList/{flag}/{id}")]
        public ActionResult GetFacilityAmenityOptionsList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allFacility = _repo.ExecWithStoreProcedure<OrganizationAmenityOptionListViewModel>("spAllOrganizationAmenityOption_Get @Search, @UserTypeID, @OrganizationID, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 4 },
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1006 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allFacility
                .Select(x => new OrganizationAmenityOptionListViewModel()
                {
                    OrganizationAmenityOptionID = x.OrganizationAmenityOptionID,
                    OrganizationID = x.OrganizationTypeID,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeID = x.OrganizationTypeID,
                    Name = x.Name,
                    Description = x.Description,
                    IsOption = x.IsOption,
                    UpdatedDateAsString = x.UpdatedDate.HasValue ? x.UpdatedDate.Value.ToDefaultFormate() : string.Empty,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    ImagePath = x.ImagePath,
                    TotalRecordCount = x.TotalRecordCount
                }).ToList();

            int TotalRecordCount = 0;
            
            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount.HasValue ? result[0].TotalRecordCount.Value : 0;

            ViewBag.FacilityID = id;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        //---- Get AmenityOptions Details
        public PartialViewResult FacilityAmenityOptions(int? id)
        {
            ViewBag.FacilityID = Session["FacilityID"];
            if (id > 0)
            {
                ViewBag.ID = id;

                var orgFeaturedInfo = _repo.ExecWithStoreProcedure<OrganizationAmenityOptionListViewModel>("spAllOrganizationAmenityOption_GetById @OrganizationAmenityOptionID",
                    new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrganizationAmenityOptionsUpdateViewModel
                    {
                        OrganizationAmenityOptionID = x.OrganizationAmenityOptionID.Value,
                        OrganizationID = x.OrganizationID.Value,
                        OrganisationName = x.OrganisationName,
                        OrganizationTypeID = x.OrganizationTypeID.Value,
                        Name = x.Name,
                        Description = x.Description,
                        IsOption = x.IsOption,
                        ImagePath = x.ImagePath,
                        UpdatedDate = x.UpdatedDate.HasValue ? x.UpdatedDate.Value.ToDefaultFormate() : string.Empty,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted
                    }).First();

                return PartialView(@"/Views/Facility/Partial/_FacilityAmenityOptions.cshtml", orgFeaturedInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganizationAmenityOptionsUpdateViewModel();
                return PartialView(@"/Views/Facility/Partial/_FacilityAmenityOptions.cshtml", orgAddressInfo);
            }
        }
        public PartialViewResult FacilityViewAmenityOptions(int? id)
        {
            ViewBag.FacilityID = Session["FacilityID"];
            if (id > 0)
            {
                ViewBag.ID = id;

                var orgFeaturedInfo = _repo.ExecWithStoreProcedure<OrganizationAmenityOptionsViewModel>("spAllOrganizationAmenityOption_GetById @OrganizationAmenityOptionID",
                    new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrganizationAmenityOptionsUpdateViewModel
                    {
                        OrganizationAmenityOptionID = x.OrganizationAmenityOptionID,
                        OrganizationID = x.OrganizationID,
                        OrganisationName = x.OrganisationName,
                        OrganizationTypeID = x.OrganizationTypeID,
                        Name = x.Name,
                        Description = x.Description,
                        IsOption = x.IsOption,
                        ImagePath = x.ImagePath,
                        UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted
                    }).First();
                orgFeaturedInfo.IsViewMode = true;

                return PartialView(@"/Views/Facility/Partial/_FacilityAmenityOptions.cshtml", orgFeaturedInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganizationAmenityOptionsUpdateViewModel();
                orgAddressInfo.IsViewMode = true;
                return PartialView(@"/Views/Facility/Partial/_FacilityAmenityOptions.cshtml", orgAddressInfo);
            }
        }

        //-- Delete Facility Amenity Option
        [Route("DeleteFacilityAmenityOptions/{id}")]
        [HttpPost]
        public JsonResult DeleteFacilityAmenityOptions(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAllOrganizationAmenityOption_Delete @OrganizationAmenityOptionID, @ModifiedBy",
                    new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Facility Amenity Option has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Facility Amenity Option deleted Error.." });
            }
        }

        [HttpPost, Route("AddEditFacilityAmenityOptions"), ValidateAntiForgeryToken]
        public JsonResult AddEditFacilityAmenityOptions(OrganizationAmenityOptionsUpdateViewModel model, HttpPostedFileBase Image1)
        {

            try
            {
                if (model.OrganizationID > 0 && model.Name != null)
                {
                    string imagePath = "";

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/AmenityOptions/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/AmenityOptions"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "AmenityOpt-Facility-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/AmenityOptions"), newImageName);
                        Image1.SaveAs(path);

                        imagePath = newImageName;
                    }

                    if (model.OrganizationAmenityOptionID > 0 && imagePath == "")
                    {
                        imagePath = model.ImagePath;
                    }


                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAllOrganizationAmenityOption_Create " +
                            "@OrganizationAmenityOptionID," +
                            "@OrganizationID," +
                            "@Name," +
                            "@Description," +
                            "@ImagePath," +
                            "@IsOption," +
                            "@CreatedBy," +
                            "@IsActive",
                                          new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = model.OrganizationAmenityOptionID > 0 ? model.OrganizationAmenityOptionID : 0 },
                                          new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = model.OrganizationID },
                                          new SqlParameter("Name", System.Data.SqlDbType.NVarChar) { Value = (object)model.Name ?? DBNull.Value },
                                          new SqlParameter("Description", System.Data.SqlDbType.NVarChar) { Value = (object)model.Description ?? DBNull.Value },
                                          new SqlParameter("ImagePath", System.Data.SqlDbType.NVarChar) { Value = (object)imagePath ?? DBNull.Value },
                                          new SqlParameter("IsOption", System.Data.SqlDbType.Bit) { Value = model.IsOption },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive }
                                      );





                    return Json(new JsonResponse() { Status = 1, Message = "Facility Amenity Option info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name, Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Facility Amenity Option info saving Error.." });
            }

        }

        #endregion

        [HttpGet, Route("GetFacilityDetail/")]
        public void GetFacilityDetail()
        {
            try
            {
                int userId = User.Identity.GetUserId<int>();
                if (userId > 0)
                {
                    var loggedFacultyDetail = _facility.GetById(userId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost, Route("BookFacultyTimeSlot/")]
        //public JsonResult BookFacultyTimeSlot(appSlotViewModel slotBook)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var slotDetails = _slot.GetSingle(x => x.SlotId == slotBook.SlotId);

        //            slotDetails.IsBooked = true;
        //            slotDetails.PatientUserId = slotBook.PatientUserId > 0 ? slotBook.PatientUserId : 0;

        //            _slot.UpdateData(slotDetails);
        //            _slot.SaveData();
        //            txscope.Complete();
        //            txscope.Dispose();
        //            return Json(new JsonResponse() { Status = 200, Message = "Appointment is booked successfully", Data = "A" }, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            throw ex;
        //        }
        //    }
        //}


        [HttpPost, Route("SaveFacilityUser/")]
        public JsonResult SaveFacilityUser(UserAndAddress userAndAddress)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
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
                            //applicationUserId = createUser.Id;
                            //patient.UserId = createUser.Id;
                            //patient.IsActive = true;
                            //_patient.InsertData(patient);
                            //_patient.SaveData();

                            //var patientDetails = _patient.GetSingle(x => x.UserId == applicationUserId)?.PatientId;

                            //if (patientDetails != null)
                            //{
                            //    address.PatientId = patientDetails;
                            //    address.Address1 = userAndAddress.Address;
                            //    address.CityId = userAndAddress.CityId;
                            //    address.StateId = userAndAddress.StateId;
                            //    address.ZipCode = userAndAddress.Zipcode;
                            //    _address.InsertData(address);
                            //    _address.SaveData();
                            //}
                        }

                        txscope.Complete();
                    }
                    else
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse() { Status = 0, Message = "Email already exist.", Data = "" }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new JsonResponse() { Status = 200, Message = "Data Saved Successfully", Data = applicationUserId }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "Patent-Book-POST");
                    return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #region Cost
        public ActionResult Cost(int id)
        {
            ViewBag.costId = id;
            Session["ReferenceID"] = id;
            return View();
        }

        [Route("Facility/GetSeniorCareCostList/{id?}")]
        public ActionResult GetSeniorCareCostList(JQueryDataTableParamModel param, int id = 0)
        {
            id = Convert.ToInt32(Session["ReferenceID"]);
            var allCosts = _repo.ExecWithStoreProcedure<CostViewModel>("spCost_GetList @Search, @UserTypeID, @OrganizationID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.Facility },
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allCosts
                .Select(x => new CostViewModel()
                {
                    CostID = x.CostID,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description,
                    CreatedDateString = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDateString = x.UpdatedDate.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    TotalRecordCount = x.TotalRecordCount,
                    UserTypeID=x.UserTypeID
                }).Where(x=>x.UserTypeID== (int)UserTypes.Facility).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        //--- Add/Edit Cost
        [HttpPost]
        //[HttpPost, Route("SeniorCare/AddEditSeniorCareCost"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareCost(CostViewModel model)
        {
            try
            {
                if (Convert.ToInt32(Session["ReferenceID"]) > 0)
                {
                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spCost_Create " +
                            "@CostID," +
                            "@ReferenceId," +
                            "@UserTypeID," +
                            "@Name," +
                            "@Description," +
                            "@Price," +
                            "@IsActive," +
                            "@IsDeleted," +
                            "@CreatedBy",
                                          new SqlParameter("CostID", System.Data.SqlDbType.BigInt) { Value = model.CostID > 0 ? model.CostID : 0 },
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = Convert.ToInt32(Session["ReferenceID"]) },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.Facility },
                                          new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = (object)model.Name ?? DBNull.Value },
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (object)model.Description ?? DBNull.Value },
                                          new SqlParameter("Price", System.Data.SqlDbType.Int) { Value = (object)model.Price ?? DBNull.Value },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive ?? DBNull.Value },
                                          new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() }
                                      );





                    return Json(new JsonResponse() { Status = 1, Message = "Cost info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Cost info saving Error.." });
            }

        }

        [Route("Facility/DeleteSeniorCareCost/{id}")]
        [HttpPost]
        public JsonResult DeleteSeniorCareCost(int id)
        {
            try
            {

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spCost_Remove @CostId, @ModifiedBy",
                    new SqlParameter("CostId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Cost has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Cost deleted Error.." });
            }
        }

        #endregion


    }
}
