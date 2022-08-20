using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using Doctyme.Model;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data;
using Doctyme.Repository.Enumerable;
using System.Data.Entity;
using System.Web;
using System.IO;
using Binke.App_Helpers;
using Doctyme.Repository.Interface;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Services;
using Binke.ViewModels;

namespace Binke.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {

        private readonly IDashboardService _dashboardService;
        private readonly IDoctorService _doctor;
        private readonly IFacilityService _facility;
        private readonly IPatientService _patient;
        private readonly ISeniorCareService _seniorCare;
        private readonly IRepository _repo;
        private readonly IUserService _appUser;
        private readonly IPharmacyService _pharmacy;//Added by Reena
        private readonly IUserTypeService _usertype;//Added by Reena
        private readonly IDrugService _drugService;
        // private readonly IDrugSe
        public DashboardController(IDrugService drugService, ISeniorCareService seniorCare, IPatientService patient, IFacilityService facility, IDashboardService dashboardService, IDoctorService doctor, RepositoryService repo, IUserService appUser, IPharmacyService pharmacyService, IUserTypeService userType)
        {
            _drugService = drugService;
            _patient = patient;
            _seniorCare = seniorCare;
            _facility = facility;
            _dashboardService = dashboardService;
            _doctor = doctor;
            _repo = repo;
            _appUser = appUser;
            _pharmacy = pharmacyService; //Added by Reena
            _usertype = userType;//Added by Reena
        }

        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Index()
        {
            try
            {
                if (User.IsInRole("SeniorCare"))
                {
                    int id = 0;
                    int userId = 0;
                    int orgId = 0;
                    string orgname = "";
                    string orgnpi = "";

                    userId = User.Identity.GetUserId<int>();
                    var _data = _repo.Find<Organisation>(w => w.UserId == userId);
                    if (_data != null)
                    {
                        orgId = _data.OrganisationId;
                        orgname = _data.OrganisationName;
                        orgnpi = _data.NPI;
                        id = _data.OrganisationId;
                    }

                    ViewBag.UserId = userId;
                    ViewBag.orgId = orgId;
                    ViewBag.orgname = orgname;
                    ViewBag.orgnpi = orgnpi;
                    ViewBag.isSeniorCareUser = true;
                    DashboardItemCount count = new DashboardItemCount();
                    count.DoctorCount = _doctor.GetAll().Count();
                    count.FacilityCount = _facility.GetAll().Count();
                    count.PatientCount = _patient.GetAll().Count();
                    count.SeniorCareCount = _seniorCare.GetAll().Count(c => c.OrganizationTypeID == 1007);
                    count.DrugCount = _drugService.GetAll().Count();
                    return View(count);
                    //return RedirectToAction("SeniorCare");
                }

                else
                {
                    if (User.IsInRole("Pharmacy"))
                    {
                        int userId = User.Identity.GetUserId<int>();
                        var userInfo = _appUser.GetById(userId);
                        var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                        TempData["PharmacyData"] = "Yes";
                        ViewBag.PharmacyID = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationId : 0;
                        Session["PharmacyID"] = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationId : 0;
                        var orgInfo = _repo.ExecWithStoreProcedure<OrganisationProfileViewModel>("spGetOrganizationInfoByID @orgnizationID",
                           new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = Convert.ToString(Session["PharmacyID"]) }
                           ).Select(x => new OrganisationProfileViewModel
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
                           }).First();
                        Session["PharmacyProfile"] = orgInfo.LogoFilePath;

                    }
                    if (User.IsInRole("Patient")) ////Added by Reena
                    {
                        int userId = User.Identity.GetUserId<int>();
                        Session["PatientID"] = userId;
                        ViewBag.PatientID = userId;
                    }
                    DashboardItemCount count = new DashboardItemCount();
                    //count.DoctorCount = _doctor.GetAll().Count();
                    //count.FacilityCount = _facility.GetAll().Count();
                    //count.PatientCount = _patient.GetAll().Count();
                    //count.SeniorCareCount = _seniorCare.GetAll().Count(c => c.OrganizationTypeID == 1007);

                    ////Added by Reena
                    //count.PatientCount = _patient.ExecWithStoreProcedure<PatientProfile>("spBasicPatientProfile_Get @PatientId",
                    //                     new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = 0 }).Count();
                    count.PatientCount = _appUser.GetAll(x => x.IsActive && !x.IsDeleted && x.UserType.UserTypeName == "Patient").Count();
                    count.DoctorCount = _doctor.GetCount(x => x.IsActive & !x.IsDeleted);
                    count.FacilityCount = _facility.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganizationTypeID == 1006);
                    count.PharmacyCount = _pharmacy.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganizationTypeID == 1005);
                    count.SeniorCareCount = _seniorCare.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganizationTypeID == 1007);
                    count.DrugCount = _drugService.GetAll().Count();
                    return View(count);
                }
            }
            catch (Exception ex)
            {
                Utility.Common.LogError(ex, "DashBoard/Index");
                return View();
            }
        }
        /// <summary>
        /// Load Doctor Dashboard
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = UserRoles.Doctor)]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Doctor()
        {
            try
            {
                var Doctor = APIHelper.GetAsyncById<Doctor>(Convert.ToInt32(User.Identity.GetUserId()), APIHelper.GetDoctorDetailsById);

                int DoctorId = Convert.ToInt32(User.Identity.GetUserId());
                int newdoctorId = GetDoctorId();
                var doctorlogin = new DoctorViewModel();
                if (newdoctorId != 0)
                {
                    DoctorId = newdoctorId;
                }
                if (User.Identity.GetClaimValue("UserRole") == "Doctor")
                {
                    int _doctorId = GetDoctorId();
                    doctorlogin = _repo.ExecWithStoreProcedure<DoctorViewModel>("spDoctor @Activity ,@Id  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = _doctorId })
                          .FirstOrDefault();
                }
                Session["DoctorSearch"] = GetDoctorId();
                ViewBag.DoctorId = doctorlogin.EnumerationDate == null ? 0 : Session["DoctorSearch"];
                ViewBag.NewPatientList = _dashboardService.GetNewPatientList(StoredProcedureList.GetNewPatientList, DoctorId, "2");
                ViewBag.RecentlyAddedNewPatientList = _dashboardService.GetRecentlyAddedNewPatientList(StoredProcedureList.GetRecentlyAddedNewPatientList, DoctorId, "2");
                ViewBag.RecentlyCompletedAppointmentList = _dashboardService.GetRecentlyCompletedAppointmentList(StoredProcedureList.GetRecentlyCompletedAppointmentList, DoctorId, "2");
                ViewBag.GetTodaysAppointmentList = _dashboardService.GetTodaysAppointmentList(StoredProcedureList.GetTodaysAppointmentList, DoctorId, "2");
                ViewBag.FromDate = DateTime.Now.AddYears(-1).ToString("MM/dd/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.DoctorProfile = doctorlogin.LogoFilePath;
                Session["DoctorProfile"] = doctorlogin.LogoFilePath;
                return View();
            }
            catch (Exception ex)
            {
                Utility.Common.LogError(ex, "DashBoard/Doctor");
                return View();
            }
        }
        /// <summary>
        /// Load Doctor Dashboard Graphs
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        private int GetDoctorId()
        {
            int userId = User.Identity.GetUserId<int>();
            var doctor = _doctor.GetDoctorbyUserId(userId);
            if (doctor != null)
            {
                return doctor.DoctorId;
            }
            else
            {
                return 0;
            }
        }
        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult LoadDoctorDashboardGraphs(string FromDate, string ToDate)
        {
            try
            {
                var Doctor = _doctor.GetByUserId(Convert.ToInt32(User.Identity.GetUserId()));
                int DoctorId = Doctor != null ? Doctor.DoctorId : 0;

                var MonthlyPatientCount = _dashboardService.GetDoctorDashboardPatientGraphData(StoredProcedureList.GetPatientsByUser, DoctorId, FromDate, ToDate, "2");
                var MonthlyAppointmentCount = _dashboardService.GetDoctorDashboardGraphDataByUser(StoredProcedureList.GetAppointmentsByUser, DoctorId, FromDate, ToDate, "2");

                var AppointmentGraphLabels = MonthlyAppointmentCount.Select(x => x.Label).ToArray();
                var AppointmentGraphData = MonthlyAppointmentCount.Select(x => x.Count).ToArray();

                var PatientGraphLabels = MonthlyPatientCount.Select(x => x.Label).ToArray();
                var PatientGraphData = MonthlyPatientCount.Select(x => x.Count).ToArray();

                var result = new object();
                result = new
                {
                    AppointmentGraphLabels = AppointmentGraphLabels,
                    AppointmentGraphData = AppointmentGraphData,
                    PatientGraphLabels = PatientGraphLabels,
                    PatientGraphData = PatientGraphData,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.Common.LogError(ex, "LoadDoctorDashboardGraphs/Doctor");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult LoadOrganizationDashboardGraphs(string FromDate, string ToDate)
        {
            int userId = User.Identity.GetUserId<int>();
            var userInfo = _appUser.GetById(userId);

            //string NPI = userInfo.Uniquekey;
            var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
            TempData["PharmacyData"] = "Yes";
            ViewBag.PharmacyID = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationId : 0;
            Session["PharmacyID"] = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationId : 0;
            Session["PharmacyName"] = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationName : "";
            int PharmacyId = ViewBag.PharmacyID;


            var MonthlyPatientCount = _dashboardService.GetDoctorDashboardPatientGraphData(StoredProcedureList.GetPatientsByUser, PharmacyId, FromDate, ToDate, "3");
            var MonthlyAppointmentCount = _dashboardService.GetDoctorDashboardGraphDataByUser(StoredProcedureList.GetAppointmentsByUser, PharmacyId, FromDate, ToDate, "3");

            var AppointmentGraphLabels = MonthlyAppointmentCount.Select(x => x.Label).ToArray();
            var AppointmentGraphData = MonthlyAppointmentCount.Select(x => x.Count).ToArray();

            var PatientGraphLabels = MonthlyPatientCount.Select(x => x.Label).ToArray();
            var PatientGraphData = MonthlyPatientCount.Select(x => x.Count).ToArray();

            var result = new object();
            result = new
            {
                AppointmentGraphLabels = AppointmentGraphLabels,
                AppointmentGraphData = AppointmentGraphData,
                PatientGraphLabels = PatientGraphLabels,
                PatientGraphData = PatientGraphData,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult LoadPatientDashboardGraphs(string FromDate, string ToDate)
        {
            int userId = User.Identity.GetUserId<int>();
            var userInfo = _appUser.GetById(userId);
            var patientInfo = _appUser.GetAll().Where(x => x.IsDeleted == false && x.Id == userId && x.IsActive == true);
            TempData["PatientData"] = "Yes";
            ViewBag.PatientID = patientInfo.Count() > 0 ? patientInfo.First().Id : 0;
            Session["PatientID"] = patientInfo.Count() > 0 ? patientInfo.First().Id : 0;
            Session["PatientName"] = patientInfo.Count() > 0 ? patientInfo.Select(x => x.FirstName + x.LastName).First() : "";
            int PatientId = ViewBag.PatientID;
            var datauserType = GetUserTypes("Patient");
            string userTypeId = "6";
            if (datauserType != null)
            {
                userTypeId = datauserType.UserTypeId.ToString();
            }
            //var patientOrders = _patient.ExecWithStoreProcedure<PatientGraphDetails>("spGetPatientGraphDetails",
            //   new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId },
            //   new SqlParameter("FromDate", System.Data.SqlDbType.VarChar) { Value = FromDate },
            //   new SqlParameter("ToDate", System.Data.SqlDbType.VarChar) { Value = ToDate },
            //   new SqlParameter("IsOrder", System.Data.SqlDbType.Bit) { Value = 1 }).ToList();

            //var patientbookings = _patient.ExecWithStoreProcedure<PatientGraphDetails>("spGetPatientGraphDetails",
            //   new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId },
            //   new SqlParameter("FromDate", System.Data.SqlDbType.VarChar) { Value = FromDate },
            //   new SqlParameter("ToDate", System.Data.SqlDbType.VarChar) { Value = ToDate },
            //   new SqlParameter("IsOrder", System.Data.SqlDbType.Bit) { Value = 0 }).ToList();

            var patientOrders = GetPatientGraphDetails(userId, FromDate, ToDate, true);
            var patientbookings = GetPatientGraphDetails(userId, FromDate, ToDate, false);

            var AppointmentGraphLabels = patientbookings.Select(x => x.Label).ToArray();
            var AppointmentGraphData = patientbookings.Select(x => x.Count).ToArray();

            var PatientGraphLabels = patientOrders.Select(x => x.Label).ToArray();
            var PatientGraphData = patientOrders.Select(x => x.Count).ToArray();

            var result = new object();
            result = new
            {
                AppointmentGraphLabels = AppointmentGraphLabels,
                AppointmentGraphData = AppointmentGraphData,
                PatientGraphLabels = PatientGraphLabels,
                PatientGraphData = PatientGraphData,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<PatientGraphDetails> GetPatientGraphDetails(int userId, string FromDate, string ToDate, bool IsOrder)
        {
            DataSet ds = _doctor.GetQueryResult("spGetPatientGraphDetails " + userId + ", '" + FromDate + "', '" + ToDate + "', " + IsOrder);
            DataColumn dcCount = ds.Tables[0].Columns["Count"];
            DataColumn dcLabel = ds.Tables[0].Columns["Label"];

            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => new PatientGraphDetails()
            {
                Count = dr.Field<int>(dcCount),
                Label = dr.Field<string>(dcLabel),
            }).ToList();
            return list;
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult LoadSeniorCareDashboardGraphs(string FromDate, string ToDate)
        {
            int userId = 0;
            int orgId = 0;
            string orgname = "";
            string orgnpi = "";


            userId = User.Identity.GetUserId<int>();
            var _data = _repo.Find<Organisation>(w => w.UserId == userId);
            if (_data != null)
            {
                orgId = _data.OrganisationId;
                orgname = _data.OrganisationName;
                orgnpi = _data.NPI;
            }

            ViewBag.UserId = userId;
            ViewBag.orgId = orgId;
            ViewBag.orgname = orgname;
            ViewBag.isSeniorCareUser = true;
            //  var Doctor = _doctor.GetByUserId(Convert.ToInt32(User.Identity.GetUserId()));
            // int DoctorId = Doctor != null ? Doctor.DoctorId : 0;

            var MonthlyPatientCount = _dashboardService.GetDoctorDashboardPatientGraphData(StoredProcedureList.GetPatientsByUser, orgId, FromDate, ToDate, "5");
            var MonthlyAppointmentCount = _dashboardService.GetDoctorDashboardGraphDataByUser(StoredProcedureList.GetAppointmentsByUser, orgId, FromDate, ToDate, "5");

            var AppointmentGraphLabels = MonthlyAppointmentCount.Select(x => x.Label).ToArray();
            var AppointmentGraphData = MonthlyAppointmentCount.Select(x => x.Count).ToArray();

            var PatientGraphLabels = MonthlyPatientCount.Select(x => x.Label).ToArray();
            var PatientGraphData = MonthlyPatientCount.Select(x => x.Count).ToArray();

            var result = new object();
            result = new
            {
                AppointmentGraphLabels = AppointmentGraphLabels,
                AppointmentGraphData = AppointmentGraphData,
                PatientGraphLabels = PatientGraphLabels,
                PatientGraphData = PatientGraphData,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult LoadFacilityDashboardGraphs(string FromDate, string ToDate)
        {
            int userId = 0;
            int orgId = 0;
            string orgname = "";
            string orgnpi = "";


            userId = User.Identity.GetUserId<int>();
            var _data = _repo.Find<Organisation>(w => w.UserId == userId);
            if (_data != null)
            {
                orgId = _data.OrganisationId;
                orgname = _data.OrganisationName;
                orgnpi = _data.NPI;
            }

            ViewBag.UserId = userId;
            ViewBag.orgId = orgId;
            ViewBag.orgname = orgname;

            //  var Doctor = _doctor.GetByUserId(Convert.ToInt32(User.Identity.GetUserId()));
            // int DoctorId = Doctor != null ? Doctor.DoctorId : 0;

            var MonthlyPatientCount = _dashboardService.GetDoctorDashboardPatientGraphData(StoredProcedureList.GetPatientsByUser, orgId, FromDate, ToDate, "4");
            var MonthlyAppointmentCount = _dashboardService.GetDoctorDashboardGraphDataByUser(StoredProcedureList.GetAppointmentsByUser, orgId, FromDate, ToDate, "4");

            var AppointmentGraphLabels = MonthlyAppointmentCount.Select(x => x.Label).ToArray();
            var AppointmentGraphData = MonthlyAppointmentCount.Select(x => x.Count).ToArray();

            var PatientGraphLabels = MonthlyPatientCount.Select(x => x.Label).ToArray();
            var PatientGraphData = MonthlyPatientCount.Select(x => x.Count).ToArray();

            var result = new object();
            result = new
            {
                AppointmentGraphLabels = AppointmentGraphLabels,
                AppointmentGraphData = AppointmentGraphData,
                PatientGraphLabels = PatientGraphLabels,
                PatientGraphData = PatientGraphData,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Facility)]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Facility()
        {
            int userId = 0;
            int orgId = 0;
            string orgname = "";
            string orgnpi = "";
            string enumerationDate = "";


            userId = User.Identity.GetUserId<int>();
            var _data = _repo.Find<Organisation>(w => w.UserId == userId);
            if (_data != null)
            {
                orgId = _data.OrganisationId;
                orgname = _data.OrganisationName;
                orgnpi = _data.NPI;
                enumerationDate = _data.EnumerationDate.ToString();
            }

            ViewBag.UserId = userId;
            ViewBag.orgId = orgId;
            ViewBag.orgname = orgname;


            ViewBag.NewPatientList = _dashboardService.GetNewPatientList(StoredProcedureList.GetNewPatientList, orgId, "4");
            ViewBag.RecentlyAddedNewPatientList = _dashboardService.GetRecentlyAddedNewPatientList(StoredProcedureList.GetRecentlyAddedNewPatientList, orgId, "4");
            ViewBag.RecentlyCompletedAppointmentList = _dashboardService.GetRecentlyCompletedAppointmentList(StoredProcedureList.GetRecentlyCompletedAppointmentList, orgId, "4");
            ViewBag.GetTodaysAppointmentList = _dashboardService.GetTodaysAppointmentList(StoredProcedureList.GetTodaysAppointmentList, orgId, "4");
            ViewBag.FromDate = DateTime.Now.AddYears(-1).ToString("MM/dd/yyyy");
            ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
            //var orgId = _facility.SQLQuery<int>("select OrganisationId from Organisation where UserId = " + userId.ToString() + " and IsActive = 1 and IsDeleted = 0 order by createdby desc").FirstOrDefault();
            //Changes made against Issue#25
            ViewBag.EnumerationDate = enumerationDate;
            return View();
        }

        [Authorize(Roles = UserRoles.Patient)]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Patient()
        {
            ////Added by Reena
            int userId = User.Identity.GetUserId<int>();
            Session["PatientID"] = userId;
            //var patientDetails = _patient.ExecWithStoreProcedure<PatientProfile>("spBasicPatientProfile_Get @PatientId",
            //             new SqlParameter("@PatientId", System.Data.SqlDbType.Int) { Value = userId }).FirstOrDefault();
            var patientDetails = GetPatientDetails(userId);
            var datauserType = GetUserTypes("Patient");
            string userTypeId = "6";
            if (datauserType != null)
            {
                userTypeId = datauserType.UserTypeId.ToString();
            }
            ViewBag.RecentlyCompletedAppointmentList = _dashboardService.GetRecentlyCompletedAppointmentList(StoredProcedureList.GetRecentlyCompletedAppointmentList, userId, userTypeId);
            ViewBag.GetTodaysAppointmentList = _dashboardService.GetTodaysAppointmentList(StoredProcedureList.GetTodaysAppointmentList, userId, userTypeId);
            ViewBag.FromDate = DateTime.Now.AddYears(-1).ToString("MM/dd/yyyy");
            ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
            var newPatientList = _patient.ExecWithStoreProcedure<PatientOrders>("GetNewPatientOrderList @PatientId",
                new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId }).ToList();
            var recentlyAddedNewPatientList = _patient.ExecWithStoreProcedure<PatientOrders>("GetRecentlyAddedNewPatientOrderList @PatientId",
                new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId }).ToList();
            ViewBag.NewOrderList = newPatientList;
            ViewBag.RecentlyAddedOrdersList = recentlyAddedNewPatientList;

            var recentlyCompletedBookingList = _patient.ExecWithStoreProcedure<PatientBooking>("RecentlyCompletedBookingList @PatientId",
                new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId }).ToList();
            var getTodayBookingList = _patient.ExecWithStoreProcedure<PatientBooking>("GetTodayPatientBookingList @PatientId",
                new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId }).ToList();
            ViewBag.RecentlyCompletedBookingList = recentlyCompletedBookingList;
            ViewBag.GetTodayBookingList = getTodayBookingList;

            ////var patientOrders = _patient.ExecWithStoreProcedure<PatientOrders>("spPatientOrderListByPatientId @PatientId",
            ////    new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId }
            ////                ).ToList();
            ////ViewBag.PatientOrderList = patientOrders;
            ////var bookings = _patient.ExecWithStoreProcedure<PatientBooking>("spPatientBookingByPatientId @Search, @PatientId, @PageIndex, @PageSize, @Sort", //Added by Reena
            ////                new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = " " },
            ////                new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = userId },
            ////                new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = 1 },
            ////                new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = 100 },
            ////                new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = "Asc" }).ToList();

            ////ViewBag.PatientBookingList = bookings;
            return View(patientDetails);
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
            DataColumn dcCityStateZipCodeID = ds.Tables[0].Columns["CityStateZipCodeID"];
            DataColumn dcAddressId = ds.Tables[0].Columns["AddressId"];
            DataColumn dcDoctorId = ds.Tables[0].Columns["DoctorId"];

            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => new PatientProfile(){
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
                CityStateZipCodeID = dr.Field<int?>(dcCityStateZipCodeID),
                AddressId = dr.Field<int?>(dcAddressId),
                DoctorId = dr.Field<int?>(dcDoctorId)
            }).FirstOrDefault();
            return list;
        }

        [Authorize(Roles = UserRoles.Pharmacy)]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Pharmacy()
        {
            int userId = User.Identity.GetUserId<int>();
            var userInfo = _appUser.GetById(userId);


            string NPI = userInfo.Uniquekey;
            var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
            TempData["PharmacyData"] = "Yes";
            ViewBag.PharmacyID = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationId : 0;
            Session["PharmacyID"] = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationId : 0;
            Session["PharmacyName"] = pharmacyInfo.Count() > 0 ? pharmacyInfo.First().OrganisationName : "";
            int PharmacyId = ViewBag.PharmacyID;
            ViewBag.NewPatientList = _dashboardService.GetNewPatientList(StoredProcedureList.GetNewPatientList, PharmacyId, "3");
            ViewBag.RecentlyAddedNewPatientList = _dashboardService.GetRecentlyAddedNewPatientList(StoredProcedureList.GetRecentlyAddedNewPatientList, PharmacyId, "3");
            ViewBag.RecentlyCompletedAppointmentList = _dashboardService.GetRecentlyCompletedAppointmentList(StoredProcedureList.GetRecentlyCompletedAppointmentList, PharmacyId, "3");
            ViewBag.GetTodaysAppointmentList = _dashboardService.GetTodaysAppointmentList(StoredProcedureList.GetTodaysAppointmentList, PharmacyId, "3");
            ViewBag.FromDate = DateTime.Now.AddYears(-1).ToString("MM/dd/yyyy");
            ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
            var orgInfo = _repo.ExecWithStoreProcedure<OrganisationProfileViewModel>("spGetOrganizationInfoByID @orgnizationID",
                        new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = Convert.ToString(Session["PharmacyID"]) }
                        ).Select(x => new OrganisationProfileViewModel
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
            //Session["PharmacyProfile"] = orgInfo != null ? orgInfo.LogoFilePath : string.Empty;
            Session["PharmacyProfile"] = userInfo != null ? userInfo.ProfilePicture.TrimStart('~') : string.Empty;
            return View();
        }

        [Authorize(Roles = UserRoles.SeniorCare)]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult SeniorCare()
        {

            int userId = 0;
            int orgId = 0;
            string orgname = "";
            string orgImagePath = "";
            string orgnpi = "";
            userId = User.Identity.GetUserId<int>();
            var _data = _repo.Find<Organisation>(w => w.UserId == userId);
            if (_data != null)
            {
                orgId = _data.OrganisationId;
                orgname = _data.OrganisationName;
                orgnpi = _data.NPI;
                orgImagePath = _data.LogoFilePath;
            }

            ViewBag.UserId = userId;
            ViewBag.orgId = orgId;
            ViewBag.orgname = orgname;
            ViewBag.isSeniorCareUser = true;
            ViewBag.SeniorcareId = _data.EnumerationDate == null ? 0 : _data.OrganisationId;
            ViewBag.NewPatientList = _dashboardService.GetNewPatientList(StoredProcedureList.GetNewPatientList, orgId, "5");
            ViewBag.RecentlyAddedNewPatientList = _dashboardService.GetRecentlyAddedNewPatientList(StoredProcedureList.GetRecentlyAddedNewPatientList, orgId, "5");
            ViewBag.RecentlyCompletedAppointmentList = _dashboardService.GetRecentlyCompletedAppointmentList(StoredProcedureList.GetRecentlyCompletedAppointmentList, orgId, "5");
            ViewBag.GetTodaysAppointmentList = _dashboardService.GetTodaysAppointmentList(StoredProcedureList.GetTodaysAppointmentList, orgId, "5");
            ViewBag.FromDate = DateTime.Now.AddYears(-1).ToString("MM/dd/yyyy");
            ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
            //return RedirectToAction("SeniorCareProfile", "SeniorCare", new { id = 0, flag = 1, isSeniorCareUser = true });
            Session["SeniorCareProfileImg"] = "/Uploads/SeniorCareProfile/" + orgImagePath;
            return View();
        }

        private UserType GetUserTypes(string name)//Added by Reena
        {
            var allUserType = _usertype.GetAll(x => !x.IsDeleted && x.UserTypeName == name).FirstOrDefault();
            return allUserType;
        }
    }
}
