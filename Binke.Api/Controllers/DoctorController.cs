using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Data.SqlClient;
using System.Transactions;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Enumerable;
using Binke.Api.Utility;
using System.IO;
using Doctyme.Model.ViewModels;
using System.Drawing;
using Binke.Api.ViewModels;
using Doctyme.Repository.Services;
using Microsoft.Owin;
//using Binke.Api.Models;
//using Common = Binke.Api.Utility.Common;

namespace Binke.Api
{
    [Authorize]
    public class DoctorController : BaseApiController
    {
        private readonly IDoctorService _doctor;
        private readonly IRepository _repo;
        private readonly IFeaturedDoctorService _featuredDoctor;
        private readonly IDoctorInsuranceAcceptedService _doctorInsuranceAccepted;
        private readonly IDoctorInsuranceService _doctorInsurance;
        private readonly IAgeGroupService _agegroup;
        private readonly IDoctorImageService _doctorImage;
        private readonly IDoctorFacilityAffiliationService _doctorFacilityAffiliation;
        private readonly ISpecialityService _speciality;
        private readonly IFacilityService _facility;

        public DoctorController(IDoctorService doctorService, IRepository repo, IFeaturedDoctorService featuredDoctor, IDoctorInsuranceAcceptedService insuranceAcceptedService, IDoctorInsuranceService doctorInsuranceService, IAgeGroupService agegroup, IDoctorImageService doctorImage, IDoctorFacilityAffiliationService doctorFacilityAffiliation, IFacilityService facility, ISpecialityService speciality)
        {
            this._doctor = doctorService;
            this._repo = repo;
            this._featuredDoctor = featuredDoctor;
            this._doctorInsuranceAccepted = insuranceAcceptedService;
            this._doctorInsurance = doctorInsuranceService;
            this._agegroup = agegroup;
            this._doctorImage = doctorImage;
            this._doctorFacilityAffiliation = doctorFacilityAffiliation;
            this._facility = facility;
            this._speciality = speciality;

        }

        #region Doctor
        [HttpPost, Route("api/Doctors")]
        public HttpResponseMessage GetAllDoctors(Pagination objPagination)
        {
            //Pagination objPagination = new Pagination();
            //objPagination.StartIndex = Convert.ToInt32(HttpContext.Current.Request.Params["StartIndex"]);
            //objPagination.PageSize = Convert.ToInt32(HttpContext.Current.Request.Params["PageSize"]);
            objPagination.StartIndex = objPagination.StartIndex > 0 ? ((objPagination.StartIndex / objPagination.PageSize)) + 1 : 1;
            objPagination.PageSize = objPagination.PageSize > 0 ? objPagination.PageSize : 10;
            objPagination.SortDirection = string.IsNullOrEmpty(objPagination.SortDirection) ? "Asc" : objPagination.SortDirection;
            var _ipInfo = new IpInfo();
            var list = _doctor.GetDoctors(objPagination);
            var latOfUser = string.Empty;
            var longOfUser = string.Empty;
            decimal lat2 = 0;
            decimal long2 = 0;
            string ipString = GetIPString();
            try
            {
                if (!string.IsNullOrEmpty(ipString))
                    _ipInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<IpInfo>(ipString);
            }
            catch
            {
                _ipInfo = new IpInfo();
            }
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

            decimal.TryParse(latOfUser, out lat2);
            decimal.TryParse(longOfUser, out long2);
            var doctList = _doctor.GetDistance(list.Select(x => x.DoctorId).ToList(), lat2, long2);
            foreach (var item in list)
            {
                item.DistanceCount = doctList.FirstOrDefault(x => x.DoctorId == item.DoctorId).DistanceCount;

                if (!string.IsNullOrEmpty(item.LogoFilePath))
                    item.LogoFilePath = "https://www.doctyme.com/Uploads/DoctorSiteImages/" + item.LogoFilePath;
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = string.Empty, Doctors = list.OrderBy(m => m.DistanceCount).ToList() });
        }

