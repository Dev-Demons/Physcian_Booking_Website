using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Enumerable;
using Binke.Api.Utility;

namespace Binke.Api.Controllers
{
    public class BookingController : BaseApiController
    {
        private readonly IRepository _repo;
        public BookingController(IRepository repo)
        {
            _repo = repo;
        }

        #region Senior Care
        //-- Get Senior Care Booking List
        [HttpGet, Route("api/GetSeniorCareBookingList/{id}")]
        public HttpResponseMessage GetSeniorCareBookingList(int Id, Pagination param)
        {
            if (param == null)
            {
                param = new Pagination();
            }
            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spProviderBooking_Get_new @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = Id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1007 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.StartIndex : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.SortDirection != null ? param.SortDirection : "Asc" }
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
                    OrganisationName = x.OrganisationName,
                    UserTypeID = 5
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

        //-- Get Senior Care Booking Details
        [HttpGet, Route("api/GetSeniorCareBookingDetail/{id}")]
        public HttpResponseMessage GetSeniorCareBookingDetail(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Slot Id is required" });


            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spSeniorCareDocOrgBooking_GetById @SlotId, @OrganizationId",
                new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = 1007 }

                ).Select(x => new DoctorBookingUpdateModel
                {
                    SlotId = x.SlotId,
                    OrganisationId = x.OrganisationId ?? 0,
                    AddressId = x.AddressId,
                    BookedFor = x.BookedFor,
                    FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserTypeID = 5,
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
                    OrganisationName = x.OrganisationName

                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = "Invalid SeniorCareBookingId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Booking = orgInfo });
        }

        //--Create/Edit Senior Care Booking
        [HttpPost, Route("api/SeniorCareBooking/save")]
        public HttpResponseMessage SaveSeniorCareBooking(DoctorBookingUpdateModel model)
        {
            try
            {
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
                                      new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
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
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 }
                                  );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = "Senior Care booking info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Senior Care Info Not Found! Should be select Senior Care Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Senior Care booking info saving Error.." });
            }

        }

        //-- Delete Senior Care Booking
        [HttpDelete, Route("api/SeniorCareBooking/{id}/remove")]
        public HttpResponseMessage DeleteSeniorCareBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Senior Care Booking info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Senior Care Booking info deleted Error.." });
            }
        }
        #endregion

        #region Pharmacy
        //-- Get Pharmacy Booking List
        [HttpGet, Route("api/GetPharmacyBookingList/{id}")]
        public HttpResponseMessage GetPharmacyBookingList(int Id, Pagination param)
        {
            if(param == null)
            {
                param = new Pagination();
            }
            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spProviderBooking_Get_new @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Pharmacy },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = Id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = OrganisationTypes.Pharmacy },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.StartIndex : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.SortDirection != null ? param.SortDirection : "Asc" }
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
                    OrganisationName = x.OrganisationName,
                    UserTypeID = 3
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

        //-- Get Pharmacy Booking Details
        [HttpGet, Route("api/GetPharmacyBookingDetail/{id}")]
        public HttpResponseMessage GetPharmacyBookingDetail(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Slot Id is required" });


            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spDocOrgBooking_GetById  @SlotId, @OrganizationId",
                new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = 1005 }

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
                    UserTypeID = 3,//pharmacy
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
                    OrganisationName = x.OrganisationName

                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = "Invalid PharmacyBookingId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Booking = orgInfo });
        }

        //--Create/Edit PharmacyBooking
        [HttpPost, Route("api/SavePharmacyBooking/save")]
        public HttpResponseMessage SavePharmacyBooking(DoctorBookingUpdateModel model)
        {
            try
            {
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
                                      new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
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
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }
                                  );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = "Pharmacy booking info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Pharmacy Info Not Found! Should be select Pharmacy Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Pharmacy booking info saving Error.." });
            }

        }

        //-- Delete Pharmacy Booking
        [HttpDelete, Route("api/DeletePharmacyBooking/{id}/remove")]
        public HttpResponseMessage DeletePharmacyBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Pharmacy Booking info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Pharmacy Booking info deleted Error.." });
            }
        }
        #endregion

        #region Facility
        //-- Get Facility Booking List
        [HttpGet, Route("api/GetFacilityBookingList/{id}")]
        public HttpResponseMessage GetFacilityBookingList(int Id, Pagination param)
        {
            if (param == null)
            {
                param = new Pagination();
            }
            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spProviderBooking_Get_new @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.Search == null ? " " : param.Search },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Facility },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = Id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = OrganisationTypes.Facility },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.StartIndex > 0 ? ((param.StartIndex / param.PageSize)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.PageSize > 0 ? param.StartIndex : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.SortDirection != null ? param.SortDirection : "Asc" }
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
                    OrganisationName = x.OrganisationName,
                    UserTypeID = 4
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

        //-- Get Facility Booking Details
        [HttpGet, Route("api/GetFacilityBookingDetail/{id}")]
        public HttpResponseMessage GetFacilityBookingDetail(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Slot Id is required" });


            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingModel>("spDocOrgBooking_GetById  @SlotId, @OrganizationId",
                new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = OrganisationTypes.Facility }

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
                    UserTypeID = UserTypes.Facility,
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
                    OrganisationName = x.OrganisationName

                }).FirstOrDefault();

            if (orgInfo is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = "Invalid FacilityBookingId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Booking = orgInfo });
        }

        //--Create/Edit Facility Booking
        [HttpPost, Route("api/SaveFacilityBooking/save")]
        public HttpResponseMessage SaveFacilityBooking(DoctorBookingUpdateModel model)
        {
            try
            {
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
                                      new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
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
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Facility }
                                  );

                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = "Facility booking info save successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Error..! Facility Info Not Found! Should be select Facility Name." });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.NotFound, Message = "Facility booking info saving Error.." });
            }

        }

        //-- Delete Facility Booking
        [HttpDelete, Route("api/DeleteFacilityBooking/{id}/remove")]
        public HttpResponseMessage DeleteFacilityBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = UserId }
                    );

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Pharmacy Booking info has been deleted successfully" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = $@"Pharmacy Booking info deleted Error.." });
            }
        }
        #endregion
    }
}
