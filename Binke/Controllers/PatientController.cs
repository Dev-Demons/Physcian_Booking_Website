using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Binke.Models;
using Binke.Utility;
using Binke.ViewModels;
using Doctyme.Model;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Microsoft.AspNet.Identity;

namespace Binke.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IAddressService _address;
        private readonly IPatientService _patient;
        private readonly IStateService _state;
        private readonly IUserService _appUser;
        private ApplicationUserManager _userManager;
        private readonly IRepository _repo;
        private readonly IUserTypeService _usertype;//Added by Reena
        private readonly IDoctorService _doctor;
        private readonly ICityStateZipService _cityStateZipService;

        private int _patientId;

        public PatientController(IAddressService address, IPatientService patient, IStateService state, IUserService appUser, ApplicationUserManager applicationUserManager, IRepository repo, IUserTypeService userType, IDoctorService doctor,
            ICityStateZipService cityStateZipService)
        {
            _address = address;
            _patient = patient;
            _state = state;
            _appUser = appUser;
            _userManager = applicationUserManager;
            _repo = repo;//Added by Reena
            _usertype = userType;//Added by Reena
            _doctor = doctor;
            _cityStateZipService = cityStateZipService;
        }

        #region Admin Section
        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }

        [Route("GetPatientList/{flag}")]
        public ActionResult GetPatientList(bool flag, JQueryDataTableParamModel param)
        {
            var allPatients = _appUser.GetAll(x => x.IsActive == flag && !x.IsDeleted && x.UserType.UserTypeName == "Patient").Select(x => new PatientBasicInformation()
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allPatients = allPatients.Where(x => x.FullName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.Email.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<PatientBasicInformation, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.FullName
                    : sortColumnIndex == 2 ? c.Email
                        : sortColumnIndex == 4 ? c.CreatedDate
                                : c.FullName;
            allPatients = sortDirection == "asc" ? allPatients.OrderBy(orderingFunction).ToList() : allPatients.OrderByDescending(orderingFunction).ToList();

            var display = allPatients.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allPatients.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("PatientProfile/{id?}")]
        public ActionResult PatientProfile(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Patient");
            }
            var patient = _patient.GetSingle(x => x.PatientId == id);
            return View(patient);
        }

        [Route("Patient/Booking")]
        public ActionResult Booking()
        {
            return View();
        }

        [Route("Patient/Orders")]
        public ActionResult Orders()
        {
            return View();
        }

        [Route("GetPatientOrder/{id?}")]
        public ActionResult GetPatientOrder(JQueryDataTableParamModel param, int id = 0)
        {
            try
            {
                var orders1 = _patient.ExecWithStoreProcedure<PatientOrders>("spPatientOrdersByPatientId @Search, @PatientId, @PageIndex, @PageSize, @Sort",
                           new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                            new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = id },
                            new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                            new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                            new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                            );

                var orders = orders1.ToList();

                int TotalRecordCount = 0;

                if (orders.Count > 0)
                    TotalRecordCount = orders[0].TotalRecordCount;


                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = orders
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("Patient/Profile/{userId}")]
        public ActionResult PatientUserProfile(int userId)
        {
            Session["PatientID"] = userId;
            var users = _patient.SQLQuery<DrpUser>("select * from AspNetUsers where Id = " + userId.ToString()).ToList();
            var user = new DrpUser();
            if (users.Count > 0)
                user = users.First();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPatientUser(DrpUser user)
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
                    if (!rgx.IsMatch(user.Password))
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

                _patient.ExecuteSqlCommandForUpdate("AspNetUsers", "Id", user.Id, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Patient user updated successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating facility user" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("GetPatientBooking/{id?}")]
        public ActionResult GetPatientBooking(JQueryDataTableParamModel param, int id = 0)
        {
            try
            {
                var bookings = _patient.ExecWithStoreProcedure<OrgBookingListViewModel>("spPatientBookingByPatientId @Search, @PatientId, @PageIndex, @PageSize, @Sort", //Added by Reena
                            new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                            new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = id },
                            new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                            new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                            new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                            ).ToList();

                int TotalRecordCount = 0;

                if (bookings.Count > 0)
                    TotalRecordCount = bookings[0].TotalRecordCount;


                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = bookings
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("Patient/Profile/{id?}/{isEdit}")]
        public ActionResult Profile(int id = 0, bool isEdit = false)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Patient");
            }
            //var patients = _patient.ExecWithStoreProcedure<PatientProfile>("spPatientProfile_Get @PatientId",
            //             new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = id }).ToList();
            //var patients = _patient.ExecWithStoreProcedure<PatientProfile>("spBasicPatientProfile_Get @PatientId",
            //             new SqlParameter("@PatientId", System.Data.SqlDbType.Int) { Value = id }).ToList();//Added by Reena
            var patients = GetPatientDetails(id);

            Session["PatientID"] = id;
            ViewBag.PatientID = id;
            ViewBag.IsEdit = isEdit;
            ViewBag.InsuranceType = InsuranceTypeList();
            if (patients != null)
                return View(patients);
            else
                return View(new PatientProfile());
        }

        private PatientProfile GetPatientDetails(int userId)
        {
            DataSet ds = _doctor.GetQueryResult("spBasicPatientProfile_Get " + userId);
            DataColumn dcUserId = ds.Tables[0].Columns["UserId"];
            DataColumn dcFirstName = ds.Tables[0].Columns["FirstName"];
            DataColumn dcLastName = ds.Tables[0].Columns["LastName"];
            DataColumn dcMiddleName = ds.Tables[0].Columns["MiddleName"];
            DataColumn dcAddress = ds.Tables[0].Columns["Address"];
            DataColumn dcCity = ds.Tables[0].Columns["City"];
            DataColumn dcState = ds.Tables[0].Columns["State"];
            DataColumn dcZipCode = ds.Tables[0].Columns["ZipCode"];
            DataColumn dcDateOfBirth = ds.Tables[0].Columns["DateOfBirth"];
            DataColumn dcPhoneNumber = ds.Tables[0].Columns["PhoneNumber"];
            DataColumn dcEmail = ds.Tables[0].Columns["Email"];
            DataColumn dcProfilePicture = ds.Tables[0].Columns["ProfilePicture"];
            DataColumn dcPrimaryDoctor = ds.Tables[0].Columns["PrimaryDoctor"];
            DataColumn dcHealthInsurance = ds.Tables[0].Columns["HealthInsurance"];
            DataColumn dcPatientId = ds.Tables[0].Columns["PatientId"];
            DataColumn dcInsurancePlanID = ds.Tables[0].Columns["InsurancePlanID"];
            DataColumn dcInsuranceTypeId = ds.Tables[0].Columns["InsuranceTypeId"];
            DataColumn dcCityStateZipCodeID = ds.Tables[0].Columns["CityStateZipCodeID"];
            DataColumn dcAddressId = ds.Tables[0].Columns["AddressId"];
            DataColumn dcDoctorId = ds.Tables[0].Columns["DoctorId"];

            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => new PatientProfile()
            {
                UserId = dr.Field<int>(dcUserId),
                FirstName = dr.Field<string>(dcFirstName),
                LastName = dr.Field<string>(dcLastName),
                MiddleName = dr.Field<string>(dcMiddleName),
                Address = dr.Field<string>(dcAddress),
                City = dr.Field<string>(dcCity),
                State = dr.Field<string>(dcState),
                ZipCode = dr.Field<string>(dcZipCode),
                DateOfBirth = dr.Field<DateTime?>(dcDateOfBirth),
                PhoneNumber = dr.Field<string>(dcPhoneNumber),
                Email = dr.Field<string>(dcEmail),
                ProfilePicture = dr.Field<string>(dcProfilePicture),
                PrimaryDoctor = dr.Field<string>(dcPrimaryDoctor),
                HealthInsurance = dr.Field<string>(dcHealthInsurance),
                PatientId = dr.Field<int?>(dcPatientId),
                InsurancePlanID = dr.Field<int?>(dcInsurancePlanID),
                InsuranceTypeId = dr.Field<int?>(dcInsuranceTypeId),
                CityStateZipCodeID = dr.Field<int?>(dcCityStateZipCodeID),
                AddressId = dr.Field<int?>(dcAddressId),
                DoctorId = dr.Field<int?>(dcDoctorId)
            }).FirstOrDefault();
            return list;
        }

        private IEnumerable<SelectListItem> InsuranceTypeList()
        {
            DataSet ds = _doctor.GetQueryResult("spInsuranceTypeDropDownList_Get");
            DataColumn dcId = ds.Tables[0].Columns["InsuranceTypeId"];
            DataColumn dcName = ds.Tables[0].Columns["Name"];
            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => new SelectListItem { Text = dr.Field<string>(dcName), Value = dr.Field<int>(dcId).ToString() }).ToList();
            return list;
        }

        [HttpGet, Route("Patient/Profile/{id?}/InsurancePlanList/{typeId}")]
        public ActionResult InsurancePlanList(string typeId)
        {
            IEnumerable<string> list = null;
            try
            {
                list = GetInsurancePlan(typeId);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetZipCodeSearch/{typeId}");
                return Json(new
                {
                    lstZipCode = list
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private IEnumerable<string> GetInsurancePlan(string typeId)
        {
            DataSet ds = _doctor.GetQueryResult("spInsurancePlanDropDownListByType_Get" + " " + typeId);
            DataColumn dcId = ds.Tables[0].Columns["InsurancePlanId"];
            DataColumn dcName = ds.Tables[0].Columns["Name"];
            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => dr.Field<string>(dcName) + "," + dr.Field<int>(dcId).ToString()).ToList();
            return list;
        }

        [HttpGet, Route("Patient/Profile/{id?}/GetZipCityStateList/{cityZipCode}")]
        public ActionResult GetZipCityStateList(string cityZipCode)
        {
            List<string> result = new List<string>();
            try
            {
                result = GetZipCityState(cityZipCode);
                return Json(result
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetZipCodeSearch/{zipCode}");
                return Json(new
                {
                    lstZipCode = result
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private List<string> GetZipCityState(string cityZipCode)
        {
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.spGetCityStateZipByZipcode + " " + cityZipCode);
            DataColumn dc = ds.Tables[0].Columns["ZipCityState"];
            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => dr.Field<string>(dc)).ToList();
            return list;
        }

        [Route("ActiveDeActivePatient/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActivePatient(bool flag, int id)
        {
            var patient = _patient.GetById(id);
            patient.IsActive = !flag;
            patient.PatientUser.IsActive = !flag;
            _patient.UpdateData(patient);
            _patient.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The patient has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region Patient Profile
        [HttpGet, Route("PatientBasicInformation/{id?}")]
        public ActionResult BasicInformation(int id = 0)
        {
            ViewBag.StateList = _state.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.StateName,
                Value = x.StateId.ToString()
            }).ToList();
            if (User.IsInRole(UserRoles.Admin))
                _patientId = id;
            else
                _patientId = GetPatientId();
            var patient = _patient.GetById(_patientId);
            var result = AutoMapper.Mapper.Map<PatientBasicInformation>(patient);
            result.FirstName = patient.PatientUser?.FirstName ?? "";
            result.MiddleName = patient.PatientUser?.MiddleName ?? "";
            result.LastName = patient.PatientUser?.LastName ?? "";
            result.Gender = patient.PatientUser?.Gender ?? "";
            result.DateOfBirth = patient.PatientUser?.DateOfBirth?.ToString("dd-MM-yyyy");
            result.PhoneNumber = patient.PatientUser?.PhoneNumber;
            result.FaxNumber = patient.PatientUser?.FaxNumber;

            //result.PrimaryInsurance = patient.PrimaryInsurance;
            // result.SecondaryInsurance = patient.SecondaryInsurance;

            result.AddressView = AutoMapper.Mapper.Map<AddressViewModel>(patient.Address.FirstOrDefault(x => x.IsActive && !x.IsDeleted));


            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPatient(string dateOfBirth, PatientProfile user)
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
                    if (!rgx.IsMatch(user.Password))
                        return Json(new JsonResponse { Status = 0, Message = "Passwords must contain at least one lowercase letter, one uppercase letter, and one number." }, JsonRequestBehavior.AllowGet);

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user.UserId);
                    var result = await _userManager.ResetPasswordAsync(user.UserId, code, user.Password);
                    if (!result.Succeeded)
                        return Json(new JsonResponse { Status = 0, Message = "Error in updating password.. may be it will not meet password requirements." }, JsonRequestBehavior.AllowGet);
                }


                //if (Image1 != null && Image1.ContentLength > 0)
                //{
                //    DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/PatientSiteImages/"));
                //    if (!dir.Exists)
                //    {
                //        Directory.CreateDirectory(Server.MapPath("~/Uploads/PatientSiteImages"));
                //    }

                //    string extension = Path.GetExtension(user.ProfilePicture);
                //    string newImageName = "Patient-" + DateTime.Now.Ticks.ToString() + extension;

                //    var path = Path.Combine(Server.MapPath("~/Uploads/PatientSiteImages"), newImageName);
                //    Image1.SaveAs(path);

                //    string imagePath = newImageName;
                //    user.ProfilePicture = imagePath;
                //}
                if (!string.IsNullOrWhiteSpace(user.ProfilePicture) && !user.ProfilePicture.Contains("Patient"))
                {
                    string extension = Path.GetExtension(user.ProfilePicture);
                    string newImageName = "Patient-" + DateTime.Now.Date.Ticks.ToString() + extension;
                    string imagePath = newImageName;
                    user.ProfilePicture = imagePath;
                }
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("FirstName", SqlDbType.VarChar) { Value = user.FirstName });
                //parameters.Add(new SqlParameter("MiddleName", SqlDbType.VarChar) { Value = user.MiddleName });
                parameters.Add(new SqlParameter("LastName", SqlDbType.VarChar) { Value = user.LastName });
                parameters.Add(new SqlParameter("PhoneNumber", SqlDbType.NVarChar) { Value = user.PhoneNumber });
                parameters.Add(new SqlParameter("DateOfBirth", SqlDbType.DateTime)
                {
                    Value =
                    DateTime.ParseExact(!string.IsNullOrEmpty(dateOfBirth) ? dateOfBirth : DateTime.Now.ToString(),
                    new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd HH:mm:ss.fff")
                });
                parameters.Add(new SqlParameter("ProfilePicture", SqlDbType.NVarChar) { Value = user.ProfilePicture });
                parameters.Add(new SqlParameter("Email", SqlDbType.NVarChar) { Value = user.Email });

                _patient.ExecuteSqlCommandForUpdate("AspNetUsers", "Id", user.UserId, parameters);

                using (TransactionScope scope = new TransactionScope())
                {
                    Patient patient = _repo.Find<Patient>(w => w.PatientId == user.PatientId);
                    if (patient == null)
                        patient = new Patient();
                    patient.UserId = user.UserId != 0 ? user.UserId : User.Identity.GetUserId<int>();
                    patient.PatientId = user.PatientId.HasValue ? user.PatientId.Value : 0;
                    patient.InsurancePlanID = user.InsurancePlanID;
                    patient.IsActive = true;
                    patient.IsDeleted = false;
                    patient.IsPrimary = true;
                    patient.IsTeleMedCovered = true;
                    patient.ReferenceId = user.DoctorId.HasValue ? user.DoctorId.Value : new Nullable<int>();

                    int status = 1;
                    if (patient.PatientId > 0)
                    {
                        patient.ModifiedBy = User.Identity.GetUserId<int>();
                        patient.UpdatedDate = DateTime.UtcNow;
                        status = _repo.Update<Patient>(patient, true);
                    }
                    else
                    {
                        patient.CreatedBy = User.Identity.GetUserId<int>();
                        patient.CreatedDate = DateTime.UtcNow;
                        patient = _repo.Insert<Patient>(patient, true);
                    }

                    Address address = _repo.Find<Address>(w => w.AddressId == user.AddressId);
                    if (address == null)
                        address = new Address();
                    address.Address1 = user.Address;
                    address.AddressId = user.AddressId.HasValue ? user.AddressId.Value : 0;
                    address.AddressTypeID = 11;
                    address.ReferenceId = user.UserId;
                    address.CityStateZipCodeID = !string.IsNullOrEmpty(user.City) ? GetZipCityStateID(user.ZipCode, user.City) : 0;
                    address.UserTypeID = 12;
                    address.IsDeleted = false;
                    address.IsActive = true;
                    address.Phone = user.PhoneNumber;
                    address.Email = user.Email;

                    if (address.AddressId > 0)
                    {
                        address.ModifiedBy = User.Identity.GetUserId<int>();
                        address.UpdatedDate = DateTime.UtcNow;
                        status = _repo.Update<Address>(address, true);
                    }
                    else
                    {
                        address.CreatedBy = User.Identity.GetUserId<int>();
                        address.CreatedDate = DateTime.UtcNow;
                        address = _repo.Insert<Address>(address, true);
                    }

                    if (status == 1)
                        scope.Complete();
                }
                return Json(new JsonResponse { Status = 1, Message = "Patient updated successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating patient" }, JsonRequestBehavior.AllowGet);
            }
        }

        private int GetZipCityStateID(string cityZipCode, string city)
        {
            DataSet ds = _doctor.GetQueryResult("spGetCityStateCityByZipCodeAutoComplete " + " " + cityZipCode + ", '" + city + "'");
            //var result = ds.Tables[0].Rows.Cast<DataRow>().ToArray();
            DataColumn dc = ds.Tables[0].Columns["CityStateZipCodeID"];

            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => dr.Field<int>(dc)).FirstOrDefault();
            return list;
        }

        [HttpGet, Route("Patient/Profile/{id?}/GetDoctorList/{doctorName}")]
        public ActionResult GetDoctorList(string doctorName)
        {
            List<string> result = new List<string>();
            try
            {
                result = GetDoctorListAutoCompleteByDoctorName(doctorName);
                return Json(result
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetZipCodeSearch/{doctorName}");
                return Json(new
                {
                    lstZipCode = result
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<string> GetDoctorListAutoCompleteByDoctorName(string DoctorName)
        {
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.spGetDoctorListAutoCompleteByDoctorName + " " + DoctorName);
            //var result = ds.Tables[0].Rows.Cast<DataRow>().ToArray();
            DataColumn dcName = ds.Tables[0].Columns["PrimaryDoctor"];

            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => dr.Field<string>(dcName)).ToList();
            return list;
        }

        [Route("PatientBasicInformation")]
        [HttpPost]
        public JsonResult BasicInformation(PatientBasicInformation model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var patient = _patient.GetById(model.PatientId);

                    #region Patient Basic
                    patient.PatientUser.FirstName = model.FirstName ?? "";
                    patient.PatientUser.MiddleName = model.MiddleName ?? "";
                    patient.PatientUser.LastName = model.LastName ?? "";
                    patient.PatientUser.Gender = model.Gender ?? "";
                    patient.PatientUser.DateOfBirth = Convert.ToDateTime(model.DateOfBirth);
                    patient.PatientUser.PhoneNumber = model.PhoneNumber;
                    patient.PatientUser.FaxNumber = model.FaxNumber;
                    // patient.InsurancePlan. = model.PrimaryInsurance;
                    // patient.SecondaryInsurance = model.SecondaryInsurance;
                    _patient.UpdateData(patient);
                    _patient.SaveData();
                    #endregion

                    #region Patient Address

                    var address = AutoMapper.Mapper.Map<AddressViewModel, Address>(model.AddressView);
                    address.AddressId = model.AddressView.AddressId;
                    if (address.AddressId == 0)
                    {
                        //  address.PatientId = model.PatientId;
                        address.IsActive = true;
                        _address.InsertData(address);
                        _address.SaveData();
                    }
                    else
                    {
                        var editAddress = _address.GetById(address.AddressId);
                        editAddress.Address1 = address.Address1;
                        editAddress.Address2 = address.Address2;
                        editAddress.CityId = address.CityId;
                        editAddress.StateId = address.StateId;
                        editAddress.Country = address.Country;
                        editAddress.ZipCode = address.ZipCode;
                        _address.UpdateData(editAddress);
                        _address.SaveData();
                    }
                    #endregion

                    txscope.Complete();
                    return Json(new JsonResponse { Status = 1, Message = "Patient basic information updated." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "BasicInformation-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult Address(int? id)//Added by Reena
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PatientData"] = "Yes";
                ViewBag.PatientID = id;
                Session["PatientID"] = id;
            }
            else
            {
                if (User.IsInRole("Patient"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    var patientInfo = _repo.Find<ApplicationUser>(x => x.IsDeleted == false && x.Id == userId && x.IsActive == true);
                    if (patientInfo != null)
                    {
                        TempData["PatientData"] = "Yes";
                        ViewBag.PatientID = patientInfo.Id;
                        Session["PatientID"] = patientInfo.Id;
                        Session["PatientName"] = patientInfo.FullName;
                    }
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [HttpPost, Route("AddEditPatientAddress"), ValidateAntiForgeryToken]
        public JsonResult AddEditPatientAddress(OrganisationAddressViewModel model)//Added by Reena
        {
            try
            {
                var datauserType = GetUserTypes("Patient");
                int userTypeId = 6;
                if (datauserType != null)
                {
                    userTypeId = datauserType.UserTypeId;
                }
                model.OrganisationId = User.Identity.GetUserId<int>();
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
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                      new SqlParameter("Phone", System.Data.SqlDbType.NVarChar) { Value = (object)model.Phone ?? DBNull.Value },
                                      new SqlParameter("Fax", System.Data.SqlDbType.NVarChar) { Value = (object)model.Fax ?? DBNull.Value },
                                      new SqlParameter("Email", System.Data.SqlDbType.NVarChar) { Value = (object)model.Email ?? DBNull.Value },
                                      new SqlParameter("WebSite", System.Data.SqlDbType.VarChar) { Value = (object)model.WebSite ?? DBNull.Value },
                                      new SqlParameter("Lat", System.Data.SqlDbType.Decimal) { Value = (object)model.Lat ?? DBNull.Value },
                                      new SqlParameter("Lon", System.Data.SqlDbType.Decimal) { Value = (object)model.Lon ?? DBNull.Value },
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = userTypeId },
                                      new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = model.AddressId > 0 ? User.Identity.GetUserId<int>() : (object)DBNull.Value }
                                  );

                return Json(new JsonResponse() { Status = 1, Message = "Patient address info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Patient address info saving Error.." });
            }

        }

        [Route("DeletePatientAddress/{id}")]
        [HttpPost]
        public JsonResult DeletePatientAddress(int id)//Added by Reena
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_Delete @AddressId, @ModifiedBy",
                    new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Patient Address has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Patient Address deleted Error.." });
            }


        }

        [HttpPost, Route("AddEditPatientOrder"), ValidateAntiForgeryToken]
        public JsonResult AddEditPatientOrder(OrgPatientOrderUpdateViewModel model, HttpPostedFileBase Image1)//Added by Reena
        {
            try
            {
                if (model.PatientId == 0)
                {
                    var userId = User.Identity.GetUserId<int>();
                    model.PatientId = userId;
                }
                if (model.PatientId > 0)
                {
                    string imagePath = null;

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/PharmacyPrescription/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/PharmacyPrescription"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "Prescription-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/PharmacyPrescription"), newImageName);
                        Image1.SaveAs(path);

                        imagePath = newImageName;
                    }

                    if (model.OrderId > 0 && imagePath == null)
                    {
                        imagePath = model.PrescriptionImage;
                    }
                    var datauserType = GetUserTypes("Patient");
                    int userTypeId = 6;
                    if (datauserType != null)
                    {
                        userTypeId = datauserType.UserTypeId;
                    }
                    if (model.Date == null)
                    {
                        model.Date = DateTime.Now.Date;
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
                                                  new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = userTypeId },
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
                                                  new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                                  new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive ?? DBNull.Value }
                                              ).FirstOrDefault();

                    long curOrderId = 0;
                    if (model.OrderId == 0)
                        curOrderId = orderData.OrderId;
                    else
                        curOrderId = model.OrderId;

                    if (curOrderId > 0)
                    {
                        string drugIds = model.DragIds;
                        string[] drugsArray = drugIds.Split(',');

                        for (int i = 0; i < drugsArray.Length; i++)
                        {
                            int DrugID = int.Parse(drugsArray[i]);
                            int DetailsId = int.Parse(Request["DetailId_" + DrugID.ToString()].ToString());
                            string strDescription = Request["Description_" + DrugID.ToString()];
                            decimal unitPrice = decimal.Parse(Request["UnitPrice_" + DrugID.ToString()].ToString());
                            int qty = int.Parse(Request["qty_" + DrugID.ToString()].ToString());
                            decimal totPrice = decimal.Parse(Request["tot_" + DrugID.ToString()].ToString());

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
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                              new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = 1 }

                                          );
                        }
                    }

                    return Json(new JsonResponse() { Status = 1, Message = "Patient Order info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Patient Info Not Found! Should be select Patient Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Patient Order info saving Error.." });
            }

        }

        //-- Delete Patient Order
        [Route("DeletePatientOrder/{id}")]
        [HttpPost]
        public JsonResult DeletePatientOrder(int id)//Added by Reena
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spPatientOrder_Delete @OrderId, @ModifiedBy",
                    new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Patient order info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Patient Order info deleted Error.." });
            }
        }

        public PartialViewResult PatientBooking(int? slotId, int? addressId)//Added by Reena
        {
            ViewBag.PatientID = Session["PatientID"];
            var addressList = new List<OrgAddressDropdownViewModel>();
            var param = new JQueryDataTableParamModel();
            var datauserType = GetUserTypes("Patient");
            int userTypeId = 6;
            if (datauserType != null)
            {
                userTypeId = datauserType.UserTypeId;
            }
            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get").ToList();
            ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name");
            var planList = new List<InsurancePlanDropDownViewModel>();
            ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name");

            if (slotId > 0)
            {
                ViewBag.ID = slotId;
                var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spGetDocOrgBooking_ById @SlotId",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = slotId }
                    ).Select(x => new OrgBookingUpdateViewModel
                    {
                        SlotId = x.SlotId,
                        OrganisationId = x.OrganisationId.HasValue ? x.OrganisationId.Value : 0,
                        DoctorId = x.DoctorId,
                        AddressId = x.AddressId,
                        BookedFor = x.BookedFor,
                        OrganizatonTypeID = x.OrganizatonTypeID.HasValue ? x.OrganizatonTypeID.Value : 0,
                        OrganisationName = x.OrganisationName,
                        FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        UserTypeID = 12,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        SlotDate = x.SlotDate,
                        SlotTime = x.SlotTime,
                        Description = x.Description,
                        IsBooked = x.IsBooked,
                        IsEmailReminder = x.IsEmailReminder,
                        IsTextReminder = x.IsTextReminder,
                        IsInsuranceChanged = x.IsInsuranceChanged,
                        InsurancePlanId = x.InsurancePlanId.HasValue ? x.InsurancePlanId.Value : 0,
                        InsuranceTypeId = x.InsuranceTypeId.HasValue ? x.InsuranceTypeId.Value : 0,
                        FullAddress = x.FullAddress,
                    }).FirstOrDefault();
                int? docorgId = orgInfo.DoctorId > 0 ? orgInfo.DoctorId : orgInfo.OrganisationId;

                //var addressesList = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModel>("sprGetDocOrganiationAddresses @OrganizationId, @UserTypeId",
                //new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = docorgId },
                //new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = orgInfo.UserTypeID }
                //).ToList();
                string spName = "EXEC GetTimeSlotsbyDocId " + orgInfo.DoctorId + " , 1" + " , '" + orgInfo.SlotDate + "'";
                if (orgInfo.DoctorId == 0)
                {
                    spName = "EXEC GetTimeSlotsbyOrgId " + orgInfo.OrganisationId + " , 1" + " , '" + orgInfo.SlotDate + "'";
                }
                DataSet ds = _doctor.GetQueryResult(spName);
                var lstslotTimes = Common.ConvertDataTable<Doctyme.Model.ViewModels.SlotTimes>(ds.Tables[1]).ToList();
                var list = lstslotTimes.ToList();
                if (list != null && list.Count() > 0)
                {
                    ViewBag.slotsList = new SelectList(list.Select(x => new
                    {
                        SlotTime = x.SlotSatrtTime
                    }).ToList(), "SlotTime", "SlotTime", orgInfo.SlotTime);
                }
                else
                {
                    ViewBag.slotsList = new SelectList(new List<OrgBookingUpdateViewModel>(), "SlotTime", "SlotTime", orgInfo.SlotTime);
                }
                orgInfo.AddressId = addressId.Value;
                var addressesList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddressInfoByID @AddressID",
           new SqlParameter("AddressID", System.Data.SqlDbType.Int) { Value = orgInfo.AddressId }
           ).Select(x => new OrgAddressDropdownViewModel()
           {
               AddressId = x.AddressId,
               FullAddress = x.Address1 + " " + x.Address2 + " " + GetCityStateInfoById(x.CityStateZipCodeID, "zip") + " " + GetCityStateInfoById(x.CityStateZipCodeID, "city") + " " + GetCityStateInfoById(x.CityStateZipCodeID, "state") + " " + GetCityStateInfoById(x.CityStateZipCodeID, "country")
           });
                ViewBag.addressList = new SelectList(addressesList.OrderBy(o => o.AddressId), "AddressId", "FullAddress", orgInfo.AddressId);
                planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get").ToList();
                ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name", orgInfo.InsurancePlanId);
                ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name", orgInfo.InsuranceTypeId);
                return PartialView(@"/Views/Patient/Partial/_PatientBooking.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgBookingUpdateViewModel();
                if(orgInfo!=null)
                {
                    orgInfo.UserTypeID = orgInfo.UserTypeID==null ? 0 : orgInfo.UserTypeID;
                }
                ViewBag.slotsList = new SelectList(new List<OrgBookingUpdateViewModel>(), "SlotTime", "SlotTime", orgInfo.SlotTime);
                ViewBag.addressList = new SelectList(new List<OrganizationAddressDropDownViewModel>(), "AddressId", "FullAddress", orgInfo.AddressId);
                return PartialView(@"/Views/Patient/Partial/_PatientBooking.cshtml", orgInfo);
            }
        }

        [HttpPost]
        public JsonResult GetPatientInsurancePlanByTypeId(int TypeId)//Added by Reena
        {
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spPatientInsurancePlanDropDownList_Get @InsuranceTypeId",
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
                );

            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult PatientOrder(int? id)//Added by Reena
        {
            ViewBag.PatientID = Session["PatientID"];
            var datauserType = GetUserTypes("Patient");
            int userTypeId = 6;
            if (datauserType != null)
            {
                userTypeId = datauserType.UserTypeId;
            }
            var addressList = new List<OrgAddressDropdownViewModel>();
            //var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
            //    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.PatientID },
            //    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = userTypeId }
            //    ).Select(x => new OrgAddressDropdownViewModel()
            //    {
            //        AddressId = x.AddressId,
            //        FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country
            //    }).ToList();

            ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "OrganizationAddress");

            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get");
            ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name");


            var planList = new List<InsurancePlanDropDownViewModel>();
            ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name");

            ViewBag.orderItems = null;

            if (id > 0)
            {
                ViewBag.ID = id;

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
                        //FullName = getPatinetNameById(x.PatientId).Count() > 0 ? (getPatinetNameById(x.PatientId).First().FirstName + " " + getPatinetNameById(x.PatientId).First().LastName) : "",
                        //PhoneNumber = getPatinetNameById(x.PatientId).Count() > 0 ? getPatinetNameById(x.PatientId).First().PhoneNumber : "",
                        //Email = getPatinetNameById(x.PatientId).Count() > 0 ? getPatinetNameById(x.PatientId).First().Email : "",
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
                    }).First();
                var addressesList = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModel>("sprGetDocOrganiationAddresses @OrganizationId, @UserTypeId",
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = orgInfo.PatientId },
                new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = userTypeId }
                ).ToList();

                ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "OrganizationAddress", orgInfo.AddressId);
                var planList1 = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get").ToList();
                ViewBag.planList = new SelectList(planList1.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name", orgInfo.InsurancePlanId);

                if (ItemList((long)id).Count() > 0)
                {
                    ViewBag.orderItems = ItemList((long)id);
                }
                else
                {
                    ViewBag.orderItems = null;
                }

                return PartialView(@"/Views/Patient/Partial/_PatientOrder.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgPatientOrderUpdateViewModel();
                return PartialView(@"/Views/Patient/Partial/_PatientOrder.cshtml", orgInfo);
            }
        }

        public List<OrgPatientOrderDetailsViewModel> ItemList(long id)//Added by Reena
        {
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
                        TotalAmount = x.TotalAmount
                    });

            return itemsList.ToList();
        }

        [Route("GetPatientOfficeLocationList/{flag}/{id}")]
        public ActionResult GetPatientOfficeLocationList(bool flag, JQueryDataTableParamModel param, int? id)//Added by Reena
        {
            var datauserType = GetUserTypes("Patient");
            int userTypeId = 6;
            if (datauserType != null)
            {
                userTypeId = datauserType.UserTypeId;
            }

            var allPharmacyAddress = _repo.ExecWithStoreProcedure<OrganisationAddressViewModel>("spAddress_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = userTypeId },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 0 },
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
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    ZipCode = x.ZipCode,
                    City = x.City,
                    State = x.State,
                    Country = x.Country,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
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

        public PartialViewResult PatientAddress(int? id)//Added by Reena
        {
            ViewBag.PatientID = Session["PatientID"];

            var addressTypes = _repo.ExecWithStoreProcedure<AddressTypeDropDownViewModel>("spAddressTypeDropDown");
            ViewBag.AddressTypes = new SelectList(addressTypes.OrderBy(o => o.AddressTypeId), "AddressTypeId", "Name");

            //var zipCodeList = _repo.ExecWithStoreProcedure<CityStateInfoByZipCodeViewModel>("spGetCityStateList_ByType @type",
            //    new SqlParameter("type", System.Data.SqlDbType.VarChar) { Value = "zip" });

            //var cityList = _repo.ExecWithStoreProcedure<CityStateInfoByZipCodeViewModel>("spGetCityStateList_ByType @type",
            //   new SqlParameter("type", System.Data.SqlDbType.VarChar) { Value = "city" });

            //var stateList = _repo.ExecWithStoreProcedure<CityStateInfoByZipCodeViewModel>("spGetCityStateList_ByType @type",
            //   new SqlParameter("type", System.Data.SqlDbType.VarChar) { Value = "state" });

            //var countryList = _repo.ExecWithStoreProcedure<CityStateInfoByZipCodeViewModel>("spGetCityStateList_ByType @type",
            //   new SqlParameter("type", System.Data.SqlDbType.VarChar) { Value = "country" });

            var lst1 = new List<CityStateInfoByZipCodeViewModel>();

            ViewBag.cityList = new SelectList(lst1, "City", "City");
            ViewBag.stateList = new SelectList(lst1, "State", "State");
            ViewBag.countryList = new SelectList(lst1, "Country", "Country");


            if (id > 0)
            {
                ViewBag.ID = id;

                var orgAddressInfo = _repo.ExecWithStoreProcedure<OrganisationAddress>("spGetAddressInfoByID @AddressID",
                    new SqlParameter("AddressID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrganisationAddressViewModel
                    {
                        AddressId = x.AddressId,
                        OrganisationId = x.ReferenceId,
                        ReferenceId = x.ReferenceId,
                        AddressTypeId = x.AddressTypeID,
                        //OrganisationName = GetOrganisationNameById(x.ReferenceId),
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
                    }).First();

                ViewBag.AddressTypes = new SelectList(addressTypes.OrderBy(o => o.AddressTypeId), "AddressTypeId", "Name", orgAddressInfo.AddressTypeId);
                return PartialView(@"/Views/Patient/Partial/_PatientAddress.cshtml", orgAddressInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganisationAddressViewModel();
                return PartialView(@"/Views/Patient/Partial/_PatientAddress.cshtml", orgAddressInfo);
            }
        }

        public string GetCityStateInfoById(int id, string type)//Added by Reena
        {
            string result = "";

            var info = _repo.SQLQuery<CityStateInfoByZipCodeViewModel>("spGetCityStateZipInfoByID @ID", new SqlParameter("ID", System.Data.SqlDbType.Int) { Value = id }).ToList();

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


        [HttpPost, Route("AddEditPatientBooking"), ValidateAntiForgeryToken]
        public JsonResult AddEditPatientBooking(OrgBookingUpdateViewModel model)
        {
            try
            {
                int userTypeId = 6;
                //if (model != null && (model.UserTypeID == null || model.UserTypeID < 0))
                //{
                //    var datauserType = GetUserTypes("Patient");
                //    userTypeId = datauserType.UserTypeId;
                //}
                userTypeId = model.UserTypeID ?? 0;
                model.BookedFor = User.Identity.GetUserId<int>();
                if (model.OrganisationId == 0)
                {
                    model.OrganisationId = User.Identity.GetUserId<int>();
                    model.BookedFor = User.Identity.GetUserId<int>();
                }
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
                                          new SqlParameter("IsBooked", System.Data.SqlDbType.Bit) { Value = true },
                                          new SqlParameter("IsEmailReminder", System.Data.SqlDbType.Bit) { Value = model.IsEmailReminder },
                                          new SqlParameter("IsTextReminder", System.Data.SqlDbType.Bit) { Value = model.IsTextReminder },
                                          new SqlParameter("IsInsuranceChanged", System.Data.SqlDbType.Bit) { Value = model.IsInsuranceChanged },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = true },
                                          new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = model.InsurancePlanId },
                                          new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId },
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = model.Description },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = userTypeId }

                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Patient booking info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Patient Info Not Found! Should be select Patient Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Patient booking info saving Error.." });
            }

        }

        //-- Delete Pharmacy Booking
        [Route("DeletePatientBooking/{id}")]
        [HttpPost]
        public JsonResult DeletePatientBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Patient Booking info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Patient Booking info deleted Error.." });
            }
        }

        [HttpPost]
        public JsonResult GetPharmacyList(string Prefix, int OrganizationTypeId, string ZipCode = "")
        {
            var pharmacyList = _repo.ExecWithStoreProcedure<OrganisationAddressViewModel>("spGetOrganisationInfoAutoCompleteByText @strType, @OrganisationName, @OrganisationTypeID,@ZipCode",
                new SqlParameter("strType", System.Data.SqlDbType.VarChar) { Value = "booking" },
                new SqlParameter("OrganisationName", System.Data.SqlDbType.VarChar) { Value = Prefix },
                new SqlParameter("OrganisationTypeID", System.Data.SqlDbType.Int) { Value = OrganizationTypeId },
                new SqlParameter("ZipCode", System.Data.SqlDbType.VarChar) { Value = ZipCode }
                ).ToList();

            return Json(pharmacyList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetZipcodeList(string ZipCode, int OrganizationTypeId)
        {
            var ZipcodeList = _repo.ExecWithStoreProcedure<ViewModels.CityStateZip>("SpGetZipcodebyserchtext @ZipCode",
                new SqlParameter("ZipCode", System.Data.SqlDbType.VarChar) { Value = ZipCode }
                ).ToList();

            return Json(ZipcodeList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDoctorList(string Prefix, int OrganizationTypeId)
        {
            var docList = _repo.ExecWithStoreProcedure<OrganisationAddressViewModel>("spGetDoctorInfoAutoComplete @Search",
                new SqlParameter("Search", System.Data.SqlDbType.VarChar) { Value = Prefix }
                ).ToList();

            return Json(docList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAddressbyOrgList(string Prefix, int OrganizationId, int OrganizationTypeId)//Added by Reena
        {
            var addressList = new List<OrgAddressDropdownViewModel>();
            var param = new JQueryDataTableParamModel();
            var datauserType = GetUserTypes("Patient");
            int userTypeId = 6;
            if (datauserType != null)
            {
                userTypeId = datauserType.UserTypeId;
            }
            var allPatientAddress = _repo.ExecWithStoreProcedure<OrganisationAddressViewModel>("spAddress_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = userTypeId },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = OrganizationId },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = OrganizationTypeId },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    ).ToList();
            addressList = allPatientAddress.Select(x => new OrgAddressDropdownViewModel() { AddressId = x.AddressId, FullAddress = (x.Address1 + ", " + x.City + ", " + x.State + ", " + x.Country + ", " + x.ZipCode) }).ToList();
            //ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress");

            return Json(addressList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("PatientProfileImageSave")]
        public JsonResult PatientProfileImageSave()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            //fname = file.FileName;
                            string extension = Path.GetExtension(file.FileName);
                            string newImageName = "Patient-" + DateTime.Now.Date.Ticks.ToString() + extension;
                            fname = newImageName;
                        }
                        // Get the complete folder path and store the file inside it.  
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/PatientSiteImages"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/PatientSiteImages"));
                        }
                        fname = Path.Combine(Server.MapPath("~/Uploads/PatientSiteImages/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public JsonResult GetOpeningHoursbyId(int OrganizationId, int userTypeId, DateTime? bookingDate)//Added by Reena
        {

            var list = _repo.ExecWithStoreProcedure<OrgOpeningHoursUpdateViewModel>("spOrgOpeningHoursById @OrganisationId, @UserTypeId, @BookingDate",
                       new SqlParameter("OrganisationId", System.Data.SqlDbType.NChar) { Value = OrganizationId },
                       new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = userTypeId },
                       new SqlParameter("BookingDate", System.Data.SqlDbType.Int) { Value = bookingDate }
                       );

            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Controller Common
        private int GetPatientId()
        {
            int userId = User.Identity.GetUserId<int>();
            return _patient.GetSingle(x => x.UserId == userId).PatientId;
        }

        private UserType GetUserTypes(string name)//Added by Reena
        {
            var allUserType = _usertype.GetAll(x => !x.IsDeleted && x.UserTypeName == name).FirstOrDefault();
            return allUserType;
        }

        private int GetInsuranceTypeId(int id)//Added by Reena
        {
            int InsuranceTypeId = 0;

            var planInfo = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceInfo_GetByPlanID @InsurancePlanId",
                   new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = id }
                   );

            InsuranceTypeId = planInfo.First().InsuranceTypeId;

            return InsuranceTypeId;
        }
        #endregion
    }
}