        [HttpPost, Route("api/DoctorsList")]
        public HttpResponseMessage GetAllDoctorsList(Pagination objPagination)
        {
            objPagination.StartIndex = objPagination.StartIndex > 0 ? ((objPagination.StartIndex / objPagination.PageSize)) + 1 : 1;
            objPagination.PageSize = objPagination.PageSize > 0 ? objPagination.PageSize : 10;
            objPagination.SortDirection = string.IsNullOrEmpty(objPagination.SortDirection) ? "Asc" : objPagination.SortDirection;

            var list = _doctor.GetDoctorsList(objPagination);
            if (list.Any())
            {
                List<int> tempSpecities;
                List<int> tempFacility;
                var specities = _speciality.GetAll(x => x.IsActive && !x.IsDeleted)
                    .Select(x => new SelectIdValueModel { Id = x.SpecialityId, Value = x.SpecialityName }).ToList();
                var facility = _facility.GetAll(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Facility")
                    .Select(x => new SelectIdValueModel { Id = x.OrganisationId, Value = x.OrganisationName }).ToList();
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item.LogoFilePath))
                        item.LogoFilePath = "https://www.doctyme.com/Uploads/DoctorSiteImages/" + item.LogoFilePath;

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

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = string.Empty, Doctors = list });
        }
        [HttpGet, Route("api/Doctor/{id}")]
        public HttpResponseMessage GetDoctorDetails(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "'DoctorId' parameter required." });

            var result = _repo.ExecWithStoreProcedure<DoctorViewModel>("spDoctor @Activity ,@Id  ",
                           new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                           new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                           .FirstOrDefault();
            if (!string.IsNullOrEmpty(result.LogoFilePath))
                result.LogoFilePath = "https://www.doctyme.com/Uploads/DoctorSiteImages/" + result.LogoFilePath;

            if (result is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = "Invalid DoctorId" });

            var Doctor = result;
            var DoctorId = Convert.ToString(id);
            Doctor.DoctorName = Doctor.FirstName + " " + Doctor.LastName;
            Doctor.DoctorId = Convert.ToInt32(DoctorId);

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = string.Empty, Doctor = Doctor });

        }


        [HttpPost, Route("api/Doctor/save")]
        public HttpResponseMessage SaveDoctor(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

            if (_doctor.GetSingle(x => x.NPI == model.NPI && x.DoctorId != model.DoctorId) == null)
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDoctor_Create @UserId, @NamePrefix, @NameSuffix, @FirstName, @LastName, @MiddleName, @Gender, @Status, @NPI, @Education, @EnumerationDate, @ShortDescription, @LongDescription, @SoleProprietor, @IsAllowNewPatient, @IsNtPcp, @IsPrimaryCare, @Keywords, @PracticeStartDate, @Id, @CreatedBy, @Language, @OtherNames, @Credential",
                    new SqlParameter("UserId", System.Data.SqlDbType.Int) { SqlValue = UserId },
                    new SqlParameter("NamePrefix", System.Data.SqlDbType.VarChar, 10) { SqlValue = string.IsNullOrEmpty(model.NamePrefix) ? string.Empty : model.NamePrefix },
                    new SqlParameter("NameSuffix", System.Data.SqlDbType.VarChar, 10) { SqlValue = string.IsNullOrEmpty(model.NameSuffix) ? string.Empty : model.NameSuffix },
                    new SqlParameter("FirstName", System.Data.SqlDbType.VarChar, 50) { SqlValue = model.FirstName },
                    new SqlParameter("LastName", System.Data.SqlDbType.VarChar, 50) { SqlValue = model.LastName },
                    new SqlParameter("MiddleName", System.Data.SqlDbType.VarChar, 50) { SqlValue = string.IsNullOrEmpty(model.MiddleName) ? string.Empty : model.MiddleName },
                    new SqlParameter("Gender", System.Data.SqlDbType.VarChar, 10) { SqlValue = string.IsNullOrEmpty(model.Gender) ? string.Empty : model.Gender },
                    new SqlParameter("Status", System.Data.SqlDbType.VarChar, 10) { SqlValue = string.IsNullOrEmpty(model.Status) ? string.Empty : model.Status },
                    new SqlParameter("NPI", System.Data.SqlDbType.VarChar, 10) { SqlValue = model.NPI },
                    new SqlParameter("Education", System.Data.SqlDbType.VarChar, 50) { SqlValue = string.IsNullOrEmpty(model.Education) ? string.Empty : model.Education },
                    new SqlParameter("EnumerationDate", System.Data.SqlDbType.Date) { SqlValue = model.EnumerationDate.HasValue ? model.EnumerationDate.Value : System.Data.SqlTypes.SqlDateTime.Null, IsNullable = true },
                    new SqlParameter("ShortDescription", System.Data.SqlDbType.VarChar, 300) { SqlValue = string.IsNullOrEmpty(model.ShortDescription) ? string.Empty : model.ShortDescription },
                    new SqlParameter("LongDescription", System.Data.SqlDbType.NVarChar) { SqlValue = string.IsNullOrEmpty(model.LongDescription) ? string.Empty : model.LongDescription },
                    new SqlParameter("SoleProprietor", System.Data.SqlDbType.Bit) { SqlValue = model.SoleProprietor },
                    new SqlParameter("IsAllowNewPatient", System.Data.SqlDbType.Bit) { SqlValue = model.IsAllowNewPatient },
                    new SqlParameter("IsNtPcp", System.Data.SqlDbType.Bit) { SqlValue = model.IsNtPcp },
                    new SqlParameter("IsPrimaryCare", System.Data.SqlDbType.Bit) { SqlValue = model.IsPrimaryCare },
                    new SqlParameter("Keywords", System.Data.SqlDbType.NVarChar) { SqlValue = string.IsNullOrEmpty(model.Keywords) ? string.Empty : model.Keywords },
                    new SqlParameter("PracticeStartDate", System.Data.SqlDbType.DateTime) { SqlValue = model.PracticeStartDate.HasValue ? model.PracticeStartDate.Value : System.Data.SqlTypes.SqlDateTime.Null, IsNullable = true },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { SqlValue = model.DoctorId },
                    new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { SqlValue = UserId },
                    new SqlParameter("Language", System.Data.SqlDbType.VarChar, 1000) { SqlValue = string.IsNullOrEmpty(model.Language) ? string.Empty : model.Language },
                    new SqlParameter("OtherNames", System.Data.SqlDbType.NVarChar) { SqlValue = string.IsNullOrEmpty(model.OtherNames) ? string.Empty : model.OtherNames },
                    new SqlParameter("Credential", System.Data.SqlDbType.VarChar, 10) { SqlValue = string.IsNullOrEmpty(model.Credential) ? string.Empty : model.Credential }
                    );
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Doctor save successfully" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "NPI Already Exists" });
            }
        }

        [HttpDelete, Route("api/Doctor/{id}/remove")]
        public HttpResponseMessage DeleteDoctor(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "'DoctorId' parameter required." });

            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("sprDeleteDoctor " +
                     "@Id", new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id });

            if (ExecWithStoreProcedure > 0)
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Doctor deleted successfully" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong." });
        }

        #endregion

        #region DoctorAgeGroup

        [HttpPost, Route("api/DoctorAgeGroup/save")]
        public HttpResponseMessage SaveDoctorAgeGroup(AgeGroupModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<DoctorAgeGroup>(x => x.AgeGroup.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {

                                txscope.Dispose();
                                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Age Group already exists. Could not add the data!" });
                            }

                            var agegroup = new Doctyme.Model.DoctorAgeGroup()
                            {
                                DoctorAgeGroupId = model.Id,
                                AgeGroupId = model.AgeGroupId,
                                DoctorId = model.DoctorId,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedDate = DateTime.Now
                            };


                            _repo.Insert<DoctorAgeGroup>(agegroup, true);
                            txscope.Complete();
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Age Group save successfully" });
                        }
                        else
                        {

                            var bResultdata = _repo.Find<DoctorAgeGroup>(x => x.AgeGroupId == model.AgeGroupId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                            if (bResultdata != null)
                            {
                                txscope.Dispose();
                                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Age Group already exists. Could not add the data!" });
                            }

                            var age = _repo.All<DoctorAgeGroup>().FirstOrDefault(x => x.DoctorAgeGroupId == model.Id);
                            age.AgeGroupId = model.AgeGroupId;

                            _repo.Update<DoctorAgeGroup>(age, true);
                            txscope.Complete();
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Age Group update successfully" });
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    //Common.LogError(ex, "AddEditDoctorAgeGroup-Post");
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpGet, Route("api/DoctorAgeGroup/{id}/get")]
        public HttpResponseMessage DoctorAgeGroup(int id)
        {

            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "AgeGroupId is required" });

            var result = _repo.Find<DoctorAgeGroup>(id);

            if (result is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid AgeGroupId" });

            var agegroup = new AgeGroupModel()
            {
                AgeGroupId = result.AgeGroupId,
                Name = result.AgeGroup.Name,
                Description = result.AgeGroup.Description,
                DoctorName = result.Doctor.Name,
                Id = result.DoctorAgeGroupId,
                DoctorId = result.DoctorId
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = string.Empty, AgeGroup = agegroup });
        }

        [Route("api/DoctorAgeGroup/{doctorId}")]
        public HttpResponseMessage GetDoctorAgeGroupList(int doctorId, Pagination param)
        {
            var allagegroup = _repo.All<DoctorAgeGroup>().Where(x => x.IsActive && !x.IsDeleted && x.DoctorId == doctorId).Select(x => new AgeGroupModel()
            {
                AgeGroupId = x.AgeGroupId,
                Name = x.AgeGroup.Name,
                Description = x.AgeGroup.Description,
                IsActive = x.IsActive,
                DoctorName = x.Doctor.FirstName,
                Id = x.DoctorAgeGroupId,
                DoctorId = x.DoctorId
            }).ToList();

            if (!string.IsNullOrEmpty(param.Search))
            {
                allagegroup = allagegroup.Where(x => x.Name.ToString().ToLower().Contains(param.Search.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.Search.ToLower())
                                                || x.UpdatedDate.ToString().ToLower().Contains(param.Search.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(param.SortColumnName);
            var sortDirection = param.SortDirection; // asc or desc

            Func<AgeGroupModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.CreatedDate.ToDefaultFormate()
                        : sortColumnIndex == 3 ? c.UpdatedDate?.ToDefaultFormate()
                            : c.Name;
            allagegroup = sortDirection == "asc" ? allagegroup.OrderBy(orderingFunction).ToList() : allagegroup.OrderByDescending(orderingFunction).ToList();

            var display = allagegroup.Skip(param.StartIndex).Take(param.PageSize);
            var total = allagegroup.Count;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = total,
                TotalDisplayRecords = total,
                AgeGroups = display
            });
        }

        [HttpDelete, Route("api/DoctorAgeGroup/{id}/remove")]
        public HttpResponseMessage RemoveDoctorAgeGroup(int id)
        {
            var agegroup = _repo.Find<DoctorAgeGroup>(id);
            agegroup.IsDeleted = true;
            _repo.Update<DoctorAgeGroup>(agegroup, true);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Age Group remove successfully" });
        }

        #endregion

        #region DocOrgInsurances
        [Route("api/DoctorInsurances/{doctorId}")]
        public HttpResponseMessage GetDoctorInsurancesList(int doctorId, Pagination param)
        {
            var allInsurances = _doctor.GetDoctorInsurancess(param, doctorId).Where(x => x.IsActive && !x.IsDeleted).Select(x => new DoctorInsurancesViewModel()
            {
                DocOrgInsuranceId = x.DocOrgInsuranceId,
                InsuranceIdentifierId = x.InsuranceIdentifierId,
                InsurancePlanId = x.InsurancePlanId,
                StateId = x.StateId,
                UserTypeId = x.UserTypeId,
                DoctorName = x.DoctorName,
                DoctorId = x.DoctorId,
                TotalRows = x.TotalRows,
                IsActive = x.IsActive,
                InsurancePlanName = x.InsurancePlanName,
                ReferenceName = x.ReferenceName

            }).ToList();

            var total = allInsurances.Count > 0 ? allInsurances.FirstOrDefault().TotalRows : 0;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = total,
                TotalDisplayRecords = total,
                Insurances = allInsurances
            });
        }

        [HttpPost, Route("api/DoctorInsurance/save")]
        public HttpResponseMessage SaveDoctorInsurances(DoctorInsurancesViewModel model)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Data = ModelState.Errors() });

            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgInsurances " +
                  "@Activity," +
                   "@Id," +
                "@ReferenceId," +
                        "@InsurancePlanId," +
                        "@InsuranceIdentifierId," +
                        "@StateId," +
                        "@IsActive," +
                        "@CreatedBy," +
                       "@UserTypeId"

                          , new SqlParameter("Activity", System.Data.SqlDbType.VarChar) { Value = "Insert" },
                           new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = model.DocOrgInsuranceId },
                                      new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                      new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = (object)model.InsurancePlanId ?? DBNull.Value },
                                      new SqlParameter("InsuranceIdentifierId", System.Data.SqlDbType.VarChar, 20) { Value = string.IsNullOrEmpty(model.InsuranceIdentifierId) ? string.Empty : model.InsuranceIdentifierId },
                                      new SqlParameter("StateId", System.Data.SqlDbType.NVarChar) { Value = string.IsNullOrEmpty(model.StateId) ? string.Empty : model.StateId },
                                        new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                           new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                       new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 }
                       );
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Insurance save successfully" });
        }

        [HttpGet, Route("api/DoctorInsurance/{id}")]
        public HttpResponseMessage GetDoctorInsurances(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "InsuranceId is required" });

            var Insurances = _repo.ExecWithStoreProcedure<DoctorInsurancesViewModel>("spDocOrgInsurances @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();

            Insurances.DoctorName = Insurances.ReferenceName;
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Insurances = Insurances });

        }

        [HttpDelete, Route("api/DoctorInsurance/{id}/remove")]
        public HttpResponseMessage RemoveDoctorInsurances(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "InsuranceId is required" });

            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgInsurances @Activity, @Id ",
                                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Delete" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Insurance deleted successfully" });
        }
        #endregion


        #region Images
        [HttpGet, Route("api/DoctorImages/{doctorId}")]
        public HttpResponseMessage GetDoctorImages(int doctorId, Pagination param)
        {
            var allImages = _doctorImage.GetAll(x => !x.IsDeleted && x.ReferenceId == doctorId).Select(x => new DoctorImageModel()
            {
                Id = x.SiteImageId,
                ImagePath = FilePathList.Doctor + x.ImagePath,
                CreatedDate = x.CreatedDate.UtcToUserTime(),
                UpdatedDate = x.UpdatedDate?.UtcToUserTime()
            }).ToList();

            if (!string.IsNullOrEmpty(param.Search))
            {
                allImages = allImages.Where(x => x.CreatedDate.ToString().ToLower().Contains(param.Search.ToLower())
                                              || x.UpdatedDate.ToString().ToLower().Contains(param.Search.ToLower())).ToList();
            }
            var sortColumnIndex = Convert.ToInt32(param.SortColumnName);
            var sortDirection = param.SortDirection; // asc or desc

            Func<DoctorImageModel, string> orderingFunction =
                c => sortColumnIndex == 2 ? c.CreatedDate.ToDefaultFormate()
                    : sortColumnIndex == 3 ? c.UpdatedDate?.ToDefaultFormate()
                            : c.CreatedDate.ToDefaultFormate();

            allImages = sortDirection == "asc" ? allImages.OrderBy(orderingFunction).ToList() : allImages.OrderByDescending(orderingFunction).ToList();


            var display = allImages.Skip(param.StartIndex).Take(param.PageSize);
            var total = allImages.Count;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = total,
                TotalDisplayRecords = total,
                Images = display
            });
        }

        [HttpGet, Route("api/doctorImage/{id}/get")]
        public HttpResponseMessage DoctorImage(int id)
        {
            if (id == 0) return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "ImageId is required" });

            var doctorImage = _doctorImage.GetById(id);

            if (doctorImage == null) return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid ImageId" });

            var result = new DoctorImageModel
            {
                DoctorImageId = doctorImage.SiteImageId,
                DoctorId = doctorImage.ReferenceId ?? 0,
                ImagePath = FilePathList.Doctor + doctorImage.ImagePath,
                ImageName = doctorImage.ImagePath
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Image = result });
        }

        [HttpPost, Route("api/doctorImage/save")]
        public HttpResponseMessage MyImages([FromBody] DoctorImageModel model)
        {
            var filePath = new List<string>();
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var imageDetail = _doctorImage.GetById(model.DoctorImageId);

                    string extension = Path.GetExtension(model.ImageName);
                    var ImagePath = $@"Doctor-{DateTime.Now.Ticks}{extension}";
                    string singleFile = Path.Combine(HttpContext.Current.Server.MapPath(FilePathList.Doctor), ImagePath);
                    Common.CheckServerPath(HttpContext.Current.Server.MapPath(FilePathList.Doctor));

                    using (MemoryStream ms = new MemoryStream(model.Image))
                    {
                        Image image = Image.FromStream(ms);
                        image.Save(singleFile);
                    }

                    if (imageDetail is null)
                    {
                        var doctorImage = new SiteImage()
                        {
                            ReferenceId = model.DoctorId,
                            ImagePath = ImagePath,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now,
                            CreatedBy = UserId,
                            UserTypeID = 2
                        };
                        _doctorImage.InsertData(doctorImage);
                    }
                    else
                    {
                        imageDetail.ImagePath = ImagePath;
                        imageDetail.IsDeleted = false;
                        imageDetail.ModifiedBy = UserId;
                        imageDetail.UpdatedDate = DateTime.Now;
                        _doctorImage.UpdateData(imageDetail);
                    }

                    _doctorImage.SaveData();
                    txscope.Complete();
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Image upload successfully" });
                }
                catch (Exception ex)
                {
                    if (filePath.Any())
                    {
                        filePath.ForEach(x => Common.DeleteFile(x, true));
                    }
                    txscope.Dispose();
                    //Common.LogError(ex, "MyImages-Post");
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpDelete, Route("api/doctorImage/{id}/Remove")]
        public HttpResponseMessage RemoveMyImage(int id)
        {
            if (id == 0) return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "ImageId is required" });

            var doctorImage = _doctorImage.GetById(id);
            Common.DeleteFile(doctorImage.ImagePath);
            _doctorImage.DeleteData(doctorImage);
            _doctorImage.SaveData();
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Image deleted successfully" });
        }
        #endregion

        #region DocOrgTaxonomy
        [Route("api/DoctorTaxonomy/{doctorId}")]
        public HttpResponseMessage GetDoctorTaxonomyList(int doctorId, Pagination param)
        {
            var Info = _repo.ExecWithStoreProcedure<DoctorTaxonomyModel>("spDocOrgTaxonomyDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = doctorId },

                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = Info
                .Select(x => new DoctorTaxonomyListModel()
                {
                    DocOrgTaxonomyID = x.DocOrgTaxonomyID,
                    TaxonomyID = x.TaxonomyID,
                    ReferenceID = x.ReferenceID,
                    DoctorId = x.ReferenceID,
                    Taxonomy_Code = x.Taxonomy_Code,
                    Specialization = x.Specialization,
                    UpdatedDate = x.UpdatedDate?.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    TotalRecordCount = x.TotalRecordCount
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                Taxonomies = result
            });
        }


        //---- Get Taxonomy Details
        [HttpGet, Route("api/DoctorTaxonomy/{id}/get")]
        public HttpResponseMessage Taxonomy(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "TaxonomyId is required" });

            var orgTaxonomyInfo = _repo.ExecWithStoreProcedure<DoctorTaxonomyModel>("spDocOrgTaxonomyDoctor_GetById @DocOrgTaxonomyID",
                new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new DoctorTaxonomyUpdateModel
                {
                    DocOrgTaxonomyID = x.DocOrgTaxonomyID,
                    TaxonomyID = x.TaxonomyID,
                    DoctorId = x.ReferenceID,
                    Taxonomy_Code = x.Taxonomy_Code,
                    Specialization = x.Specialization,
                    UserTypeID = (int)x.UserTypeId,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                }).First();


            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Taxonomy = orgTaxonomyInfo });
        }


        //-- Add/Edit Taxonomy

        [HttpPost, Route("api/DoctorTaxonomy/save")]
        public HttpResponseMessage SaveDoctorTaxonomy(DoctorTaxonomyUpdateModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgTaxonomy_Create " +
                            "@DocOrgTaxonomyID," +
                            "@ReferenceID," +
                            "@TaxonomyID," +
                            "@IsActive," +
                            "@CreatedBy," +
                            "@UserTypeId",
                                          new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = model.DocOrgTaxonomyID > 0 ? model.DocOrgTaxonomyID : 0 },
                                          new SqlParameter("ReferenceID", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                          new SqlParameter("TaxonomyID", System.Data.SqlDbType.Int) { Value = model.TaxonomyID },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.VarChar) { Value = model.IsActive },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Doctor Taxonomy info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 4, Data = ModelState.Errors() });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong" });
            }

        }

        [HttpDelete, Route("api/DoctorTaxonomy/{id}/Remove")]
        public HttpResponseMessage RemoveDoctorTaxonomy(int id)
        {
            if (id == 0) return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "TaxonomyId is required" });

            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgTaxonomy_Remove @DocOrgTaxonomyID",
                    new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Taxonomy deleted successfully" });
        }

        #endregion

        #region Social Media

        [HttpGet, Route("api/DoctorSocialMedias/{doctorId}")]
        public HttpResponseMessage GetDoctorSocialMediaList(int doctorId, Pagination param)
        {
            var socialMediaInfo = _repo.ExecWithStoreProcedure<DocSocialMediaUpdateModel>("[spSocialMediaDoctor_Get] @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = doctorId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = socialMediaInfo
                .Select(x => new DocSocialMediaUpdateModel()
                {
                    SocialMediaId = x.SocialMediaId,
                    DoctorId = x.ReferenceId ?? 0,
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


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                SocialMedias = result
            });
        }


        //---- Get Social Media Details
        [HttpGet, Route("api/DoctorSocialMedia/{id}")]
        public HttpResponseMessage DoctorSocialMedia(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "SocailMediaId is required" });


            var docSocialMediaInfo = _repo.ExecWithStoreProcedure<DocSocialMediaUpdateModel>("spSocialMediaDoctor_GetById @SocialMediaId",
                new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new DocSocialMediaUpdateModel
                {
                    SocialMediaId = x.SocialMediaId,
                    DoctorId = (int)x.ReferenceId,
                    UserTypeID = 2,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Facebook = x.Facebook,
                    Twitter = x.Twitter,
                    LinkedIn = x.LinkedIn,
                    Instagram = x.Instagram,
                    Youtube = x.Youtube,
                    Pinterest = x.Pinterest,
                    Tumblr = x.Tumblr
                }).FirstOrDefault();

            if (docSocialMediaInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid SocailMediaId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, SocialMedia = docSocialMediaInfo });
        }


        [HttpPost, Route("api/DoctorSocialMedia/save")]
        public HttpResponseMessage SaveDoctorSocialMedia(DocSocialMediaUpdateModel model)
        {
            try
            {
                if (model.DoctorId > 0)
                {
                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSocialMedia_Create " +
                            "@SocialMediaId," +
                            "@ReferenceId," +
                            "@Facebook," +
                            "@Twitter," +
                            "@LinkedIn," +
                            "@Instagram," +
                            "@Youtube," +
                            "@Pinterest," +
                            "@Tumblr," +
                            "@IsActive," +
                            "@UserTypeId," +
                            "@CreatedBy",
                                          new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = model.SocialMediaId > 0 ? model.SocialMediaId : 0 },
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                          new SqlParameter("Facebook", System.Data.SqlDbType.VarChar) { Value = (object)(model.Facebook) ?? DBNull.Value },
                                          new SqlParameter("Twitter", System.Data.SqlDbType.VarChar) { Value = (object)model.Twitter ?? DBNull.Value },
                                          new SqlParameter("LinkedIn", System.Data.SqlDbType.VarChar) { Value = (object)model.LinkedIn ?? DBNull.Value },
                                          new SqlParameter("Instagram", System.Data.SqlDbType.VarChar) { Value = (object)model.Instagram ?? DBNull.Value },
                                          new SqlParameter("Youtube", System.Data.SqlDbType.VarChar) { Value = (object)model.Youtube ?? DBNull.Value },
                                          new SqlParameter("Pinterest", System.Data.SqlDbType.VarChar) { Value = (object)model.Pinterest ?? DBNull.Value },
                                          new SqlParameter("Tumblr", System.Data.SqlDbType.VarChar) { Value = (object)model.Tumblr ?? DBNull.Value },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive ?? false },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Doctor social media info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Doctor Info Not Found! Should be select Doctor Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Doctor social media info saving Error.." });
            }

        }
        #endregion

        #region Opening Hours
        //---- Get Opening Hours Details
        [HttpGet, Route("api/DoctorOpeningHours/{doctorId}")]
        public HttpResponseMessage DoctorOpeningHours(int doctorId)
        {
            if (doctorId == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.BadRequest, Message = "DoctorId is required" });

            var orgInfo = _repo.ExecWithStoreProcedure<DoctorOpeningHoursUpdateModel>("spDoctorOpeningHour_GetByDoctorID @DoctorId",
                new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = doctorId }
                );

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.BadRequest, Message = "Invalid DoctorId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, OpeningHours = orgInfo });
        }


        //--- Update Doctor Opening Hours

        [HttpPost, Route("api/DoctorOpeningHours/save")]
        public HttpResponseMessage SaveDoctorOpeningHours(DoctorOpeningHoursUpdateModel model)
        {
            try
            {

                if (model.DoctorID > 0)
                {
                    for (int i = 0; i < 7; i++)
                    {

                        int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOpeningHourDoctor_Create " +
                                "@OpeningHourID," +
                                "@DoctorID," +
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
                                              new SqlParameter("DoctorID", System.Data.SqlDbType.Int) { Value = model.DoctorID },
                                              new SqlParameter("CalendarDate", System.Data.SqlDbType.Date) { Value = (object)model.CalendarDate ?? DBNull.Value },
                                              new SqlParameter("WeekDay", System.Data.SqlDbType.Int) { Value = model.DayNo[i] },
                                              new SqlParameter("SlotDuration", System.Data.SqlDbType.Int) { Value = (object)model.SlotDuration[i] ?? DBNull.Value },
                                              new SqlParameter("StartDateTime", System.Data.SqlDbType.NVarChar) { Value = (object)model.StartTime[i] ?? DBNull.Value },
                                              new SqlParameter("EndDateTime", System.Data.SqlDbType.NVarChar) { Value = (object)model.EndTime[i] ?? DBNull.Value },
                                              new SqlParameter("IsHoliday", System.Data.SqlDbType.Bit) { Value = (object)model.IsHoliday[i] ?? DBNull.Value },
                                              new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive[i] ?? DBNull.Value },
                                               new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = false },
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                              new SqlParameter("Comments", System.Data.SqlDbType.NVarChar) { Value = (object)model.Comments[i] ?? DBNull.Value }
                                          );
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Doctor Opening Hours info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Doctor Info Not Found! Should be select Doctor Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Doctor OpeningHours info saving Error.." });
            }

        }

        #endregion

        #region Doctor State License

        [HttpGet, Route("api/DoctorStateLicenses/{doctorId}")]
        public HttpResponseMessage GetDoctorStateLicenseList(int doctorId, Pagination param)
        {
            var result = _repo.ExecWithStoreProcedure<DoctorStateLicenseModel>("spDocOrgStateLicensesDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = doctorId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    ).Select(x => new DoctorStateLicenseListModel
                    {
                        DocOrgStateLicenseId = x.DocOrgStateLicense,
                        ReferenceName = x.ReferenceName,
                        HealthCareProviderTaxonomyCode = x.HealthCareProviderTaxonomyCode,
                        ProviderLicenseNumber = x.ProviderLicenseNumber,
                        ProviderLicenseNumberStateCode = x.ProviderLicenseNumberStateCode,
                        HealthcareProviderPrimaryTaxonomySwitch = x.HealthcareProviderPrimaryTaxonomySwitch,
                        UpdatedDate = x.UpdatedDate?.ToDefaultFormate(),
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        TotalRecordCount = x.TotalRecordCount
                    }).ToList();


            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                StateLicenses = result
            });
        }

        [HttpPost, Route("api/DoctorStateLicense/save")]
        public HttpResponseMessage SaveDoctorStateLicense(DoctorStateLicenseUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.DoctorId > 0)
                {
                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgStateLicenses_Create " +
                            "@DocOrgStateLicense," +
                            "@ReferenceId," +
                            "@HealthCareProviderTaxonomyCode," +
                            "@ProviderLicenseNumber," +
                            "@ProviderLicenseNumberStateCode," +
                            "@HealthcareProviderPrimaryTaxonomySwitch," +
                            "@IsActive," +
                            "@CreatedBy," +
                            "@UserTypeId",
                                          new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = model.DocOrgStateLicenseId > 0 ? model.DocOrgStateLicenseId : 0 },
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                          new SqlParameter("HealthCareProviderTaxonomyCode", System.Data.SqlDbType.NVarChar) { Value = (object)model.HealthCareProviderTaxonomyCode ?? DBNull.Value },
                                          new SqlParameter("ProviderLicenseNumber", System.Data.SqlDbType.NVarChar) { Value = (object)model.ProviderLicenseNumber ?? DBNull.Value },
                                          new SqlParameter("ProviderLicenseNumberStateCode", System.Data.SqlDbType.NVarChar) { Value = (object)model.ProviderLicenseNumberStateCode ?? DBNull.Value },
                                          new SqlParameter("HealthcareProviderPrimaryTaxonomySwitch", System.Data.SqlDbType.Bit) { Value = model.HealthcareProviderPrimaryTaxonomySwitch ?? false },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Doctor State License info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Doctor State License info saving Error.." });
            }

        }

        [HttpGet, Route("api/DoctorStateLicense/{id}")]
        public HttpResponseMessage StateLicense(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "DoctorStateLicenseId is required" });

            var orgLicenseInfo = _repo.ExecWithStoreProcedure<DoctorStateLicenseModel>("spDocOrgStateLicensesDoctor_GetById @DocOrgStateLicense",
                new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new DoctorStateLicenseUpdateModel
                {
                    DocOrgStateLicenseId = x.DocOrgStateLicense,
                    DoctorId = x.ReferenceId,
                    UserTypeID = x.UserTypeID,
                    HealthCareProviderTaxonomyCode = x.HealthCareProviderTaxonomyCode,
                    ProviderLicenseNumber = x.ProviderLicenseNumber,
                    ProviderLicenseNumberStateCode = x.ProviderLicenseNumberStateCode,
                    HealthcareProviderPrimaryTaxonomySwitch = x.HealthcareProviderPrimaryTaxonomySwitch,
                    IsDeleted = x.IsDeleted.HasValue ? x.IsDeleted.Value : false,
                    IsActive = x.IsActive.HasValue ? x.IsActive.Value : false
                }).FirstOrDefault();

            if (orgLicenseInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid DoctorStateLicenseId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, StateLicense = orgLicenseInfo });
        }

        [HttpDelete, Route("api/DoctorStateLicense/{id}/remove")]
        public HttpResponseMessage RemoveDoctorStateLicense(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "StateLicenseId is required" });

            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgStateLicenses_Remove @DocOrgStateLicense",
                    new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "StateLicense deleted successfully" });
        }
        #endregion

        #region DoctorAffiliation
        [Route("api/DoctorAffiliations/{doctorId}")]
        public HttpResponseMessage GetDoctorAffiliationList(int doctorId, Pagination param)
        {
            var allAffiliation = _doctor.GetDoctorAffiliations(param, doctorId).Where(x => x.IsActive && !x.IsDeleted && x.DoctorId == doctorId).Select(x => new DoctorAffiliationViewModel()
            {
                DoctorAffiliationId = x.DoctorAffiliationId,
                OrganisationName = x.OrganisationName,
                Description = x.Description,
                IsActive = x.IsActive,
                DoctorName = x.DoctorName,
                // Id = x.DoctorAffiliationId,
                TotalRows = x.TotalRows
            }).ToList();


            var total = allAffiliation.Count > 0 ? allAffiliation.FirstOrDefault().TotalRows : 0;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = total,
                TotalDisplayRecords = total,
                Affiliations = allAffiliation
            });
        }

        [HttpGet, Route("api/DoctorAffiliation/{id}")]
        public HttpResponseMessage GetDoctorAffiliation(int id)
        {

            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "DoctorAffilationId is required" });

            var result = _repo.ExecWithStoreProcedure<DoctorAffiliationViewModel>("spDoctorAffiliation @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();

            if (result is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid DoctorAffilationId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Affiliation = result });

        }

        [HttpPost, Route("api/DoctorAffiliation/save")]
        public HttpResponseMessage SaveDoctorAffiliation(DoctorAffiliationViewModel model)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDoctorAffiliation " +
                            "@Activity," +
                             "@Id," +
                          "@OrganisationId," +
                           "@DoctorId," +
                           "@IsActive," +
                          "@CreatedBy"

                            , new SqlParameter("Activity", System.Data.SqlDbType.VarChar) { Value = "Insert" },
                              new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = model.DoctorAffiliationId },
                              new SqlParameter("OrganisationId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                  new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId }
                         );
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Affiliation save successfully" });
        }

        [HttpDelete, Route("api/DoctorAffiliation/{id}/remove")]
        public HttpResponseMessage RemoveDoctorAffiliation(int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorAffiliation @Activity ,@Id  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Delete" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                         ;

            return Request.CreateResponse(new { Status = 1, Message = " Affiliation remove successfully" });
        }
        #endregion

        #region DoctorGender

        [HttpPost, Route("api/DoctorGender/Save")]
        public HttpResponseMessage SaveDoctorGender(DoctorGenderModel model)
        {

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.DoctorGenderTypeId == 0)
                    {
                        var bResult = _repo.Find<DoctorGender>(x => x.GenderTypeId.ToString().ToLower().Equals(model.GenderTypeId.ToString()) && x.IsActive && !x.IsDeleted && x.DoctorId == model.DoctorId);
                        if (bResult != null)
                        {

                            txscope.Dispose();
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Gender already exists. Could not add the data!" });
                        }

                        var Gender = new Doctyme.Model.DoctorGender()
                        {
                            DoctorGenderId = model.DoctorGenderTypeId,
                            GenderTypeId = model.GenderTypeId,
                            DoctorId = model.DoctorId,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };


                        _repo.Insert<DoctorGender>(Gender, true);
                        txscope.Complete();
                        return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Gender save successfully" });
                    }
                    else
                    {

                        var bResultdata = _repo.Find<DoctorGender>(x => x.DoctorGenderId == model.DoctorGenderTypeId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                        if (bResultdata != null)
                        {
                            txscope.Dispose();
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Gender already exists. Could not add the data!" });
                        }

                        var gender = _repo.All<DoctorGender>().ToList().Where(x => x.DoctorGenderId == model.DoctorGenderTypeId).FirstOrDefault();
                        gender.GenderTypeId = model.GenderTypeId;

                        _repo.Update<DoctorGender>(gender, true);
                        txscope.Complete();
                        return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Gender update successfully" });
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpGet, Route("api/DoctorGender/{id}")]
        public HttpResponseMessage GetDoctorGender(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "DoctorGenderId is required" });

            var result = _repo.All<DoctorGender>().Where(x => x.DoctorGenderId == id).FirstOrDefault();

            if (result is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid DoctorGenderId" });

            var genderInfo = new DoctorGenderModel()
            {
                GenderTypeId = result.GenderTypeId,
                Name = result.Gender.Name,
                Description = result.Gender.Description,
                IsActive = result.IsActive,
                DoctorGenderTypeId = result.DoctorGenderId,
                DoctorId = result.DoctorId
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Gender = genderInfo });

        }

        [Route("api/doctorGenders/{doctorId}")]
        public HttpResponseMessage GetDoctorGenderList(int doctorId, Pagination param)
        {
            var allGender = _repo.All<DoctorGender>().Where(x => x.IsActive && !x.IsDeleted && x.DoctorId == doctorId).Select(x => new DoctorGenderModel()
            {
                GenderTypeId = x.GenderTypeId,
                Name = x.Gender.Name,
                Description = x.Gender.Description,
                IsActive = x.IsActive,
                DoctorGenderTypeId = x.DoctorGenderId,
                DoctorId = x.DoctorId

            }).ToList();

            if (!string.IsNullOrEmpty(param.Search))
            {
                allGender = allGender.Where(x => x.GenderTypeId.ToString().ToLower().Contains(param.Search.ToLower())
                                                ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(param.SortColumnName);
            var sortDirection = param.SortDirection; // asc or desc

            Func<DoctorGenderModel, int> orderingFunction =
                c => sortColumnIndex == 1 ? c.GenderTypeId
                            : c.DoctorGenderTypeId;

            allGender = sortDirection == "asc" ? allGender.OrderBy(orderingFunction).ToList() : allGender.OrderByDescending(orderingFunction).ToList();

            var display = allGender.Skip(param.StartIndex).Take(param.PageSize);
            var total = allGender.Count;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = total,
                TotalDisplayRecords = total,
                Genders = display
            });
        }

        [HttpDelete, Route("api/doctorgender/{id}/remove")]
        public HttpResponseMessage RemoveDoctorGender(int id)
        {
            var Gender = _repo.All<DoctorGender>().ToList().Where(x => x.DoctorGenderId == id).FirstOrDefault();
            Gender.IsDeleted = true;
            _repo.Update<DoctorGender>(Gender, true);
            // _Gender.SaveData();
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Gender remove successfully" });
        }

        #endregion

        #region Booking

        [HttpPost, Route("api/DoctorBookings/{Id}/{UserType}")]
        public HttpResponseMessage GetDoctorBookingList(int Id, Pagination param, int UserType)
        {

            if (UserType == 2)
            {
                var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spDocOrgBookingDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                         new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                         new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                         new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = Id },

                         new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                         new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                         new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                         );
                var result = orgInfo
               .Select(x => new DoctorBookingListModel()
               {
                   SlotId = x.SlotId,
                   DoctorId = x.DoctorId,
                   DoctorName = x.DoctorName + " [" + x.Credential + "]",
                   UpdatedDate = x.UpdatedDate?.ToDefaultFormate(),
                   SlotDate = x.SlotDate,
                   SlotTime = x.SlotTime,
                   IsActive = x.IsActive,
                   IsDeleted = x.IsDeleted,
                   BookedFor = x.BookedFor,
                   Description = x.Description,
                   FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                   FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country,
                   Email = x.Email,
                   PatientName = x.PatientName,
                   PhoneNumber = x.PhoneNumber,
                   TotalRecordCount = x.TotalRecordCount,
                   OrganisationName = x.OrganisationName

               }).ToList();

                int TotalRecordCount = 0;

                if (result.Count > 0)
                    TotalRecordCount = result[0].TotalRecordCount;


                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Status = HttpStatusCode.OK,
                    TotalRecords = TotalRecordCount,
                    TotalDisplayRecords = TotalRecordCount,
                    Bookings = result
                });
            }
            else
            {
                var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spPatientBooking_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                        new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                        new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserType },
                        new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = Id },

                        new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                        new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                        new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                        );
                var result = orgInfo
               .Select(x => new DoctorBookingListModel()
               {
                   SlotId = x.SlotId,
                   DoctorId = x.DoctorId,
                   DoctorName = x.DoctorName + " [" + x.Credential + "]",
                   UpdatedDate = x.UpdatedDate?.ToDefaultFormate(),
                   SlotDate = x.SlotDate,
                   SlotTime = x.SlotTime,
                   IsActive = x.IsActive,
                   IsDeleted = x.IsDeleted,
                   BookedFor = x.BookedFor,
                   Description = x.Description,
                   FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                   FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country,
                   Email = x.Email,
                   PatientName = x.PatientName,
                   PhoneNumber = x.PhoneNumber,
                   TotalRecordCount = x.TotalRecordCount,
                   OrganisationName = x.OrganisationName
               }).ToList();

                int TotalRecordCount = 0;

                if (result.Count > 0)
                    TotalRecordCount = result[0].TotalRecordCount;


                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Status = HttpStatusCode.OK,
                    TotalRecords = TotalRecordCount,
                    TotalDisplayRecords = TotalRecordCount,
                    Bookings = result
                });
            }

        }


        //---- Get Booking Details
        [HttpGet, Route("api/DoctorBookingDetail/{id}")]
        public HttpResponseMessage DoctorBooking(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Slot Id is required" });


            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spBooking_GetById @SlotId",
                new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id }

                ).Select(x => new DoctorBookingUpdateModel
                {
                    SlotId = x.SlotId,
                    OrganisationId = x.OrganisationId ?? 0,
                    DoctorId = x.DoctorId,
                    AddressId = x.AddressId,
                    BookedFor = x.BookedFor,
                    FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserTypeID = 2,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    SlotDate = x.SlotDate,
                    SlotTime = x.SlotTime,
                    Description = x.Description,
                    IsBooked = x.IsBooked,
                    IsEmailReminder = x.IsEmailReminder,
                    IsTextReminder = x.IsTextReminder,
                    IsInsuranceChanged = x.IsInsuranceChanged,
                    InsurancePlanId = x.InsurancePlanId,
                    InsuranceTypeId = x.InsuranceTypeId,
                    PatientName = x.PatientName,
                    OrganisationName = x.OrganisationName,
                    CreatedDate = x.CreatedDate.ToString("dd-MMM-yyyy"),
                    CreatedBy = x.CreatedBy,
                    UpdatedDate = x.UpdatedDate.ToString(),
                    OrganizatonTypeID = x.OrganizationTypeId ?? 0,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country,
                    CityStateZipCodeID = x.CityStateZipCodeID,
                    AddressTypeID = x.AddressTypeID

                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = "Invalid DoctorBookingId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Booking = orgInfo });
        }
        [HttpGet, Route("api/DoctorBooking/InsuranceTypes")]
        public HttpResponseMessage InsuranceTypeList()
        {
            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModels>("spInsuranceTypeDropDownList_Get");
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = typeList });
        }

        [HttpPost, Route("api/DoctorBooking/InsurancePlanByType/{DoctorId}/{Id}")]
        public HttpResponseMessage InsurancePlanByType(int DoctorId, int Id)
        {
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModels>("spOrgInsurancePlanDropDownListDoctor_Get @DoctorId, @InsuranceTypeId",
                  new SqlParameter("DoctorID", System.Data.SqlDbType.Int) { Value = DoctorId },
                  new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = Id }
                  );

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = planList });
        }
        [HttpPost, Route("api/DoctorBooking/OrganisationList/{DoctorId}")]
        public HttpResponseMessage OrganisationList(int DoctorId)
        {
            var OrganizationList = _repo.ExecWithStoreProcedure<OrgDoctorsDropDownViewModels>("GetOrganizationByDoctor @DoctorId",
               new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = DoctorId }
               ).Select(x => new OrgDoctorsDropDownViewModels()
               {
                   OrganizationId = x.OrganizationId,
                   DisplayName = x.DisplayName
               });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = OrganizationList });
        }
        [HttpPost, Route("api/DoctorBooking/Address/{Id}")]
        public HttpResponseMessage OrganisationAddressList(int Id)
        {
            var addresses = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModels>("sprGetOrganiationAddresses @OrganizationId",
               new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = Id }

               );

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = addresses });
        }
        //--- Add Update Doctor Booking Info

        [HttpPost, Route("api/DoctorBooking/save")]
        public HttpResponseMessage SaveDoctorBooking(DoctorBookingUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = ModelState.Errors() });
                var result = _repo.All<Slot>().Where(x => x.ReferenceId == model.DoctorId && x.SlotDate == model.SlotDate && x.SlotTime == model.SlotTime && x.IsDeleted == false);
                var countX = result.Count();
                if (countX > 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Slot Already Booked" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Create " +
                        "@SlotId," +
                        "@SlotDate," +
                        "@SlotTime," +
                        "@ReferenceId," +
                        "@BookedFor," +
                        "@IsBooked," +
                        "@IsEmailReminder," +
                        "@IsTextReminder," +
                        "@IsInsuranceChanged," +
                        "@IsActive," +
                        "@InsurancePlanId," +
                        "@AddressId," +
                        "@Description," +
                        "@CreatedBy," +
                        "@UserTypeID",
                                      new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = model.SlotId > 0 ? model.SlotId : 0 },
                                      new SqlParameter("SlotDate", System.Data.SqlDbType.NVarChar) { Value = model.SlotDate },
                                      new SqlParameter("SlotTime", System.Data.SqlDbType.NVarChar) { Value = model.SlotTime },
                                      new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                      new SqlParameter("BookedFor", System.Data.SqlDbType.Int) { Value = model.BookedFor },
                                      new SqlParameter("IsBooked", System.Data.SqlDbType.Bit) { Value = model.IsBooked },
                                      new SqlParameter("IsEmailReminder", System.Data.SqlDbType.Bit) { Value = model.IsEmailReminder },
                                      new SqlParameter("IsTextReminder", System.Data.SqlDbType.Bit) { Value = model.IsTextReminder },
                                      new SqlParameter("IsInsuranceChanged", System.Data.SqlDbType.Bit) { Value = model.IsInsuranceChanged },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = true },
                                      new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = model.InsurancePlanId },
                                      new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId },
                                      new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = model.Description },
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = model.UserId },
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeID }
                                  );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = "Doctor booking info save successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Doctor booking info saving Error.." });
            }

        }

        //-- Delete Doctor Booking
        [HttpDelete, Route("api/DoctorBooking/{id}/remove")]
        public HttpResponseMessage DeleteDoctorBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Doctor Booking info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Doctor Booking info deleted Error.." });
            }
        }

        #endregion

        #region Languages

        [HttpGet, Route("api/DoctorLanguages/{doctorId}")]
        public HttpResponseMessage GetDoctorLanguageList(int doctorId, Pagination param)
        {
            var languages = _doctor.GetDoctorLanguages(param, doctorId).ToList();
            var total = languages.Count > 0 ? languages.FirstOrDefault().TotalRows : 0;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = total,
                TotalDisplayRecords = total,
                Languages = languages
            });


        }

        [HttpPost, Route("api/DoctorLanguage/save"),]
        public HttpResponseMessage SaveDoctorLanguage(DoctorLanguageViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    if (!ModelState.IsValid)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                    if (model.DoctorLanguageId == 0)
                    {

                        var bResult = _repo.Find<DoctorLanguage>(x => x.LanguageId == model.LanguageId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {

                            txscope.Dispose();
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = " Language already exists. Could not add the data!" });
                        }

                        var Language = new DoctorLanguage()
                        {
                            DoctorLanguageId = model.DoctorLanguageId,
                            LanguageId = Convert.ToInt16(model.LanguageId),
                            DoctorId = model.DoctorId,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };


                        _repo.Insert<DoctorLanguage>(Language, true);
                        // _Language.SaveData();
                        txscope.Complete();
                        return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = " Language save successfully" });
                    }
                    else
                    {

                        var bResultdata = _repo.Find<DoctorLanguage>(x => x.LanguageId == model.LanguageId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                        if (bResultdata != null)
                        {
                            txscope.Dispose();
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = " Language already exists. Could not add the data!" });
                        }

                        var age = _repo.All<DoctorLanguage>().ToList().Where(x => x.DoctorLanguageId == model.DoctorLanguageId).FirstOrDefault();
                        age.LanguageId = Convert.ToInt16(model.LanguageId);

                        _repo.Update<DoctorLanguage>(age, true);
                        // _Language.SaveData();
                        txscope.Complete();
                        return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = " Language update successfully" });
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    //Common.LogError(ex, "AddEditDoctorLanguage-Post");
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpGet, Route("api/DoctorLanguage/{id}")]
        public HttpResponseMessage GetDoctorLanguage(int id)
        {

            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "DoctorLanguageId is required" });

            var result = _repo.ExecWithStoreProcedure<DoctorLanguageViewModel>("spDoctorLanguage @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();

            if (result is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid DoctorLanguageId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Language = result });

        }

        [HttpDelete, Route("api/DoctorLanguage/{id}/remove")]
        public HttpResponseMessage RemoveDoctorLanguage(int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorLanguage @Activity ,@Id  ",
                         new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Delete" },
                      new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                     ;
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Language removed successfully" });
        }

        #endregion

        #region Address
        [HttpGet, Route("api/DoctorAddresses/{doctorId}")]
        public HttpResponseMessage GetDoctorOfficeLocationList(int doctorId, Pagination param)
        {
            var allDoctorAddress = _repo.ExecWithStoreProcedure<OrganisationAddressModel>("spAddressDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = doctorId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = allDoctorAddress
                .Select(x => new DoctorAddressListModel()
                {
                    AddressId = x.AddressId,
                    DoctorId = x.ReferenceId,
                    DoctorName = x.ReferenceName,
                    AddressTypeId = x.AddressTypeId,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    ZipCode = x.ZipCode,
                    City = x.City,
                    State = x.State,
                    Country = x.Country,
                    CreatedDate = x.CreatedDate.UtcToUserTime(),
                    UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    TotalRecordCount = x.TotalRecordCount
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                Addresses = result
            });
        }

        [HttpGet, Route("api/DoctorAddress/{id}")]
        public HttpResponseMessage Address(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "DoctorAddressId is required" });

            Common commonLayer = new Common(_repo);
            var orgAddressInfo = _repo.ExecWithStoreProcedure<OrganisationAddress>("spGetAddressInfoByID @AddressID",
                new SqlParameter("AddressID", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new DoctorAddressModel
                {
                    AddressId = x.AddressId,
                    DoctorId = x.ReferenceId,
                    ReferenceId = x.ReferenceId,
                    AddressTypeId = x.AddressTypeID,
                    // OrganisationName = GetOrganisationNameById(x.ReferenceId),
                    UserTypeID = x.UserTypeID,
                    CityStateZipCodeID = x.CityStateZipCodeID,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    ZipCode = commonLayer.GetCityStateInfoById(x.CityStateZipCodeID, "zip"),
                    City = commonLayer.GetCityStateInfoById(x.CityStateZipCodeID, "city"),
                    State = commonLayer.GetCityStateInfoById(x.CityStateZipCodeID, "state"),
                    Country = commonLayer. GetCityStateInfoById(x.CityStateZipCodeID, "country"),
                    Lat = x.Lat,
                    Lon = x.Lon,
                    Phone = x.Phone,
                    Fax = x.Fax,
                    WebSite = x.WebSite,
                    Email = x.Email,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    CreatedBy = (int)x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    TotalRecordCount = 1
                }).FirstOrDefault();

            if (orgAddressInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid DoctorAddressId" }
);

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Address = orgAddressInfo });
        }


        [HttpPost, Route("api/DoctorAddress/save")]
        public HttpResponseMessage SaveDoctorAddress(DoctorAddressModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_Create " +
                        "@AddressId," +
                        "@ReferenceId," +
                        "@AddressTypeID," +
                        "@Address1," +
                        "@Address2," +
                        "@CityStateZipCodeID," +
                        "@CreatedBy," +
                        "@IsActive," +
                        "@Phone," +
                        "@Fax," +
                        "@Email," +
                        "@WebSite," +
                        "@Lat," +
                        "@Lon," +
                        "@UserTypeID," +
                        "@ModifiedBy",
                                      new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId > 0 ? model.AddressId : 0 },
                                      new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                      new SqlParameter("AddressTypeID", System.Data.SqlDbType.Int) { Value = model.AddressTypeId },
                                      new SqlParameter("Address1", System.Data.SqlDbType.NVarChar) { Value = (object)model.Address1 ?? DBNull.Value },
                                      new SqlParameter("Address2", System.Data.SqlDbType.NVarChar) { Value = (object)model.Address2 ?? DBNull.Value },
                                      new SqlParameter("CityStateZipCodeID", System.Data.SqlDbType.Int) { Value = model.CityStateZipCodeID },
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                      new SqlParameter("Phone", System.Data.SqlDbType.NVarChar) { Value = (object)model.Phone ?? DBNull.Value },
                                      new SqlParameter("Fax", System.Data.SqlDbType.NVarChar) { Value = (object)model.Fax ?? DBNull.Value },
                                      new SqlParameter("Email", System.Data.SqlDbType.NVarChar) { Value = (object)model.Email ?? DBNull.Value },
                                      new SqlParameter("WebSite", System.Data.SqlDbType.VarChar) { Value = (object)model.WebSite ?? DBNull.Value },
                                      new SqlParameter("Lat", System.Data.SqlDbType.Decimal) { Value = (object)model.Lat ?? DBNull.Value },
                                      new SqlParameter("Lon", System.Data.SqlDbType.Decimal) { Value = (object)model.Lon ?? DBNull.Value },
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                                      new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                                  );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Doctor address info save successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Doctor address info saving Error.." });
            }

        }

        [HttpDelete, Route("api/DoctorAddress/{id}/remove")]
        public HttpResponseMessage DeleteDoctorAddress(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_Delete @AddressId, @ModifiedBy",
                    new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Doctor Address has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Doctor Address deleted Error.." });
            }


        }
        #endregion

        #region Featured
        [Route("api/DoctorFeaturedList/{doctorId}")]
        public HttpResponseMessage GetFeaturedList(int doctorId, Pagination param)
        {
            var allFeatured = _repo.ExecWithStoreProcedure<OrganisationFeaturedModel>("spFeatured_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = doctorId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    ).ToList();

            var result = allFeatured
                .Select(x => new OrganisationFeaturedListModel()
                {
                    FeaturedId = x.FeaturedId,
                    DoctorId = x.ReferenceId,
                    DoctorName = x.ReferenceName,
                    ProfileImage = x.ProfileImage,
                    Title = x.Title,
                    strStartDate = x.StartDate.ToDefaultFormate(),
                    strEndDate = x.EndDate != null ? x.EndDate?.ToDefaultFormate() : null,
                    CreatedDate = x.CreatedDate.UtcToUserTime(),
                    UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    TotalRecordCount = x.TotalRecordCount
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                FeaturedList = result
            });
        }

        [Route("api/DoctorFeatured/{id}")]
        public HttpResponseMessage GetFeatured(int id)
        {
            Common commonLayer = new Common(_repo);
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "FeaturedId is required" });

            var orgFeaturedInfo = _repo.ExecWithStoreProcedure<OrganisationFeaturedModel>("spGetFeaturedInfoByID @FeaturedID",
                new SqlParameter("FeaturedID", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new DoctorFeaturedAddEditModel
                {
                    FeaturedId = x.FeaturedId,
                    DoctorId = x.ReferenceId,
                    OrganizationTypeId = x.OrganizationTypeId,
                    DoctorName = GetDoctorNameById(x.ReferenceId),
                    UserTypeId = x.UserTypeId,
                    CityStateZipCodeID = x.CityStateZipCodeID,
                    Title = x.Title,
                    Description = x.Description,
                    FeaturdStartDate = x.StartDate,
                    EndDate = x.EndDate,
                    ProfileImage = x.ProfileImage,
                    AdvertisementLocationID = x.AdvertisementLocationID,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    CreatedBy = (int)x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    ZipCode = x.CityStateZipCodeID > 0 ? commonLayer.GetCityStateInfoById(x.CityStateZipCodeID.Value, "zip") : "",
                    City = x.CityStateZipCodeID > 0 ? commonLayer.GetCityStateInfoById(x.CityStateZipCodeID.Value, "city") : "",
                    State = x.CityStateZipCodeID > 0 ? commonLayer. GetCityStateInfoById(x.CityStateZipCodeID.Value, "state") : "",
                    Country = x.CityStateZipCodeID > 0 ? commonLayer.GetCityStateInfoById(x.CityStateZipCodeID.Value, "country") : ""
                }).FirstOrDefault();

            if (orgFeaturedInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid FeaturedId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Featured = orgFeaturedInfo });
        }

        [HttpPost, Route("api/DoctorFeatured/save")]
        public HttpResponseMessage SaveFeatured([FromBody] DoctorFeaturedAddEditModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });


                if (model.DoctorId > 0 && model.CityStateZipCodeID > 0 && model.FeaturdStartDate != null)
                {
                    string imagePath = "";

                    if (model.ProfilePic != null && model.ProfilePic.Length > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Uploads/FeaturedDoctor/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Uploads/FeaturedDoctor"));
                        }



                        string extension = Path.GetExtension(model.ProfileImage);
                        string newImageName = "Featured-Doctor-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/FeaturedDoctor"), newImageName);

                        using (MemoryStream ms = new MemoryStream(model.ProfilePic))
                        {
                            Image image = Image.FromStream(ms);
                            image.Save(path);
                        }

                        imagePath = newImageName;
                    }

                    if (model.FeaturedId > 0 && imagePath == "")
                    {
                        imagePath = model.ProfileImage;
                    }


                    // Response.Write(dt);
                    //  Response.End();

                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spFeatured_Create " +
                            "@FeaturedId," +
                            "@ReferenceId," +
                            "@ProfileImage," +
                            "@Description," +
                            "@StartDate," +
                            "@EndDate," +
                            "@CreatedBy," +
                            "@UserTypeID," +
                            "@IsActive," +
                            "@TotalImpressions," +
                            "@PaymentTypeID," +
                            "@AdvertisementLocationID," +
                            "@Title," +
                            "@CityStateZipCodeID",
                                          new SqlParameter("FeaturedId", System.Data.SqlDbType.Int) { Value = model.FeaturedId > 0 ? model.FeaturedId : 0 },
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                          new SqlParameter("ProfileImage", System.Data.SqlDbType.VarChar) { Value = (object)(imagePath) ?? DBNull.Value },
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (object)model.Description ?? DBNull.Value },
                                          new SqlParameter("StartDate", System.Data.SqlDbType.Date) { Value = (object)model.FeaturdStartDate },
                                          new SqlParameter("EndDate", System.Data.SqlDbType.Date) { Value = (object)model.EndDate ?? DBNull.Value },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("TotalImpressions", System.Data.SqlDbType.Int) { Value = (object)model.TotalImpressions ?? DBNull.Value },
                                          new SqlParameter("PaymentTypeID", System.Data.SqlDbType.Int) { Value = (object)model.PaymentTypeID ?? DBNull.Value },
                                          new SqlParameter("AdvertisementLocationID", System.Data.SqlDbType.Int) { Value = (object)model.AdvertisementLocationID ?? DBNull.Value },
                                          new SqlParameter("Title", System.Data.SqlDbType.NVarChar) { Value = (object)model.Title ?? DBNull.Value },
                                          new SqlParameter("CityStateZipCodeID", System.Data.SqlDbType.Int) { Value = model.CityStateZipCodeID }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Featured info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name, Start date, Zip code and City" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Featured info saving Error.." });
            }

        }

        [HttpDelete, Route("api/DoctorFeatured/{id}/remove")]
        public HttpResponseMessage RemoveFeatured(int id)
        {
            try
            {
                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "FeaturedId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spFeatured_Delete @FeaturedId, @ModifiedBy",
                    new SqlParameter("FeaturedId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Featured has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Featured deleted Error.." });
            }
        }

        #endregion

        #region Speciality
        [HttpGet, Route("api/GetAllSpecialities")]
        public HttpResponseMessage GetAllSpecialities(Pagination objPagination)
        {
            var specialitieslist = _repo.ExecWithStoreProcedure<Speciality>("sprSpecialities");

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = string.Empty, Specialities = specialitieslist });
        }
        #endregion

        [HttpGet, Route("api/InsurancePlans")]
        public HttpResponseMessage InsurancePlanList()
        {
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModels>("spInsurancePlanDropDownList_Get").ToList();
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = planList });
        }

        private string GetDoctorNameById(int id)
        {
            string result = "";

            var info = _repo.SQLQuery<DoctorViewModel>("spDoctor @Activity ,@Id",
                new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id }).FirstOrDefault();

            result = string.Join(" ", info.FirstName, info.LastName);

            return result;
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

            //VisitorsIPAddr = "68.37.76.206";
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

    }
}
