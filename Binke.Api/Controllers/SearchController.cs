using Binke.Api.Utility;
using Binke.Api.ViewModels;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Ajax.Utilities;

namespace Binke.Api.Controllers
{
    public class SearchController : BaseApiController
    {
        private readonly IDoctorService _doctor;
        private readonly IFacilityService _facility;
        //private readonly IPharmacyService _pharmacy;
        //private readonly ISeniorCareService _seniorCare;
        private readonly ISpecialityService _speciality;
        private readonly IFeaturedService _featuredService;

        public SearchController(IDoctorService doctor, IFacilityService facility, ISpecialityService speciality, IFeaturedService featuredService)
        {
            _doctor = doctor;
            _facility = facility;
            _speciality = speciality;
            _featuredService = featuredService;
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

        #region Doctor
        [HttpPost]
        [Route("api/Home/SearchDoctors")]
        public HttpResponseMessage SearchDoctorDetails(SearchParameterModel model)
        {
            model.PageIndex = model.PageIndex > 0 ? model.PageSize : 1;
            model.PageSize = model.PageSize > 0 ? model.PageSize : 10;

            //SearchParameterModel model = new SearchParameterModel();
            //model.Latitude = HttpContext.Current.Request.Params["Latitude"];
            //model.Longitude = HttpContext.Current.Request.Params["Longitude"];
            //model.ANP = Convert.ToBoolean(HttpContext.Current.Request.Params["ANP"]);
            //model.NTPA = Convert.ToBoolean(HttpContext.Current.Request.Params["NTPA"]);
            //model.PrimaryCare = Convert.ToBoolean(HttpContext.Current.Request.Params["PrimaryCare"]);
            //model.Specialties = Convert.ToInt32(HttpContext.Current.Request.Params["Specialties"]);
            //model.Language = Convert.ToInt32(HttpContext.Current.Request.Params["Language"]);
            //model.Search = HttpContext.Current.Request.Params["Search"];
            //model.PageIndex = Convert.ToInt32(HttpContext.Current.Request.Params["PageIndex"]);
            //model.PageSize = Convert.ToInt32(HttpContext.Current.Request.Params["PageSize"]);

            try
            {

                string ipString = GetIPString();

                var ipInfo = new IpInfo();
                var latOfUser = string.Empty;
                var longOfUser = string.Empty;

                if (!string.IsNullOrEmpty(ipString))
                {
                    try
                    {
                        ipInfo = JsonConvert.DeserializeObject<IpInfo>(ipString);
                        if (!string.IsNullOrEmpty(ipInfo.Loc))
                        {
                            latOfUser = ipInfo.Loc.Split(',')[0];
                            longOfUser = ipInfo.Loc.Split(',')[1];
                        }
                    }
                    catch
                    {
                        latOfUser = null;
                        longOfUser = null;
                    }
                }
                else
                {
                    latOfUser = null;
                    longOfUser = null;
                }
                decimal lat2 = 0;
                decimal long2 = 0;

                if (!string.IsNullOrWhiteSpace(model.Latitude))
                {
                    decimal.TryParse(model.Latitude, out lat2);
                }
                else
                {
                    decimal.TryParse(latOfUser, out lat2);
                }

                if (!string.IsNullOrWhiteSpace(model.Longitude))
                {
                    decimal.TryParse(model.Longitude, out long2);
                }
                else
                {
                    decimal.TryParse(longOfUser, out long2);
                }
                var doctorIds = new List<int>();
                var paras = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.Search??""}
                };

                //decimal.TryParse(latOfUser, out lat2);
                //decimal.TryParse(longOfUser, out long2);
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

                doctorIdsWithDistanceList.AddRange(_doctor.GetDoctorIdByNameOrAddress(model.Search, model.NTPA, model.ANP, model.PrimaryCare, model.DistanceType, lat2, long2));
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

                model.Latitude = string.IsNullOrEmpty(latOfUser) ? model.Latitude : latOfUser;
                model.Longitude = string.IsNullOrEmpty(longOfUser) ? model.Longitude : longOfUser;
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
                            item.InsuranceCount = doctorDetails.First(x => x.DoctorId == item.DoctorId).InsuaranceCount;
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
                            //item.Facility = facility.Where(x => tempFacility.Contains(x.Id))
                            //    .Select(x => new KeyValueModel { Key = x.Id, Value = x.Value }).ToList();
                            item.Facility = _facility.GetAll(x => x.IsActive && !x.IsDeleted && tempFacility.Contains(x.OrganisationId))
                           .Select(x => new OrgFacitiesInfo { Id = x.OrganisationId, Name = x.OrganisationName, OrgNpi = x.NPI, OrgType = x.OrganizationTypeID }).ToList();
                        }
                    }
                }
                var searchResult = new Doctyme.Model.ViewModels.SearchDoctorResult() { listDoctors = allslotList };
                searchResult.TotalRecord = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex + 1;

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = searchResult });
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
                new SqlParameter("@Distance",SqlDbType.Int) {Value = searchModel.DistanceType == null ? 0 : Convert.ToInt32(searchModel.DistanceType)},
                new SqlParameter("@DistanceSearch",SqlDbType.NVarChar) {Value = searchModel.SearchBox ?? ""},
                new SqlParameter("@Latitude",SqlDbType.NVarChar) {Value = searchModel.Latitude ?? "0"},
                new SqlParameter("@Longitude",SqlDbType.NVarChar) {Value = searchModel.Longitude ?? "0"},
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
        #endregion

        #region Pharmacy
        [HttpPost]
        [Route("api/Home/SearchPharmacy")]
        public HttpResponseMessage _SearchPharmacy(SearchParamModel model)
        {
            //SearchParamModel model = new SearchParamModel();
            //model = GetSearchParamModel(model);

            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.Pharmacy);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = searchResult });
        }

        private SearchParamModel GetSearchParamModel()
        {
            SearchParamModel model = new SearchParamModel();
            model.Latitude = HttpContext.Current.Request.Params["Latitude"];
            model.Longitude = HttpContext.Current.Request.Params["Longitude"];
            model.DistanceType = HttpContext.Current.Request.Params["DistanceType"];
            model.SearchBox = HttpContext.Current.Request.Params["SearchBox"];
            model.PageIndex = Convert.ToInt32(HttpContext.Current.Request.Params["PageIndex"]);
            model.PageSize = Convert.ToInt32(HttpContext.Current.Request.Params["PageSize"]);
            return model;
        }

        private OrganizationProviderListModel GetOrganizationSearchResult(SearchParamModel model, int organizationTypeId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;

                string ipString = GetIPString();

                var ipInfo = new IpInfo();
                var latOfUser = string.Empty;
                var longOfUser = string.Empty;

                if (!string.IsNullOrEmpty(ipString))
                {
                    try
                    {
                        ipInfo = JsonConvert.DeserializeObject<IpInfo>(ipString);
                        if (!string.IsNullOrEmpty(ipInfo.Loc))
                        {
                            latOfUser = ipInfo.Loc.Split(',')[0];
                            longOfUser = ipInfo.Loc.Split(',')[1];
                        }
                    }
                    catch
                    {
                        latOfUser = null;
                        longOfUser = null;
                    }
                }
                else
                {
                    latOfUser = null;
                    longOfUser = null;
                }




                var parameters = new List<SqlParameter>();
                //var parameters = new List<SqlParameter>()
                //{
                //    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                //    new SqlParameter("@Distance",SqlDbType.Int) {Value = model.DistanceType == null ? 0 : Convert.ToInt32(model.DistanceType)},
                //    new SqlParameter("@DistanceSearch",SqlDbType.NVarChar) {Value = model.SearchBox ?? ""},
                //    new SqlParameter("@Latitude",SqlDbType.NVarChar) {Value = model.Latitude ?? "0"},
                //    new SqlParameter("@Longitude",SqlDbType.NVarChar) {Value = model.Longitude ?? "0"},
                //    new SqlParameter("@OrganisationId",SqlDbType.Int) {Value = model.Id},
                //    new SqlParameter("@OrganizationTypeID",SqlDbType.Int) {Value = organizationTypeId},
                //    new SqlParameter("@Sorting",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                //    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex-1},
                //    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                //};

                var paras = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox??""},
                    new SqlParameter("@OrganizationTypeID",SqlDbType.Int) {Value = organizationTypeId}
                };
                var featureFacilityIds = _doctor.GetFeaturedFacilityIdsBySearchText(StoredProcedureList.GetFeaturedFacilityIds, paras);

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

                //decimal lat2 = 0;
                //decimal.TryParse(latOfUser, out lat2);
                //decimal long2 = 0;
                //decimal.TryParse(longOfUser, out long2);
                //model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                //model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;
                //var parameters = new List<SqlParameter>();
                //var paras = new List<SqlParameter>()
                //{
                //    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox??""},
                //    new SqlParameter("@OrganizationTypeID",SqlDbType.Int) {Value = organizationTypeId}
                //};
                //var featureFacilityIds = _doctor.GetFeaturedFacilityIdsBySearchText(StoredProcedureList.GetFeaturedFacilityIds, paras);
                var pageIndex = model.PageIndex == 0 ? 0 : model.PageIndex - 1;
                var lstOrgIds = new List<int>();
                if (featureFacilityIds != null && featureFacilityIds.Count > 0)
                    if (featureFacilityIds.Count > (pageIndex * model.PageSize))
                        lstOrgIds.AddRange(featureFacilityIds.Skip(pageIndex * model.PageSize).Take(model.PageSize).Select(m => m.ReferenceId).ToList());

                int totalRecord = 0;
                model.Latitude = string.IsNullOrEmpty(latOfUser) ? model.Latitude : latOfUser;
                model.Longitude = string.IsNullOrEmpty(longOfUser) ? model.Longitude : longOfUser;
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
                    List<OrganisationsWithDistance> orgIdsWithDistance = null;
                    if (string.IsNullOrEmpty(model.locationSearch))
                    {
                        orgIdsWithDistance = _featuredService.GetOrganisationIdsFromSearchText(model.SearchBox, organizationTypeId, Convert.ToDecimal(model.Latitude), Convert.ToDecimal(model.Longitude),
                        model.DistanceType == null ? 0 : Convert.ToInt32(model.DistanceType), skip, take, out totalRecord);
                    }
                    else
                    {
                        orgIdsWithDistance = _featuredService.GetOrganisationIdsFromSearchTextWithZipcode(model.SearchBox, organizationTypeId, Convert.ToDecimal(model.Latitude), Convert.ToDecimal(model.Longitude),
                        model.DistanceType == null ? 0 : Convert.ToInt32(model.DistanceType), skip, take, model.locationSearch, out totalRecord);
                    }
                    lstOrgIds.AddRange(orgIdsWithDistance.Select(m => m.OrganisationId).ToList());
                }

                totalRecord = totalRecord + featureFacilityIds.Count;
                parameters.Add(new SqlParameter("@OrganisationIds", SqlDbType.NVarChar) { Value = string.Join(",", lstOrgIds) });
                parameters.Add(new SqlParameter("@SearchLocationKey", SqlDbType.NVarChar) { Value = string.Join(",", lstOrgIds) });
                //var advList = _patient.ExecWithStoreProcedure<ProviderAdvertisements>("spGetAdvertisements_ByOrgTypeId @userTypeId",
                //   new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = organizationTypeId }).ToList();
                //var advertisementList = _doctor.GetDistanceMilebyOrgIds(advList, lat2, long2);

                var allslotList = _featuredService.GetOrganisationListByTypeId_HomePageSearch(StoredProcedureList.GetOrganisationListByTypeId_HomePageSearch_v1, parameters).ToList();
                var searchResult = new OrganizationProviderListModel() { OrganizationProviderList = allslotList };
                //ViewBag.SearchBox = model.SearchBox ?? "";
                searchResult.TotalRecord = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;



                //parameters.Add(new SqlParameter("@FeaturedFacilityIds", SqlDbType.NVarChar) { Value = string.Join(",", featureFacilityIds.Select(m => m.ReferenceId).ToList()) });

                //int totalRecord = 0;
                //var allslotList = _featuredService.GetOrganisationListByTypeId(StoredProcedureList.GetOrganisationListByTypeId, parameters, out totalRecord).ToList();
                //var searchResult = new OrganizationProviderListModel() { OrganizationProviderList = allslotList };
                ////ViewBag.SearchBox = model.SearchBox ?? "";

                //searchResult.TotalRecord = totalRecord;
                //searchResult.PageSize = model.PageSize;
                //searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region SeniorCare
        [HttpPost]
        [Route("api/Home/SearchSeniorCare")]
        public HttpResponseMessage _SeniorCare(SearchParamModel model)
        {
            //SearchParamModel model = new SearchParamModel();
            //model = GetSearchParamModel(model);
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.SeniorCare);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = searchResult });
        }
        #endregion

        #region Facility
        [HttpPost]
        [Route("api/Home/SearchFacility")]
        public HttpResponseMessage _Facility(SearchParamModel model)
        {
            //SearchParamModel model = new SearchParamModel();
            //model = GetSearchParamModel(model);
            var searchResult = GetOrganizationSearchResult(model, OrganisationTypes.Facility);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = searchResult });
        }
        #endregion

        #region Doctor-Profile
        [HttpPost]
        [Route("api/Profile/Doctor/{Npi}")]   //[Route("api/Profile/Doctor/{doctorName?}-{Npi}")]
        public HttpResponseMessage GetDoctorProfile(string doctorName = null, string npi = "1588667638")
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

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = doctorProfile });
        }
        #endregion
    }
}
