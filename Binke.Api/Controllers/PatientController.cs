using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Enumerable;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Binke.Api.Utility;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Data.SqlClient;


namespace Binke.Api.Controllers
{
    public class PatientController : BaseApiController
    {
        private readonly IPatientService _patient;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository _repo;
        public PatientController(IPatientService patient,IRepository repo)
        {
            _patient = patient;
            _repo = repo;
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        #region Patients
        [HttpGet, Route("api/patients")]
        public HttpResponseMessage GetPatientList(Pagination param)
        {
            var allPatients = _patient.GetAll(x => !x.IsDeleted).Select(x => new PatientBasicInformation()
            {
                Id = x.PatientId,
                PatientId = x.PatientId,
                FullName = x.PatientUser.FullName,
                Email = x.PatientUser.UserName,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.Search))
            {
                allPatients = allPatients.Where(x => x.FullName.ToString().ToLower().Contains(param.Search.ToLower())
                                                || x.Email.ToString().ToLower().Contains(param.Search.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.Search.ToLower())
                                                || x.UpdatedDate.ToString().ToLower().Contains(param.Search.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(param.SortColumnName);
            var sortDirection = param.SortDirection; // asc or desc

            Func<PatientBasicInformation, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.FullName
                    : sortColumnIndex == 2 ? c.Email
                        : sortColumnIndex == 4 ? c.CreatedDate
                            : sortColumnIndex == 5 ? c.UpdatedDate
                                : c.FullName;
            allPatients = sortDirection == "asc" ? allPatients.OrderBy(orderingFunction).ToList() : allPatients.OrderByDescending(orderingFunction).ToList();

            var display = allPatients.Skip(param.StartIndex).Take(param.PageSize);
            var total = allPatients.Count;

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = 1,
                TotalRecords = total,
                TotalDisplayRecords = total,
                Patients = display
            });
        }

        [HttpGet, Route("api/patient/{id?}")]
        public HttpResponseMessage GetPatient(int id = 0)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "PatientId is required" });

            var patient = _patient.GetById(id);

            if (patient is null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid PatientId" });

            var result = new PatientBasicInformation();
            result.PatientId = patient.PatientId;
            result.FirstName = patient.PatientUser?.FirstName ?? "";
            result.MiddleName = patient.PatientUser?.MiddleName ?? "";
            result.LastName = patient.PatientUser?.LastName ?? "";
            result.Gender = patient.PatientUser?.Gender ?? "";
            result.DateOfBirth = patient.PatientUser?.DateOfBirth?.ToString("dd-MM-yyyy");
            result.PhoneNumber = patient.PatientUser?.PhoneNumber;
            result.FaxNumber = patient.PatientUser?.FaxNumber;
            result.FullName = patient.PatientUser?.FullName ?? "";
            result.Email = patient.PatientUser?.Email ?? "";
            result.IsActive = patient.IsActive;
            result.IsDeleted = patient.IsDeleted;

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Patient = result });
        }

        [HttpPost, Route("api/patient/save")]
        public HttpResponseMessage SavePatient(PatientBasicInformation model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = ModelState.Errors() });

                if (model.PatientId == 0)
                {
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Please enter password" });
                    }

                    if (string.IsNullOrEmpty(model.ConfirmPassword))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new
                        {
                            Status = 0,
                            Message = "Please enter confirm password"
                        });
                    }

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
                        //Uniquekey = model.Uniquekey,
                        UserTypeId = 12,
                        RegisterViewModel = JsonConvert.SerializeObject(model)
                    };

                    var isExists = _userManager.FindByEmail(model.Email);

                    if (isExists != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Patient Email already exists" });
                    }

                    var result = _userManager.Create(user, model.Password);

                    if (!result.Succeeded)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Not able to create Patient, try after sometime" });
                    }

                    _userManager.AddToRole(user.Id, "Patient");

                    var patient = new Patient
                    {
                        UserId = user.Id,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy =UserId,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _patient.InsertData(patient);
                    _patient.SaveData();
                    
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Patient save successfully" });
                }
                else
                {
                    var patient = _patient.GetById(model.PatientId);

                    var exists = _userManager.Users.Any(x => x.Id != patient.UserId && x.Email == model.Email);

                    if (exists)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Patient Email already exists" });
                    }

                    patient.PatientUser.FirstName = model.FirstName ?? "";
                    patient.PatientUser.MiddleName = model.MiddleName ?? "";
                    patient.PatientUser.LastName = model.LastName ?? "";
                    patient.PatientUser.Gender = model.Gender ?? "";

                    if (!string.IsNullOrEmpty(model.DateOfBirth))
                        patient.PatientUser.DateOfBirth = Convert.ToDateTime(model.DateOfBirth);

                    patient.PatientUser.PhoneNumber = model.PhoneNumber;
                    patient.PatientUser.FaxNumber = model.FaxNumber;
                    _patient.UpdateData(patient);
                    _patient.SaveData();
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Patient save successfully" });
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong." });
            }
        }

        [HttpDelete,Route("api/patient/{id}/remove")]
        public HttpResponseMessage RemovePatient(int id)
        {
            try
            {
                if (id == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "PatientId is required" });

                var patient = _patient.GetById(id);

                if (patient is null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = 0, Message = "Invalid PatientId" });

                _patient.DeleteData(patient);
                _patient.SaveData();

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = $@"Patient has been deleted successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Something is wrong." });
            }
        }

        #endregion

        #region Booking
        [HttpPost, Route("api/patient/{patientId}/Booking/save")]
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
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeId }

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

        [HttpDelete, Route("api/patient/{patientId}/Booking/{id}/remove")]
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

        //---- Get Patient Booking Details
        [HttpGet, Route("api/patient/Booking/{id}")]
        public HttpResponseMessage PatientBooking(int id) //Added by Reena
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
                    UserTypeID = 6,//Patient
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
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = "Invalid PatientBookingId" });

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Booking = orgInfo });
        }
        #endregion
    }
}
