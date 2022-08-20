using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Enumerable;
using Binke.Api.Utility;
using System.IO;
using System.Drawing;

namespace Binke.Api
{
   
    public class OrganizationController : BaseApiController
    {
        private readonly IFacilityService _facility;
        private readonly IRepository _repo;
        public OrganizationController(IFacilityService facility, IRepository repo)
        {
            _facility = facility;
            _repo = repo;
        }


        #region Organization Review

        [Route("api/Organization/{organizationId}/reviews")]
        public HttpResponseMessage GetOrganizationReviews(int organizationId, Pagination param)
        {
            var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = param.Search ?? ""},
                    new SqlParameter("@OrganizationId",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@Sort",SqlDbType.VarChar) {Value = param.SortDirection ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = param.StartIndex==0?0:param.StartIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =param.PageSize},
                    new SqlParameter("@UserTypeID",SqlDbType.Int){ Value = param.UserTypeId}
                };

            int totalRecord = 0;
            var ReviewProviderList = _facility.GetReviewListByTypeId(StoredProcedureList.GetReview, parameters, out totalRecord).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = totalRecord,
                TotalDisplayRecords = totalRecord,
                Reviews = ReviewProviderList
            });
        }

        [Route("api/Organization/{organizationId}/review/{id}")]
        public HttpResponseMessage GetReviewById(int organizationId, int? id)
        {

            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "ReviewId is required" });

            var orgInfo = _facility.ExecWithStoreProcedure<OrgReviewViewModel>("spReview_GetById @ReviewId",
                new SqlParameter("ReviewId", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrgReviewUpdateViewModel
                {
                    ReviewId = x.ReviewId,
                    OrganisationId = x.ReferenceId,
                    OrganisationName = x.RefrenceName,
                    OrganizationTypeId = x.OrganizatonTypeID,
                    Description = x.Description,
                    Rating = x.Rating,
                    IsActive = x.IsActive
                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid ReviewId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Review = orgInfo });
        }


        [HttpDelete, Route("api/Organization/{organizationId}/review/{id}/remove")]
        public HttpResponseMessage DeleteReview(int id)
        {
            try
            {
                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "ReviewId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spReview_Remove @ReviewId, @ModifiedBy",
                    new SqlParameter("ReviewId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Review has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Review deleted.." });
            }
        }

        //--- Add/Edit Review
        [HttpPost, Route("api/Organization/review/save")]
        public HttpResponseMessage SaveReview(OrgReviewUpdateViewModel model)
        {

            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0)
                {
                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spReview_Create " +
                            "@ReviewId," +
                            "@ReferenceId," +
                            "@Description," +
                            "@Rating," +
                            "@IsActive," +
                            "@IsDeleted," +
                            "@CreatedBy," +
                            "@ApplicationUser_Id," +
                            "@Doctor_DoctorId," +
                            "@SeniorCare_SeniorCareId," +
                            "@UserTypeID",
                                          new SqlParameter("ReviewId", System.Data.SqlDbType.BigInt) { Value = model.ReviewId > 0 ? model.ReviewId : 0 },
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (object)model.Description ?? DBNull.Value },
                                          new SqlParameter("Rating", System.Data.SqlDbType.Int) { Value = (object)model.Rating ?? DBNull.Value },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive ?? DBNull.Value },
                                          new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                          new SqlParameter("Doctor_DoctorId", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                          new SqlParameter("SeniorCare_SeniorCareId", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Review info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Review info saving Error.." });
            }

        }
        #endregion

        #region Organization State License

        [Route("api/Organization/{organizationId}/StateLicenses")]
        public HttpResponseMessage GetStateLicenseList(int organizationId, Pagination param)
        {
            var result = _repo.ExecWithStoreProcedure<OrgStateLicenseModel>("spDocOrgStateLicenses_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = param.UserTypeId },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    ).Select(x => new OrgStateLicenseListModel
                    {
                        DocOrgStateLicenseId = x.DocOrgStateLicense,
                        OrganisationName = x.ReferenceName,
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
                StateLicences = result
            });
        }

        //---- Get State License Details

        [Route("api/Organization/{organizationId}/StateLicense/{Id}")]
        public HttpResponseMessage GetStateLicense(int organizationId, int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "StateLicenseId is required" });

            var orgLicenseInfo = _repo.ExecWithStoreProcedure<OrgStateLicenseModel>("spDocOrgStateLicenses_GetById @DocOrgStateLicense",
                              new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id }
                              ).Select(x => new OrgStateLicenseUpdateModel
                              {
                                  DocOrgStateLicenseId = x.DocOrgStateLicense,
                                  OrganisationId = x.ReferenceId,
                                  OrganizationTypeId = x.OrganizationTypeId,
                                  OrganisationName = x.ReferenceName,
                                  UserTypeId = x.UserTypeId,
                                  HealthCareProviderTaxonomyCode = x.HealthCareProviderTaxonomyCode,
                                  ProviderLicenseNumber = x.ProviderLicenseNumber,
                                  ProviderLicenseNumberStateCode = x.ProviderLicenseNumberStateCode,
                                  HealthcareProviderPrimaryTaxonomySwitch = x.HealthcareProviderPrimaryTaxonomySwitch,
                                  IsDeleted = x.IsDeleted.HasValue ? x.IsDeleted.Value : false,
                                  IsActive = x.IsActive.HasValue ? x.IsActive.Value : false
                              }).FirstOrDefault();

            if (orgLicenseInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid StateLicenseId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, StateLicence = orgLicenseInfo });

        }

        [HttpDelete, Route("api/Organization/{organizationId}/StateLicense/{id}/remove")]
        public HttpResponseMessage RemoveStateLicense(int id)
        {
            try
            {
                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "StateLicenseId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgStateLicenses_Remove @DocOrgStateLicense",
                    new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"StateLicense has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"StateLicense deleted.." });
            }
        }
        //--Update State License

        [HttpPost, Route("api/Organization/StateLicense/save")]
        public HttpResponseMessage SaveStateLicense(OrgStateLicenseUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0)
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
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                          new SqlParameter("HealthCareProviderTaxonomyCode", System.Data.SqlDbType.NVarChar) { Value = (object)model.HealthCareProviderTaxonomyCode ?? DBNull.Value },
                                          new SqlParameter("ProviderLicenseNumber", System.Data.SqlDbType.NVarChar) { Value = (object)model.ProviderLicenseNumber ?? DBNull.Value },
                                          new SqlParameter("ProviderLicenseNumberStateCode", System.Data.SqlDbType.NVarChar) { Value = (object)model.ProviderLicenseNumberStateCode ?? DBNull.Value },
                                          new SqlParameter("HealthcareProviderPrimaryTaxonomySwitch", System.Data.SqlDbType.Bit) { Value = model.HealthcareProviderPrimaryTaxonomySwitch ?? false },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 3 }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "State License info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "State License info saving Error.." });
            }

        }

        #endregion

        #region Organization Images
        [Route("api/Organization/{organizationId}/images")]
        public HttpResponseMessage GetImageList(int organizationId, Pagination param)
        {
            var result = _repo.ExecWithStoreProcedure<OrgSiteImageModel>("spOrganizatonSiteImage_Get @Search, @ReferenceId, @UserTypeID, @OrganizatonTypeID, @PageIndex, @PageSize, @Sort",
                   new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                   new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                   new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                   new SqlParameter("OrganizatonTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                   new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                   new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                   new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                   ).Select(x => new OrgSiteImageListModel
                   {
                       SiteImageId = x.SiteImageId,
                       OrganisationName = x.ReferenceName,
                       OrganisationId = x.ReferenceId,
                       ImagePath = x.ImagePath,
                       Name = x.Name,
                       CreatedDate = x.CreatedDate.UtcToUserTime(),
                       UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
                       IsProfile = x.IsProfile,
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
                Images = result
            });

        }

        [Route("api/Organization/{organizationId}/image/{id}")]
        public HttpResponseMessage GetSiteImage(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "SiteImageId is required" });

            var orgInfo = _repo.ExecWithStoreProcedure<OrgSiteImageModel>("spOrganizatonSiteImage_GetById @SiteImageId",
                new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrgSiteImageUpdateModel
                {
                    SiteImageId = x.SiteImageId,
                    OrganisationId = x.ReferenceId,
                    OrganisationName = x.ReferenceName,
                    Name = x.Name,
                    OrganizationTypeId = x.OrganizationTypeId,
                    ImagePath = x.ImagePath,
                    IsProfile = x.IsProfile,
                    IsActive = x.IsActive
                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid SiteImageId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Image = orgInfo });

        }

        [HttpPost, Route("api/Organization/{organizationId}/image/save")]
        public HttpResponseMessage SaveSiteImage(OrgSiteImageUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0)
                {
                    string imagePath = "";

                    if (model.Image != null && model.Image.Length > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Uploads/OrganizationSiteImages/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Uploads/OrganizationSiteImages"));
                        }

                        string extension = Path.GetExtension(model.ImagePath);
                        string newImageName = "Organization-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/OrganizationSiteImages"), newImageName);

                        using (MemoryStream ms = new MemoryStream(model.Image))
                        {
                            Image image = Image.FromStream(ms);
                            image.Save(path);
                        }

                        imagePath = newImageName;
                    }

                    if (model.SiteImageId > 0 && imagePath == "")
                    {
                        imagePath = model.ImagePath;
                    }

                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizatonSiteImage_Create " +
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
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeId },
                                          new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = model.Name }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Site image info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Name" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Site image info saving Error.." });
            }

        }

        [Route("api/Organization/{organizationId}/image/{id}/remove")]
        [HttpDelete]
        public HttpResponseMessage DeleteSiteImage(int id)
        {
            try
            {
                var orgInfo = _repo.ExecWithStoreProcedure<OrgSiteImageModel>("spOrganizatonSiteImage_GetById @SiteImageId",
                new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrgSiteImageUpdateModel
                {
                    SiteImageId = x.SiteImageId,
                    OrganisationId = x.ReferenceId,
                    OrganisationName = x.ReferenceName,
                    Name = x.Name,
                    OrganizationTypeId = x.OrganizationTypeId,
                    ImagePath = x.ImagePath,
                    IsProfile = x.IsProfile,
                    IsActive = x.IsActive
                }).FirstOrDefault();

                if (orgInfo is null)
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid ImageId" });

                if (orgInfo.ImagePath != null)
                {
                    var path = HttpContext.Current.Server.MapPath("~/Uploads/SiteImages/" + orgInfo.ImagePath);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizatonSiteImage_Remove @SiteImageId, @ModifiedBy",
                    new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The image has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"image deleted Error.." });
            }
        }
        #endregion

        #region Insurance
        [Route("api/Organization/{organizationId}/Insurances")]
        public HttpResponseMessage GetInsuranceList(int organizationId, Pagination param)
        {
            var result = _repo.ExecWithStoreProcedure<OrgInsuranceModel>("spOrganisationInsurances_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    ).Select(x => new OrgInsuranceListModel
                    {
                        DocOrgInsuranceId = x.DocOrgInsuranceId,
                        OrganisationName = x.ReferenceName,
                        Name = x.Name,
                        InsuranceTypeId = x.InsuranceTypeId,
                        TypeName = x.TypeName,
                        StateId = x.StateId,
                        InsuranceIdentifierId = x.InsuranceIdentifierId,
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
                Insurances = result
            });
        }

        [Route("api/Organization/{organizationId}/Insurance/{id}")]
        public HttpResponseMessage GetInsurance(int? id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "InsuranceId is required");

            var orgLicenseInfo = _repo.ExecWithStoreProcedure<OrgInsuranceModel>("spOrganisationInsurances_GetById @DocOrgInsuranceId",
                new SqlParameter("DocOrgInsuranceId", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrgInsuranceUpdateModel
                {
                    DocOrgInsuranceId = x.DocOrgInsuranceId,
                    OrganisationId = x.ReferenceId,
                    InsurancePlanId = x.InsurancePlanId,
                    OrganizationTypeId = x.OrganizationTypeId,
                    OrganisationName = x.ReferenceName,
                    UserTypeId = x.UserTypeId,
                    Name = x.Name,
                    StateId = x.StateId,
                    InsuranceIdentifierId = x.InsuranceIdentifierId,
                    UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
                    InsuranceTypeId = x.InsuranceTypeId,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive
                }).FirstOrDefault();

            if (orgLicenseInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                Insurance = orgLicenseInfo
            });
        }

        [HttpPost, Route("api/Organization/Insurance/save")]
        public HttpResponseMessage SaveInsurance(OrgInsuranceUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0)
                {
                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizatonInsurances_Create " +
                            "@DocOrgInsuranceId," +
                            "@ReferenceId," +
                            "@InsurancePlanId," +
                            "@InsuranceIdentifierId," +
                            "@StateId," +
                            "@IsActive," +
                            "@CreatedBy," +
                            "@UserTypeID",
                                          new SqlParameter("DocOrgInsuranceId", System.Data.SqlDbType.Int) { Value = model.DocOrgInsuranceId > 0 ? model.DocOrgInsuranceId : 0 },
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                          new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = (object)model.InsurancePlanId ?? DBNull.Value },
                                          new SqlParameter("InsuranceIdentifierId", System.Data.SqlDbType.VarChar) { Value = (object)model.InsuranceIdentifierId ?? DBNull.Value },
                                          new SqlParameter("StateId", System.Data.SqlDbType.NVarChar) { Value = (object)model.StateId ?? DBNull.Value },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive ?? DBNull.Value },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Insurance info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Insurance info saving Error.." });
            }

        }

        [Route("api/Organization/{organizationId}/Insurance/{id}/remove")]
        [HttpPost]
        public HttpResponseMessage RemoveInsurance(int id)
        {
            try
            {

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizatonInsurance_Remove @DocOrgInsuranceId, @ModifiedBy",
                    new SqlParameter("DocOrgInsuranceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Insurance Info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Insurance Info deleted Error.." });
            }
        }
        #endregion

        #region Opening Hours
        public string getWeekDayName(int day)
        {
            string DayName = "";

            if (day == 1)
                DayName = "Monday";
            if (day == 2)
                DayName = "Tuesday";
            if (day == 3)
                DayName = "Wednesday";
            if (day == 4)
                DayName = "Thursday";
            if (day == 5)
                DayName = "Friday";
            if (day == 6)
                DayName = "Saturday";
            if (day == 7)
                DayName = "Sunday";

            return DayName;
        }

        [Route("api/Organization/{organizationId}/OpeningHours")]
        public HttpResponseMessage GetOpeningHour(int organizationId)
        {
            if (organizationId == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "OrganizationId is required" });

            var orgInfo = _repo.ExecWithStoreProcedure<OrgOpeningHoursUpdateModel>("spOrgOpeningHour_GetByOrgID @OrganizationID",
                new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = organizationId }
                );

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid OrganizationId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, OpeningHours = orgInfo });
        }

        [HttpPost, Route("api/Organization/{organizationId}/OpeningHours/save")]
        public HttpResponseMessage SaveOpeningHours(OrgOpeningHoursUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganizationID > 0)
                {
                    for (int i = 0; i < 7; i++)
                    {

                        int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrgOpeningHour_Create " +
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
                                               new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = false },
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                              new SqlParameter("Comments", System.Data.SqlDbType.NVarChar) { Value = (object)model.Comments[i] ?? DBNull.Value }
                                          );

                        continue;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Opening Hours info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Info Not Found! Should be select Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "OpeningHours info saving Error.." });
            }

        }

        #endregion

        #region Featured
        [Route("api/Organization/{organizationId}/FeaturedList")]
        public HttpResponseMessage GetFeaturedList(int organizationId, Pagination param)
        {
            var allFeatured = _repo.ExecWithStoreProcedure<OrganisationFeaturedModel>("spFeatured_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = allFeatured
                .Select(x => new OrganisationFeaturedListModel()
                {
                    FeaturedId = x.FeaturedId,
                    OrganisationId = x.ReferenceId,
                    OrganisationName = x.ReferenceName,
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

        [Route("api/Organization/{organizationId}/Featured/{id}")]
        public HttpResponseMessage GetFeatured(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "FeaturedId is required" });

            var orgFeaturedInfo = _repo.ExecWithStoreProcedure<OrganisationFeaturedModel>("spGetFeaturedInfoByID @FeaturedID",
                new SqlParameter("FeaturedID", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrganisationFeaturedAddEditModel
                {
                    FeaturedId = x.FeaturedId,
                    OrganisationId = x.ReferenceId,
                    OrganizationTypeId = x.OrganizationTypeId,
                    OrganisationName = GetOrganisationNameById(x.ReferenceId),
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
                    ZipCode = x.CityStateZipCodeID > 0 ? GetCityStateInfoById(x.CityStateZipCodeID.Value, "zip") : "",
                    City = x.CityStateZipCodeID > 0 ? GetCityStateInfoById(x.CityStateZipCodeID.Value, "city") : "",
                    State = x.CityStateZipCodeID > 0 ? GetCityStateInfoById(x.CityStateZipCodeID.Value, "state") : "",
                    Country = x.CityStateZipCodeID > 0 ? GetCityStateInfoById(x.CityStateZipCodeID.Value, "country") : ""
                }).FirstOrDefault();

            if (orgFeaturedInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid FeaturedId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Featured = orgFeaturedInfo });
        }

        [HttpPost, Route("api/Organization/{organizationId}/Featured/save")]
        public HttpResponseMessage SaveFeatured(OrganisationFeaturedAddEditModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0 && model.CityStateZipCodeID > 0 && model.FeaturdStartDate != null)
                {
                    string imagePath = "";

                    if (model.Image != null && model.Image.Length > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Uploads/FeaturedOrganization/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Uploads/FeaturedOrganization"));
                        }

                        string extension = Path.GetExtension(model.ProfileImage);
                        string newImageName = "Featured-Organization-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/FeaturedOrganization"), newImageName);

                        using (MemoryStream ms = new MemoryStream(model.Image))
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
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
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

        [HttpDelete, Route("api/Organization/{organizationId}/Featured/{id}/remove")]
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

        public string GetCityStateInfoById(int id, string type)
        {
            string result = "";

            var info = _repo.SQLQuery<CityStateInfoByZipCodeModel>("spGetCityStateZipInfoByID @ID", new SqlParameter("ID", System.Data.SqlDbType.Int) { Value = id }).ToList();

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

        public string GetOrganisationNameById(int id)
        {
            string result = "";

            var info = _repo.SQLQuery<OrganizationViewModel>("spGetOrganizationInfoByID @orgnizationID", new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = id }).ToList();

            result = info[0].OrganisationName;

            return result;
        }
        #endregion

        #region Amenity Options
        [Route("api/Organization/{organizationId}/AmenityOptions")]
        public HttpResponseMessage GetAmenityOptionsList(int organizationId, Pagination param)
        {
            var allPharmacy = _repo.ExecWithStoreProcedure<OrganizationAmenityOptionsModel>("spAllOrganizationAmenityOption_Get @Search, @UserTypeID, @OrganizationID, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = allPharmacy
                .Select(x => new OrganizationAmenityOptionListModel()
                {
                    OrganizationAmenityOptionID = x.OrganizationAmenityOptionID,
                    OrganizationID = x.OrganizationID,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeId = x.OrganizationTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    IsOption = x.IsOption,
                    UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    ImagePath = x.ImagePath,
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
                AmenityOptions = result
            });
        }

        [Route("api/Organization/{organizationId}/AmenityOption/{id}")]
        public HttpResponseMessage GetAmenityOption(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "AmenityOptionId is required" });

            var orgFeaturedInfo = _repo.ExecWithStoreProcedure<OrganizationAmenityOptionsModel>("spAllOrganizationAmenityOption_GetById @OrganizationAmenityOptionID",
                new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrganizationAmenityOptionsUpdateModel
                {
                    OrganizationAmenityOptionID = x.OrganizationAmenityOptionID,
                    OrganizationID = x.OrganizationID,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeId = x.OrganizationTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    IsOption = x.IsOption,
                    ImagePath = x.ImagePath,
                    UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                }).FirstOrDefault();

            if (orgFeaturedInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid AmenityOptionId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, AmenityOption = orgFeaturedInfo });
        }

        [HttpPost, Route("api/Organization/{organizationId}/AmenityOption/save")]
        public HttpResponseMessage SaveAmenityOptions(OrganizationAmenityOptionsUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganizationID > 0 && model.Name != null)
                {
                    string imagePath = "";

                    if (model.Image != null && model.Image.Length > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Uploads/OrganizationAmenityOptions/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Uploads/OrganizationAmenityOptions"));
                        }

                        string extension = Path.GetExtension(model.ImagePath);
                        string newImageName = "AmenityOpt-Organization-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/OrganizationAmenityOptions"), newImageName);

                        using (MemoryStream ms = new MemoryStream(model.Image))
                        {
                            Image image = Image.FromStream(ms);
                            image.Save(path);
                        }

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
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive }
                                      );





                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Amenity Option info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name, Name" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Amenity Option info saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/AmenityOption/{id}/remove")]
        public HttpResponseMessage DeletePharmacyAmenityOptions(int id)
        {
            try
            {
                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "AmenityOptionId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAllOrganizationAmenityOption_Delete @OrganizationAmenityOptionID, @ModifiedBy",
                    new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Amenity Option has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Amenity Option deleted Error.." });
            }
        }
        #endregion

        #region Taxonomy
        [HttpGet, Route("api/Organization/{organizationId}/TaxonomyList")]
        public HttpResponseMessage GetTaxonomyList(int organizationId, Pagination param)
        {
            var Info = _repo.ExecWithStoreProcedure<OrganisationTaxonomyModel>("spDocOrgTaxonomy_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = Info
                .Select(x => new OrganisationTaxonomyListModel()
                {
                    DocOrgTaxonomyID = x.DocOrgTaxonomyID,
                    TaxonomyID = x.TaxonomyID,
                    ReferenceID = x.ReferenceID,
                    OrganisationId = x.ReferenceID,
                    Taxonomy_Code = x.Taxonomy_Code,
                    Specialization = x.Specialization,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeId = x.OrganizationTypeId,
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
                TaxonomyList = result
            });
        }

        [HttpGet, Route("api/Organization/{organizationId}/Taxonomy/{id}")]
        public HttpResponseMessage GetTaxonomy(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "TaxonomyId is required" });

            var orgTaxonomyInfo = _repo.ExecWithStoreProcedure<OrganisationTaxonomyModel>("spDocOrgTaxonomy_GetById @DocOrgTaxonomyID",
                new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrganisationTaxonomyUpdateModel
                {
                    DocOrgTaxonomyID = x.DocOrgTaxonomyID,
                    TaxonomyID = x.TaxonomyID,
                    OrganisationId = x.ReferenceID,
                    Taxonomy_Code = x.Taxonomy_Code,
                    Specialization = x.Specialization,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeId = x.OrganizationTypeId,
                    UserTypeId = x.UserTypeId,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                }).First();

            if (orgTaxonomyInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid TaxonomyId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Taxonomy = orgTaxonomyInfo });
        }

        [HttpPost, Route("api/Organization/{organizationId}/Taxonomy/save")]
        public HttpResponseMessage SaveTaxonomy(OrganisationTaxonomyUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgTaxonomy_Create " +
                        "@DocOrgTaxonomyID," +
                        "@ReferenceID," +
                        "@TaxonomyID," +
                        "@IsActive," +
                        "@CreatedBy," +
                        "@UserTypeId",
                                      new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = model.DocOrgTaxonomyID > 0 ? model.DocOrgTaxonomyID : 0 },
                                      new SqlParameter("ReferenceID", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                      new SqlParameter("TaxonomyID", System.Data.SqlDbType.Int) { Value = model.TaxonomyID },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.VarChar) { Value = model.IsActive },
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                      new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 3 }
                                  );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Taxonomy info save successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Taxonomy info saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/Taxonomy/{id}/remove")]
        public HttpResponseMessage RemoveTaxonomy(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgTaxonomy_Remove @DocOrgTaxonomyID",
                    new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Taxonomy info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Taxonomy info deleted Error.." });
            }
        }
        #endregion

        #region Social Media
        [HttpGet, Route("api/Organization/{organizationId}/SocialMediaList")]
        public HttpResponseMessage GetSocialMediaList(int organizationId, Pagination param)
        {
            var socialMediaInfo = _repo.ExecWithStoreProcedure<OrgSocialMediaModel>("spSocialMedia_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = socialMediaInfo
                .Select(x => new OrgSocialMediaListModel()
                {
                    SocialMediaId = x.SocialMediaId,
                    OrganisationId = (int)x.ReferenceId,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeId = x.OrganizationTypeId,
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


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                SocialMedias = result
            });
        }

        [HttpGet, Route("api/Organization/{organizationId}/SocialMedia/{id}")]
        public HttpResponseMessage GetSocialMedia(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "SocialMediaId is required" });

            var orgSocialMediaInfo = _repo.ExecWithStoreProcedure<OrgSocialMediaModel>("spSocialMedia_GetById @SocialMediaId",
                new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrgSocialMediaUpdateModel
                {
                    SocialMediaId = x.SocialMediaId,
                    OrganisationId = (int)x.ReferenceId,
                    OrganizationTypeId = x.OrganizationTypeId,
                    OrganisationName = GetOrganisationNameById((int)x.ReferenceId),
                    UserTypeId = 3,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Facebook = x.Facebook,
                    Twitter = x.Twitter,
                    LinkedIn = x.LinkedIn,
                    Instagram = x.Instagram,
                    Youtube = x.Youtube
                }).First();

            if (orgSocialMediaInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid SocialMediaId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, SocialMedia = orgSocialMediaInfo });

        }

        [HttpPost, Route("api/Organization/{organizationId}/SocialMedia/save")]
        public HttpResponseMessage SaveSocialMedia(OrgSocialMediaUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0)
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
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                          new SqlParameter("Facebook", System.Data.SqlDbType.VarChar) { Value = (object)(model.Facebook) ?? DBNull.Value },
                                          new SqlParameter("Twitter", System.Data.SqlDbType.VarChar) { Value = (object)model.Twitter ?? DBNull.Value },
                                          new SqlParameter("LinkedIn", System.Data.SqlDbType.VarChar) { Value = (object)model.LinkedIn ?? DBNull.Value },
                                          new SqlParameter("Instagram", System.Data.SqlDbType.VarChar) { Value = (object)model.Instagram ?? DBNull.Value },
                                          new SqlParameter("Youtube", System.Data.SqlDbType.VarChar) { Value = (object)model.Youtube ?? DBNull.Value },
                                          new SqlParameter("Pinterest", System.Data.SqlDbType.VarChar) { Value = (object)model.Pinterest ?? DBNull.Value },
                                          new SqlParameter("Tumblr", System.Data.SqlDbType.VarChar) { Value = (object)model.Tumblr ?? DBNull.Value },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 3 },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Social media info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Info Not Found! Should be select Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Social media info saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/SocialMedia/{id}/remove")]
        public HttpResponseMessage RemoveSocialMedia(int id)
        {
            try
            {

                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "SocialMediaId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSocialMedia_Remove @SocialMediaId",
                    new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = id }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Social media info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Social media info deleted Error.." });
            }
        }
        #endregion

        #region Booking
        [Route("api/Organization/{organizationId}/BookingList")]
        public HttpResponseMessage GetBookingList(int organizationId, Pagination param)
        {
            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spDocOrgBooking_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = orgInfo
                .Select(x => new OrgBookingListModel()
                {
                    SlotId = x.SlotId,
                    DoctorId = x.DoctorId,
                    OrganisationName = x.OrganisationName,
                    OrganisationId = x.OrganisationId ?? 0,
                    DoctorName = x.DoctorName + " [" + x.Credential + "]",
                    OrganizationTypeId = x.OrganizationTypeId??0,
                    UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
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
                Bookings = result
            });
        }

        [Route("api/Organization/{organizationId}/Booking/{id}")]
        public HttpResponseMessage GetBooking(int organizationId, int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "SlotId is required" });

            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spDocOrgBooking_GetById @SlotId, @OrganizationId",
                new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = organizationId }
                ).Select(x => new OrgBookingUpdateModel
                {
                    SlotId = x.SlotId,
                    OrganisationId = x.OrganisationId ?? 0,
                    DoctorId = x.DoctorId,
                    AddressId = x.AddressId,
                    BookedFor = x.BookedFor,
                    OrganizationTypeId = x.OrganizationTypeId ?? 0,
                    OrganisationName = x.OrganisationName,
                    FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserTypeId = 3,
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
                    InsuranceTypeId = x.InsuranceTypeId
                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid SlotId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Booking = orgInfo });
        }

        [HttpPost, Route("api/Organization/{organizationId}/Booking/save")]
        public HttpResponseMessage SaveBooking(OrgBookingUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0)
                {
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
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = model.InsurancePlanId },
                                          new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId },
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = model.Description },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }

                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Booking info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Organization Info Not Found! Should be select Organization Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Booking info saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/Booking/{id}/remove")]
        public HttpResponseMessage DeleteBooking(int id)
        {
            try
            {
                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "SlotIs is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Booking info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Booking info deleted Error.." });
            }
        }
        #endregion

        #region Profile
        [HttpGet, Route("api/Organizations")]
        public HttpResponseMessage GetOrganizationList(Pagination param)
        {
            var allPharmacys = _repo.ExecWithStoreProcedure<OrganisationProfileModel>("spOrganisation_GetByAddressTypeID @Search, @UserTypeID, @OrganizationTypeID, @AddressTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("AddressTypeID", System.Data.SqlDbType.Int) { Value = 12 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = allPharmacys
                .Select(x => new OrganisationProfileListModel()
                {
                    Id = x.OrganisationId,
                    PharmacyName = x.OrganisationName,
                    FullName = (x.AuthorizedOfficialFirstName != null ? x.AuthorizedOfficialFirstName : "") + ((x.AuthorizedOfficialFirstName != null && x.AuthorizedOfficialLastName != null) ? (", " + x.AuthorizedOfficialLastName) : " " + x.AuthorizedOfficialLastName),
                    NPI = x.NPI,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State,
                    CreatedDate = x.CreatedDate.UtcToUserTime(),
                    UpdatedDate = x.UpdatedDate?.UtcToUserTime(),
                    IsActive = x.IsActive,
                    EnabledBooking = x.EnabledBooking,
                    IsDeleted = x.IsDeleted,
                    totRows = x.TotalRecordCount
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].totRows;


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                OrganizationList = result
            });

        }

        [HttpGet, Route("api/Organization/{organizationId}")]
        public HttpResponseMessage GetOrganizationProfile(int organizationId)
        {
            if (organizationId == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "OrganizationId is required" });

            var orgInfo = _repo.ExecWithStoreProcedure<OrganisationProfileModel>("spGetOrganizationInfoByID @orgnizationID",
                new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = organizationId }
                ).Select(x => new OrganisationProfileModel
                {
                    OrganisationId = x.OrganisationId,
                    UserId = x.UserId,
                    OrganizationTypeID = x.OrganizationTypeID,
                    OrganisationName = x.OrganisationName,
                    UserTypeID = x.UserTypeID,
                    NPI = x.NPI,
                    OrganisationSubpart = x.OrganisationSubpart,
                    AliasBusinessName = x.AliasBusinessName,
                    LogoFilePath = x.LogoFilePath,
                    OrganizatonEIN = x.OrganizatonEIN,
                    EnumerationDate = x.EnumerationDate,
                    Status = x.Status,
                    AuthorisedOfficialCredential = x.AuthorisedOfficialCredential,
                    AuthorizedOfficialFirstName = x.AuthorizedOfficialFirstName,
                    AuthorizedOfficialLastName = x.AuthorizedOfficialLastName,
                    AuthorizedOfficialTelephoneNumber = x.AuthorizedOfficialTelephoneNumber,
                    AuthorizedOfficialTitleOrPosition = x.AuthorizedOfficialTitleOrPosition,
                    AuthorizedOfficialNamePrefix = x.AuthorizedOfficialNamePrefix,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    ModifiedBy = x.ModifiedBy,
                    ApplicationUser_Id = x.ApplicationUser_Id,
                    EnabledBooking = x.EnabledBooking,
                    ShortDescription = x.ShortDescription,
                    LongDescription = x.LongDescription
                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid OrganizationId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Organization = orgInfo });
        }

        [HttpPost, Route("api/Organization/save")]
        public HttpResponseMessage SaveOrganizationProfile(OrganisationProfileModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                string strShortDescription = "";
                string strLongDescription = "";
                if (model.ShortDescription != "")
                {
                    strShortDescription = System.Uri.UnescapeDataString(model.ShortDescription);
                    model.ShortDescription = strShortDescription;
                }
                if (model.LongDescription != "")
                {
                    strLongDescription = System.Uri.UnescapeDataString(model.LongDescription);
                    model.LongDescription = strLongDescription;
                }

                int curUserId = 0;

                if (!User.IsInRole("Admin"))
                {
                    curUserId = UserId;
                }
                else
                {
                    if (model.UserId > 0)
                    {
                        curUserId = (int)model.UserId;
                    }
                }

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganisation_Create " +
                        "@OrganisationId," +
                        "@UserId," +
                        "@OrganizationTypeID," +
                        "@UserTypeID," +
                        "@OrganisationName," +
                        "@OrganisationSubpart," +
                        "@EnumerationDate," +
                        "@Status," +
                        "@AuthorisedOfficialCredential," +
                        "@AuthorizedOfficialFirstName," +
                        "@AuthorizedOfficialLastName," +
                        "@AuthorizedOfficialTelephoneNumber," +
                        "@AuthorizedOfficialTitleOrPosition," +
                        "@AuthorizedOfficialNamePrefix," +
                        "@CreatedBy," +
                        "@IsActive," +
                        "@ApplicationUser_Id," +
                        "@AliasBusinessName," +
                        "@OrganizatonEIN," +
                        "@NPI," +
                        "@EnabledBooking," +
                        "@LogoFilePath," +
                        "@ShortDescription," +
                        "@LongDescription",
                                      new SqlParameter("OrganisationId", System.Data.SqlDbType.Int) { Value = model.OrganisationId > 0 ? model.OrganisationId : 0 },
                                      //new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = curUserId },
                                      new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = UserId },
                                      new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = model.OrganizationTypeID },
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeID },
                                      new SqlParameter("OrganisationName", System.Data.SqlDbType.VarChar) { Value = model.OrganisationName },
                                      new SqlParameter("OrganisationSubpart", System.Data.SqlDbType.NChar) { Value = (object)model.OrganisationSubpart ?? DBNull.Value },
                                      new SqlParameter("EnumerationDate", System.Data.SqlDbType.Date) { Value = (object)model.EnumerationDate ?? DBNull.Value },
                                      new SqlParameter("Status", System.Data.SqlDbType.NChar) { Value = " " },
                                      new SqlParameter("AuthorisedOfficialCredential", System.Data.SqlDbType.NChar) { Value = (object)model.AuthorisedOfficialCredential ?? DBNull.Value },
                                      new SqlParameter("AuthorizedOfficialFirstName", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialFirstName ?? DBNull.Value },
                                      new SqlParameter("AuthorizedOfficialLastName", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialLastName ?? DBNull.Value },
                                      new SqlParameter("AuthorizedOfficialTelephoneNumber", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialTelephoneNumber ?? DBNull.Value },
                                      new SqlParameter("AuthorizedOfficialTitleOrPosition", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialTitleOrPosition ?? DBNull.Value },
                                      new SqlParameter("AuthorizedOfficialNamePrefix", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialNamePrefix ?? DBNull.Value },
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                      new SqlParameter("ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = (object)model.ApplicationUser_Id ?? 0 },
                                      new SqlParameter("AliasBusinessName", System.Data.SqlDbType.NVarChar) { Value = (object)model.AliasBusinessName ?? DBNull.Value },
                                      new SqlParameter("OrganizatonEIN", System.Data.SqlDbType.NVarChar) { Value = (object)model.OrganizatonEIN ?? DBNull.Value },
                                      new SqlParameter("NPI", System.Data.SqlDbType.NVarChar) { Value = model.NPI },
                                      new SqlParameter("EnabledBooking", System.Data.SqlDbType.Bit) { Value = (object)model.EnabledBooking ?? DBNull.Value },
                                      new SqlParameter("LogoFilePath", System.Data.SqlDbType.NVarChar) { Value = (object)model.LogoFilePath ?? DBNull.Value },
                                      new SqlParameter("ShortDescription", System.Data.SqlDbType.NVarChar) { Value = (object)model.ShortDescription ?? DBNull.Value },
                                      new SqlParameter("LongDescription", System.Data.SqlDbType.NVarChar) { Value = (object)model.LongDescription ?? DBNull.Value }
                                  );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Organization profile save successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Organization profile saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/remove")]
        public HttpResponseMessage RemoveOrganization(int organizationId)
        {
            try
            {
                if (organizationId == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "OrganizationId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganisation_Remove @OrganisationId",
                    new SqlParameter("OrganisationId", System.Data.SqlDbType.Int) { Value = organizationId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Organization has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Organization profile deleted Error.." });
            }


        }
        #endregion

        #region Organization Address
        [Route("api/Organization/{organizationId}/OfficeLocations")]
        public HttpResponseMessage GetOrganizationOfficeLocationList(int organizationId, Pagination param)
        {
            var allPharmacyAddress = _repo.ExecWithStoreProcedure<OrganisationAddressModel>("spAddress_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = allPharmacyAddress
                .Select(x => new OrganisationAddressListModel()
                {
                    AddressId = x.AddressId,
                    OrganisationId = x.ReferenceId,
                    OrganisationName = x.ReferenceName,
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

        [Route("api/Organization/{organizationId}/OfficeLocation/{id}")]
        public HttpResponseMessage GetOrganizationAddress(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "AddressId is required" });

            var orgAddressInfo = _repo.ExecWithStoreProcedure<OrganisationAddress>("spGetAddressInfoByID @AddressID",
                new SqlParameter("AddressID", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new OrganisationAddressModel
                {
                    AddressId = x.AddressId,
                    OrganisationId = x.ReferenceId,
                    ReferenceId = x.ReferenceId,
                    AddressTypeId = x.AddressTypeID,
                    OrganisationName = GetOrganisationNameById(x.ReferenceId),
                    UserTypeID = x.UserTypeID,
                    CityStateZipCodeID = x.CityStateZipCodeID,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    ZipCode = GetCityStateInfoById(x.CityStateZipCodeID, "zip"),
                    City = GetCityStateInfoById(x.CityStateZipCodeID, "city"),
                    State = GetCityStateInfoById(x.CityStateZipCodeID, "state"),
                    Country = GetCityStateInfoById(x.CityStateZipCodeID, "country"),
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
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid AddressId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Address = orgAddressInfo });
        }

        [HttpPost, Route("api/Organization/{organizationId}/OfficeLocation/save")]
        public HttpResponseMessage SaveOrganizationAddress(OrganisationAddressModel model)
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
                                      new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
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
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                                      new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = model.AddressId > 0 ? UserId : (object)DBNull.Value }
                                  );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Organization address info save successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Organization address info saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/OfficeLocation/{id}/remove")]
        public HttpResponseMessage DeleteOrganizationAddress(int id)
        {
            try
            {

                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "AddressId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_Delete @AddressId, @ModifiedBy",
                    new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Organization Address has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Organization Address deleted Error.." });
            }


        }
        #endregion

        #region cost
        [HttpGet, Route("api/Organization/{organizationId}/CostList")]
        public HttpResponseMessage GetCostList(Pagination param, int organizationId = 0)
        {
            var allCosts = _repo.ExecWithStoreProcedure<CostModel>("spCost_GetList @Search, @UserTypeID, @OrganizationID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = allCosts
                .Select(x => new CostModel()
                {
                    CostID = x.CostID,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description,
                    CreatedDateString = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDateString = x.UpdatedDate?.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    TotalRecordCount = x.TotalRecordCount,
                    ReferenceID = x.ReferenceID
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                CostList = result
            });

        }

        [HttpPost, Route("api/Organization/{organizationId}/Cost/save")]
        public HttpResponseMessage SaveCost(CostModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.ReferenceID > 0)
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
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.ReferenceID },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                                          new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = (object)model.Name ?? DBNull.Value },
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (object)model.Description ?? DBNull.Value },
                                          new SqlParameter("Price", System.Data.SqlDbType.Int) { Value = (object)model.Price ?? DBNull.Value },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive ?? DBNull.Value },
                                          new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId }
                                      );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Cost info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Cost info saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/Cost/{id}/remove")]
        public HttpResponseMessage DeleteCost(int id)
        {
            try
            {
                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "CostId is required" });

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spCost_Remove @CostId, @ModifiedBy",
                    new SqlParameter("CostId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Cost has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Cost deleted Error.." });
            }
        }
        #endregion

        #region Orders
        [HttpPost, Route("api/Organization/{organizationId}/Order/save")]
        public HttpResponseMessage SaveOrder(OrgPatientOrderUpdateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                if (model.OrganisationId > 0)
                {
                    string imagePath = null;

                    if (model.Image != null && model.Image.Length > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Uploads/Prescription/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Uploads/Prescription"));
                        }

                        string extension = Path.GetExtension(model.PrescriptionImage);
                        string newImageName = "Prescription-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Prescription"), newImageName);

                        using (MemoryStream ms = new MemoryStream(model.Image))
                        {
                            Image image = Image.FromStream(ms);
                            image.Save(path);
                        }

                        imagePath = newImageName;
                    }

                    if (model.OrderId > 0 && imagePath == null)
                    {
                        imagePath = model.PrescriptionImage;
                    }


                    var orderData = _repo.ExecWithStoreProcedure<OrgPatientOrderViewModel>("spPatientOrder_Create " +
                                    "@OrderId," +
                                    "@PatientId," +
                                    "@ReferenceId," +
                                    "@UserTypeID," +
                                    "@AddressId," +
                                    "@Date," +
                                    "@Title," +
                                    "@Description," +
                                    "@PrescriptionImage," +
                                    "@TotalPrice," +
                                    "@CouponDiscount," +
                                    "@OtherDiscount," +
                                    "@NetPrice," +
                                    "@OrderStatus," +
                                    "@InsurancePlanId," +
                                    "@CreatedBy," +
                                    "@IsActive",
                                                  new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = model.OrderId > 0 ? model.OrderId : 0 },
                                                  new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = model.PatientId },
                                                  new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                                  new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeID },
                                                  new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId },
                                                  new SqlParameter("Date", System.Data.SqlDbType.DateTime) { Value = (object)model.Date ?? DBNull.Value },
                                                  new SqlParameter("Title", System.Data.SqlDbType.NVarChar) { Value = (object)model.Title ?? DBNull.Value },
                                                  new SqlParameter("Description", System.Data.SqlDbType.NVarChar) { Value = (object)model.Description ?? DBNull.Value },
                                                  new SqlParameter("PrescriptionImage", System.Data.SqlDbType.NVarChar) { Value = (object)imagePath ?? DBNull.Value },
                                                  new SqlParameter("TotalPrice", System.Data.SqlDbType.Decimal) { Value = (object)model.TotalPrice ?? DBNull.Value },
                                                  new SqlParameter("CouponDiscount", System.Data.SqlDbType.Decimal) { Value = (object)model.CouponDiscount ?? DBNull.Value },
                                                  new SqlParameter("OtherDiscount", System.Data.SqlDbType.Decimal) { Value = (object)model.OtherDiscount ?? DBNull.Value },
                                                  new SqlParameter("NetPrice", System.Data.SqlDbType.Decimal) { Value = (object)model.NetPrice ?? DBNull.Value },
                                                  new SqlParameter("OrderStatus", System.Data.SqlDbType.VarChar) { Value = (object)model.OrderStatus ?? DBNull.Value },
                                                  new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = (object)model.InsurancePlanId ?? DBNull.Value },
                                                  new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                                  new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive ?? DBNull.Value }
                                              ).First();

                    long curOrderId = 0;
                    if (model.OrderId == 0)
                        curOrderId = orderData.OrderId;
                    else
                        curOrderId = model.OrderId;

                    if (curOrderId > 0)
                    {
                        foreach (var detail in model.OrderDetails)
                        {
                            int DrugID = detail.DrugId;
                            int DetailsId = Convert.ToInt32(detail.OrderDetailId);
                            string strDescription = detail.Description;
                            decimal unitPrice = detail.UnitPrice ?? 0;
                            int qty = detail.Quantity ?? 0;
                            decimal totPrice = detail.TotalAmount ?? 0;

                            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spPatientOrderDetails_Create " +
                                "@OrderDetailId," +
                                "@OrderId," +
                                "@DrugId," +
                                "@Description," +
                                "@UnitPrice," +
                                "@Quantity," +
                                "@TotalAmount," +
                                "@CreatedBy," +
                                "@IsActive",
                                              new SqlParameter("OrderDetailId", System.Data.SqlDbType.BigInt) { Value = DetailsId },
                                              new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = curOrderId },
                                              new SqlParameter("DrugId", System.Data.SqlDbType.Int) { Value = DrugID },
                                              new SqlParameter("Description", System.Data.SqlDbType.NVarChar) { Value = (object)strDescription ?? DBNull.Value },
                                              new SqlParameter("UnitPrice", System.Data.SqlDbType.Decimal) { Value = unitPrice },
                                              new SqlParameter("Quantity", System.Data.SqlDbType.Int) { Value = qty },
                                              new SqlParameter("TotalAmount", System.Data.SqlDbType.Decimal) { Value = totPrice },
                                              new SqlParameter("NetPrice", System.Data.SqlDbType.Decimal) { Value = model.NetPrice },
                                              new SqlParameter("OrderStatus", System.Data.SqlDbType.VarChar) { Value = model.OrderStatus },
                                              new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = (object)model.InsurancePlanId ?? DBNull.Value },
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = UserId },
                                              new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = 1 }

                                          );
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Organization Order info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Organization Info Not Found! Should be select Organization Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Organization Order info saving Error.." });
            }

        }

        [HttpDelete, Route("api/Organization/{organizationId}/Order/{id}/remove")]
        public HttpResponseMessage DeleteOrder(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spPatientOrder_Delete @OrderId, @ModifiedBy",
                    new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Organization order info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Organization Order info deleted Error.." });
            }
        }

        //-- Delete  Order
        [HttpDelete, Route("api/Organization/{organizationId}/OrderItem/{id}/remove")]
        public HttpResponseMessage DeleteOrderItem(int id)
        {
            try
            {
                var ItemInfo = _repo.ExecWithStoreProcedure<OrgPatientOrderDetailsViewModel>("spPatientOrderDetails_GetByID @OrderDetailId",
                  new SqlParameter("OrderDetailId", System.Data.SqlDbType.BigInt) { Value = id }
                  ).Select(x => new OrgPatientOrderDetailsViewModel()
                  {
                      OrderId = x.OrderId,
                      TotalAmount = x.TotalAmount
                  }).First();

                long OrderId = ItemInfo.OrderId;
                decimal amount = (decimal)ItemInfo.TotalAmount;


                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spPatientOrderDetails_Delete @OrderDetailId, @OrderId, @Amount,  @ModifiedBy",
                    new SqlParameter("OrderDetailId", System.Data.SqlDbType.BigInt) { Value = id },
                    new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = OrderId },
                    new SqlParameter("Amount", System.Data.SqlDbType.Decimal) { Value = amount },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"The Organization order item has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Organization Order item deleted Error.." });
            }
        }

        [HttpGet, Route("api/Organization/{organizationId}/Orders")]
        public HttpResponseMessage GetPharmacyPatientOrderList(int organizationId, Pagination param)
        {
            var orgInfo = _repo.ExecWithStoreProcedure<OrgPatientOrderViewModel>("spPatientOrder_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    );

            var result = orgInfo
                .Select(x => new OrgPatientOrderListViewModel()
                {
                    OrderId = x.OrderId,
                    PatientId = x.PatientId,
                    AddressId = x.AddressId,
                    Date = x.Date.ToDefaultFormate(),
                    Title = x.Title,
                    Description = x.Description,
                    PrescriptionImage = x.PrescriptionImage,
                    PatientName = x.FirstName + " " + x.LastName,
                    OrgAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country,
                    OrderStatus = x.OrderStatus,
                    UpdatedDate = x.UpdatedDate?.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    NetPrice = x.NetPrice,
                    TotalRecordCount = x.TotalRecordCount,
                    ReferenceId = x.ReferenceId
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].TotalRecordCount;


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = TotalRecordCount,
                Orders = result
            });
        }

        [HttpGet, Route("api/Organization/{organizationId}/Order/{id}")]
        public HttpResponseMessage GetOrder(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Order Id is required" });


            var orgInfo = _repo.ExecWithStoreProcedure<OrgPatientOrderViewModel>("spPatientOrder_GetByID @OrderId",
                new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = (long)id }
                ).Select(x => new OrgPatientOrderUpdateViewModel
                {
                    OrderId = x.OrderId,
                    PatientId = x.PatientId,
                    OrganisationId = x.ReferenceId,
                    UserTypeID = x.UserTypeID,
                    AddressId = x.AddressId,
                    Date = x.Date,
                    IsActive = x.IsActive,
                    Title = x.Title,
                    Description = x.Description,
                    TotalPrice = x.TotalPrice,
                    CouponDiscount = x.CouponDiscount,
                    OtherDiscount = x.OtherDiscount,
                    NetPrice = x.NetPrice,
                    OrderStatus = x.OrderStatus,
                    PrescriptionImage = x.PrescriptionImage,
                    InsurancePlanId = x.InsurancePlanId,
                    InsuranceTypeId = x.InsurancePlanId > 0 ? GetInsuranceTypeId((int)x.InsurancePlanId) : 0,
                    OrganizationTypeID = x.OrganizationTypeID
                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid OrderId" });

            var patientInfo = getPatinetNameById(orgInfo.PatientId).FirstOrDefault();

            orgInfo.FullName = patientInfo is null ? string.Empty : patientInfo.FirstName + " " + patientInfo.LastName; orgInfo.PhoneNumber = patientInfo is null ? string.Empty : patientInfo.PhoneNumber;
            orgInfo.Email = patientInfo is null ? string.Empty : patientInfo.Email;

            var itemsList = _repo.ExecWithStoreProcedure<OrgPatientOrderDetailsViewModel>("spPatientOrderDetails_GetByOrderID @OrderId",
                    new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = id }
                    ).Select(x => new OrgPatientOrderDetailsViewModel()
                    {
                        OrderDetailId = x.OrderDetailId,
                        DrugId = x.DrugId,
                        DrugName = _repo.ExecWithStoreProcedure<OrgGetDrugInfoViewModel>("spOrgGetDrugInfo_ById @DrugId", new SqlParameter("DrugId", System.Data.SqlDbType.Int) { Value = x.DrugId }).First().DrugName,
                        Description = x.Description,
                        UnitPrice = x.UnitPrice,
                        Quantity = x.Quantity,
                        TotalAmount = x.TotalAmount,
                        OrderId = x.OrderId
                    });

            orgInfo.OrderDetails = itemsList.ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Order = orgInfo });
        }

        public List<OrgPatientInfoViewModel> getPatinetNameById(int id)
        {

            var patientNameInfo = _repo.ExecWithStoreProcedure<OrgPatientInfoViewModel>("spPatientName_GetByID @PatientId",
                   new SqlParameter("PatientId", System.Data.SqlDbType.BigInt) { Value = id }
                   );

            return patientNameInfo.ToList();
        }

        public int GetInsuranceTypeId(int id)
        {
            int InsuranceTypeId = 0;

            var planInfo = _repo.ExecWithStoreProcedure<InsurancePlanDropDownModel>("spInsuranceInfo_GetByPlanID @InsurancePlanId",
                   new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = id }
                   );

            InsuranceTypeId = planInfo.First().InsuranceTypeId;

            return InsuranceTypeId;
        }
        #endregion

        #region Coupon
        [HttpGet, Route("api/Organization/{organizationId}/coupons")]
        public HttpResponseMessage GetCoupons(int organizationId, Pagination param)
        {
            var couponInfo = _repo.ExecWithStoreProcedure<CouponModel>("spCoupon_Get @Search, @OrganizationId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = organizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = param.OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.PageSize : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = string.IsNullOrEmpty(param.SortDirection) ? "Asc" : param.SortDirection }
                    ).ToList();

            couponInfo.ForEach(x => x.UpdatedDate = x.UpdatedDate?.UtcToUserTime());

            int TotalRecordCount = 0;

            if (couponInfo.Count > 0)
                TotalRecordCount = couponInfo[0].TotalRecordCount;


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = TotalRecordCount,
                TotalDisplayRecords = couponInfo.Count,
                Coupons = couponInfo
            });
        }

        [HttpPost, Route("api/Organization/{organizationId}/coupon/save")]
        public HttpResponseMessage SaveCoupon(CouponModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 4, Message = ModelState.Errors() });

                var couponData = _repo.ExecuteSQLQuery("spCoupon_Create @DrugCouponID, @OrganizationID, @CouponCode, @CouponDiscountType, @CouponDiscountAmount, @CouponStartDate, @CouponEndDate, @IsActive, @CreateBy",
                                new SqlParameter("DrugCouponID", System.Data.SqlDbType.Int) { Value = (object)model.DrugCouponID },
                                new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = (object)model.OrganizationID },
                                new SqlParameter("CouponCode", System.Data.SqlDbType.NVarChar, 100) { Value = (object)model.CouponCode },
                                new SqlParameter("CouponDiscountType", System.Data.SqlDbType.Int) { Value = (object)    model.CouponDiscountType },
                                new SqlParameter("CouponDiscountAmount", System.Data.SqlDbType.Money) { Value = (object)            model.CouponDiscountAmount },
                                new SqlParameter("CouponStartDate", System.Data.SqlDbType.Date) { Value = (object)model.CouponStartDate },
                                new SqlParameter("CouponEndDate", System.Data.SqlDbType.Date) { Value = (object)model.CouponEndDate },
                                new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive },
                                new SqlParameter("CreateBy", System.Data.SqlDbType.Bit) { Value = (object)UserId });

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Coupon save successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Coupon saving Error.." });
            }
        }
        #endregion
    }
}
