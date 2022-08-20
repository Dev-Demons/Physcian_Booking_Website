using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Model.ViewModels;
using Binke.Utility;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data;
using Doctyme.Repository.Enumerable;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using System.Web;
using System.IO;
using Binke.App_Helpers;
using System.Globalization;
using Binke.Models;
using Binke.ViewModels;

namespace Binke.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IAddressService _address;
        private readonly ICityService _city;
        private readonly ICityStateZipService _cityStateZip;

        private readonly IDoctorService _doctor;
        private readonly IAgeGroupService _agegroup;
        private readonly IDoctorFacilityAffiliationService _doctorFacilityAffiliation;
        private readonly IDoctorInsuranceAcceptedService _doctorInsuranceAccepted;
        private readonly IDoctorImageService _doctorImage;
        private readonly IDoctorSpecialityService _doctorSpeciality;
        private readonly IDoctorInsuranceService _doctorInsurance;
        private readonly IDoctorAgeGroupService _doctoragegroup;
        private readonly IExperienceService _experience;
        private readonly IFacilityService _facility;
        private readonly IFeaturedDoctorService _featuredDoctor;
        private readonly IFeaturedSpecialityService _featuredSpeciality;
        private readonly IOpeningHourService _openingHour;
        private readonly ISlotService _slot;
        private readonly IStateService _state;
        private readonly ISpecialityService _speciality;
        private readonly ISocialMediaService _socialMedia;
        private readonly IQualificationService _qualification;
        private readonly IUserService _appUser;
        private readonly IDoctorLanguageService _doctorLanguageService;
        private readonly ILanguageService _languageService;
        private readonly IRepository _repo;
        private readonly IErrorLogService _errorLog;

        private ApplicationUserManager _userManager;
        private readonly IPatientService _patient;

        private int _doctorId;
        public DoctorController(IAddressService address, ICityService city, ICityStateZipService cityStateZip, IDoctorService doctor, IAgeGroupService agegroup, IDoctorFacilityAffiliationService doctorFacilityAffiliation, IDoctorImageService doctorImage, ILanguageService languageService,
            IDoctorSpecialityService doctorSpeciality, IDoctorInsuranceService doctorInsurance, IDoctorAgeGroupService doctoragegroup, IExperienceService experience, IFacilityService facility, IFeaturedDoctorService featuredDoctor, IFeaturedSpecialityService featuredSpeciality, IDoctorLanguageService doctorLanguageService,
            IOpeningHourService openingHour, ISlotService slot, IStateService state, ISpecialityService speciality, ISocialMediaService socialMedia, IQualificationService qualification, IDoctorInsuranceAcceptedService insurance, IUserService appUser, IRepository repo, ApplicationUserManager userManager, IErrorLogService errorLog,
            IPatientService patient)
        {
            _address = address;
            _city = city;
            _cityStateZip = cityStateZip;
            _doctor = doctor;
            _agegroup = agegroup;
            _doctorFacilityAffiliation = doctorFacilityAffiliation;
            _doctorImage = doctorImage;
            _doctorSpeciality = doctorSpeciality;
            _doctorInsurance = doctorInsurance;
            _doctoragegroup = doctoragegroup;
            _experience = experience;
            _facility = facility;
            _featuredDoctor = featuredDoctor;
            _featuredSpeciality = featuredSpeciality;
            _openingHour = openingHour;
            _slot = slot;
            _state = state;
            _speciality = speciality;
            _socialMedia = socialMedia;
            _qualification = qualification;
            _appUser = appUser;
            _doctorLanguageService = doctorLanguageService;
            //_doctorInsuranceAccepted = insurance;
            _languageService = languageService;
            _repo = repo;
            _userManager = userManager;
            _errorLog = errorLog;
            _patient = patient;
        }

        #region Admin Section
        // GET: Doctor
        public ActionResult Index()
        {
            if (User.Identity.GetClaimValue("UserRole") == "Doctor")
            {
                int docid = GetDoctorId();
                if (docid == 0)
                {
                    return RedirectToAction("DoctorProfile");
                }
                else
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    return View();
                }

            }
            else

            {
                Session["DoctorSearch"] = 0;
            }
            return View();

        }
        [HttpPost, Route("AddEditDoctorSave")]
        [ValidateInput(false)]
        public JsonResult AddEditDoctorSave(DoctorViewModel model, HttpPostedFileBase Image1)
        {
            if (_doctor.GetSingle(x => x.NPI == model.NPI && x.DoctorId != model.DoctorId) == null)
            {

                string imagePath = "";
                if (model.LogoFilePath == null)
                    model.LogoFilePath = imagePath;
                if (Image1 != null && Image1.ContentLength > 0)
                {
                    DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/DoctorSiteImages/"));
                    if (!dir.Exists)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/DoctorSiteImages"));
                    }

                    string extension = Path.GetExtension(Image1.FileName);
                    string newImageName = "Doctor-" + DateTime.Now.Ticks.ToString() + extension;

                    var path = Path.Combine(Server.MapPath("~/Uploads/DoctorSiteImages"), newImageName);
                    Image1.SaveAs(path);

                    imagePath = newImageName;
                    model.LogoFilePath = imagePath;
                }

                if (model.DoctorId > 0 && imagePath == "")
                {
                    imagePath = model.LogoFilePath != null ? model.LogoFilePath : "";
                    if (model.LogoFilePath == null)
                        model.LogoFilePath = imagePath;
                }

               // model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
                //model.IsPrimaryCare = Convert.ToString(Request["IsPrimaryCare"]) == "on" ? true : false;
                //model.IsNtPcp = Convert.ToString(Request["IsNtPcp"]) == "on" ? true : false;
                //model.IsAllowNewPatient = Convert.ToString(Request["IsAllowNewPatient"]) == "on" ? true : false;
              //  model.EnabledBooking = Convert.ToString(Request["EnabledBooking"]) == "on" ? true : false;
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDoctor_Create " +
                            "@UserId," +
                            "@NamePrefix," +
                            "@NameSuffix," +
                            "@FirstName," +
                            "@LastName," +
                            "@MiddleName," +
                            "@Gender," +
                            "@Status," +
                            "@NPI," +
                            "@Education," +
                            "@EnumerationDate," +
                            "@ShortDescription," +
                            "@LongDescription," +
                            "@SoleProprietor," +
                            "@IsAllowNewPatient," +
                            "@IsNtPcp," +
                            "@IsPrimaryCare," +
                            "@IsActive," +
                            "@EnabledBooking," +
                            "@Keywords," +
                            "@PracticeStartDate," +
                            "@Id," +
                            "@CreatedBy," +
                            "@Language," +
                            "@OtherNames," +
                             "@Credential," +
                             "@LogoFilePath",
                                           new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("NamePrefix", System.Data.SqlDbType.VarChar) { Value = model.NamePrefix == null ? "" : model.NamePrefix },
                                          new SqlParameter("NameSuffix", System.Data.SqlDbType.VarChar) { Value = model.NameSuffix == null ? "" : model.NameSuffix },
                                          new SqlParameter("FirstName", System.Data.SqlDbType.VarChar) { Value = model.FirstName == null ? "" : model.FirstName },
                                         new SqlParameter("LastName", System.Data.SqlDbType.VarChar) { Value = model.LastName == null ? "" : model.LastName },
                                          new SqlParameter("MiddleName", System.Data.SqlDbType.VarChar) { Value = model.MiddleName == null ? "" : model.MiddleName },
                                          new SqlParameter("Gender", System.Data.SqlDbType.VarChar) { Value = string.IsNullOrEmpty(model.Gender) ? "" : model.Gender },
                                          new SqlParameter("Status", System.Data.SqlDbType.VarChar) { Value = model.Status == null ? "" : model.Status },
                                          new SqlParameter("NPI", System.Data.SqlDbType.VarChar) { Value = model.NPI },
                                          new SqlParameter("Education", System.Data.SqlDbType.VarChar) { Value = model.Education == null ? "" : model.Education },
                                          new SqlParameter("EnumerationDate", System.Data.SqlDbType.Date) { Value = model.EnumerationDate },
                                          new SqlParameter("ShortDescription", System.Data.SqlDbType.VarChar) { Value = model.ShortDescription == null ? "" : model.ShortDescription },
                                          new SqlParameter("LongDescription", System.Data.SqlDbType.VarChar) { Value = model.LongDescription == null ? "" : model.LongDescription },
                                          new SqlParameter("SoleProprietor", System.Data.SqlDbType.Bit) { Value = model.SoleProprietor },
                                          new SqlParameter("IsAllowNewPatient", System.Data.SqlDbType.Bit) { Value = model.IsAllowNewPatient },
                                          new SqlParameter("IsNtPcp", System.Data.SqlDbType.Bit) { Value = model.IsNtPcp },

                                           new SqlParameter("IsPrimaryCare", System.Data.SqlDbType.Bit) { Value = model.IsPrimaryCare },
                                           new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                           new SqlParameter("EnabledBooking", System.Data.SqlDbType.Bit) { Value = model.EnabledBooking },
                                          new SqlParameter("Keywords", System.Data.SqlDbType.VarChar) { Value = model.Keywords == null ? "" : model.Keywords },
                                            new SqlParameter("PracticeStartDate", System.Data.SqlDbType.DateTime) { Value = model.PracticeStartDate },
                                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = model.DoctorId },


                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                            new SqlParameter("Language", System.Data.SqlDbType.VarChar) { Value = model.Language == null ? "" : model.Language },
                                              new SqlParameter("OtherNames", System.Data.SqlDbType.VarChar) { Value = model.OtherNames == null ? "" : model.OtherNames },

                                          new SqlParameter("Credential", System.Data.SqlDbType.VarChar) { Value = model.Credential },
                                           new SqlParameter("LogoFilePath", System.Data.SqlDbType.VarChar) { Value = model.LogoFilePath }
                           );
                Session["DoctorProfile"] = model.LogoFilePath;

                var userId = User.Identity.GetUserId<int>();
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("PhoneNumber", SqlDbType.NVarChar) { Value = model.PhoneNumber });
                _patient.ExecuteSqlCommandForUpdate("AspNetUsers", "Id", userId, parameters);

                return Json(new JsonResponse() { Status = 1, Message = "Doctor save successfully" });
            }
            else
            {
                return Json(new JsonResponse() { Status = 0, Message = "NPI Already Exists" });
            }
        }
        [HttpGet, Route("AddEditDoctor/{id?}")]
        public ActionResult AddEditDoctor(int? id)
        {
            //int docBoardCertificationid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorViewModel>("spDoctor @Activity ,@Id  ",
                         new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                         new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                         .FirstOrDefault();
            // var result = _repo.All<DoctorBoardCertification>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();
            var list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorLanguage @Activity  ",
                new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "LanguageList" }
             ).ToList();
            if (id != 0 && id != null)
            {
                var Doctor = result;
                Doctor.LanguagesList = list;
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Doctor.DoctorName = doctor.FirstName + " " + doctor.LastName;
                Doctor.DoctorId = Convert.ToInt32(DoctorId);
                return View(Doctor);
            }
            else
            {
                var Doctor = new DoctorViewModel();
                Doctor.LanguagesList = list;
               // Doctor.NPI = Convert.ToString(new Random().Next(11111, 99999)) + Convert.ToString(new Random().Next(11111, 99999));
                return View(Doctor);
            }

        }
        [HttpGet, Route("AddEditViewDoctor/{id?}")]
        public ActionResult AddEditViewDoctor(int? id)
        {
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorViewModel>("spDoctor @Activity ,@Id  ",
                         new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                         new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                        .FirstOrDefault();
            var list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorLanguage @Activity  ",
               new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "LanguageList" }
            ).ToList();
            if (id != 0 && id != null)
            {
                result.LanguagesList = list;
                var DoctorId = Convert.ToString(Session["DoctorSearch"]);
                return View("AddEditDoctor", result);
            }
            else
            {
                var Doctor = new DoctorViewModel();
                Doctor.LanguagesList = list;
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Doctor.DoctorName = doctor.FirstName + " " + doctor.LastName;
                Doctor.IsViewMode = true;
                Doctor.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctor", Doctor);
            }

        }
        [Route("GetDoctorById/{id}")]
        public ActionResult GetDoctorById(string id)
        {
            Session["DoctorSearch"] = id;
            return RedirectToAction("DoctorAgeGroup");
        }
        //[Route("GetDoctorList/{flag}")]
        //public ActionResult GetDoctorList(bool flag, JQueryDataTableParamModel param)
        //{
        //    Dictionary<int, string> SortColumns = new Dictionary<int, string>();
        //    SortColumns.Add(0, "FullName");
        //    SortColumns.Add(1, "Email");
        //    SortColumns.Add(2, "Gender");
        //    SortColumns.Add(3, "Npi");
        //    SortColumns.Add(4, "IsActive");
        //    SortColumns.Add(5, "IsBooking");
        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
        //    Pagination objPagination = new Pagination()
        //    {
        //        StartIndex = param.iDisplayStart,
        //        PageSize = param.iDisplayLength,
        //        SortColumnName = SortColumns[sortColumnIndex],
        //        SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
        //    };
        //    objPagination.Search = param.sSearch ?? "";
        //    var allDoctorsList = App_Helpers.APIHelper.GetSyncListPagination<DoctorBasicInformation>(objPagination, App_Helpers.APIHelper.GetAllDoctorsList);
        //    var display = allDoctorsList;
        //    var total = allDoctorsList.Count > 0 ? allDoctorsList.FirstOrDefault().TotalRows : 0;
        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);
        //}

        [Route("DoctorProfile/{id?}/{flag?}")]
        public ActionResult DoctorProfile(int? id, bool flag = false)
        {
            var doctor = new DoctorViewModel();
            if (User.Identity.GetClaimValue("UserRole") == "Doctor")
            {
                _doctorId = GetDoctorId();
                if (_doctorId != 0)
                {
                    Session["DoctorSearch"] = _doctorId;
                    id = _doctorId;
                }

                else
                {
                    Session["DoctorSearch"] = _doctorId;
                    id = _doctorId;
                }
            }
            else
            {
                Session["DoctorSearch"] = id;
                //var doctors = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
                var doctors = _doctor.GetDoctorDetailsById(Convert.ToInt32(id));
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";

            }
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorViewModel>("spDoctor @Activity ,@Id  ",
                         new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                      new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                      .FirstOrDefault();
            ViewBag.DoctorId = result.EnumerationDate == null ? 0 : id;
            // var result = _repo.All<DoctorBoardCertification>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();
            var list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorLanguage @Activity  ",
                new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "LanguageList" }
             ).ToList();
            if (id != 0 && id != null)
            {
                var Doctor = result;
                Doctor = Doctor ?? new DoctorViewModel();
                Doctor.LanguagesList = list;
                var DoctorId = Convert.ToString(Session["DoctorSearch"]);
                //var doctors = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
                var doctors = _doctor.GetDoctorDetailsById(Convert.ToInt32(id));
                Doctor.DoctorName = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
                Doctor.DoctorId = Convert.ToInt32(DoctorId);
                if (flag)
                {
                    Doctor.IsViewMode = true;
                }
                return View(Doctor);
            }
            else
            {
                var Doctor = new DoctorViewModel();
                Doctor = Doctor ?? new DoctorViewModel();
                Doctor.LanguagesList = list;
                //Doctor.NPI = Convert.ToString(new Random().Next(11111, 99999)) + Convert.ToString(new Random().Next(11111, 99999));
                //while (_doctor.GetSingle(x => x.NPI == Doctor.NPI) != null)
                //{
                //    Doctor.NPI = Convert.ToString(new Random().Next(11111, 99999)) + Convert.ToString(new Random().Next(11111, 99999));
                //}
                return View(Doctor);
            }


        }
        [HttpPost, Route("DeleteDoctor/{id?}")]
        public JsonResult DeleteDoctor(int id)
        {
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("sprDeleteDoctor " +
                     "@Id"
                        ,
                             new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id }
                        );
            return Json(new JsonResponse() { Status = 1, Message = "Doctor deleted successfully" });
        }
        [HttpPost, Route("ActiveDeActiveDoctor/{flag}/{id}")]
        public JsonResult ActiveDeActiveDoctor(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctor @Activity ,@Id,@IsActive  ",
                               new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Activate" },
                            new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                            new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = !flag })
                           ;

            //var doctor = _doctor.GetById(id);
            //doctor.IsActive = !flag;
            //doctor.DoctorUser.IsActive = !flag;
            //_doctor.UpdateData(doctor);
            //_doctor.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The doctor has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }

        [HttpPost, Route("ChangeBookingDoctor/{flag}/{id}")]
        public JsonResult ChangeBookingDoctor(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctor @Activity ,@Id,@IsActive  ",
                               new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Booking" },
                            new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                            new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = !flag })
                           ;

            //var doctor = _doctor.GetById(id);
            //doctor.IsActive = !flag;
            //doctor.DoctorUser.IsActive = !flag;
            //_doctor.UpdateData(doctor);
            //_doctor.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The doctor has {(flag ? "not" : "")} been booking successfully" });
        }

        //[Route("GetDoctorById/{id}")]
        //public ActionResult GetDoctorById(string id)
        //{
        //    // HttpCookie cookies = new HttpCookie("DoctorSearch");
        //    //cookies["DoctorID"] = id;
        //    //cookies.HttpOnly = true;
        //    Session["DoctorSearch"] = id;
        //    //Response.Cookies.Add(cookies);
        //    return RedirectToAction("DoctorAgeGroup");
        //}
        [Route("GetDoctorList/{flag}")]
        public ActionResult GetDoctorList(bool flag, JQueryDataTableParamModel param)
        {
            Dictionary<int, string> SortColumns = new Dictionary<int, string>();
            SortColumns.Add(0, "FullName");
            SortColumns.Add(1, "Email");
            SortColumns.Add(2, "Gender");
            SortColumns.Add(3, "Npi");
            SortColumns.Add(4, "IsActive");
            SortColumns.Add(5, "IsBooking");
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            Pagination objPagination = new Pagination()
            {
                StartIndex = param.iDisplayStart,
                PageSize = param.iDisplayLength,
                SortColumnName = SortColumns[sortColumnIndex],
                SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
            };
            objPagination.Search = param.sSearch ?? "";
            var allDoctorsList = _doctor.GetDoctors(objPagination);// App_Helpers.APIHelper.GetSyncListPagination<DoctorBasicInformation>(objPagination, App_Helpers.APIHelper.GetAllDoctorsList);
            var display = allDoctorsList;
            var total = allDoctorsList.Count > 0 ? allDoctorsList.FirstOrDefault().TotalRows : 0;
            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("Profile/{npi?}")]
        public ActionResult Profile(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("Index", "Doctor");
            }
            var doctor = _doctor.GetSingle(x => x.NPI == npi);
            return View(doctor);
        }
        //[Route("Profile/{id?}")]
        //public ActionResult Profile(int? id)
        //{
        //    if (User.Identity.GetClaimValue("UserRole") == "Doctor")
        //    {
        //        _doctorId = GetDoctorId();

        //        Session["DoctorSearch"] = _doctorId;

        //    }
        //    else
        //    {
        //        Session["DoctorSearch"] = id;
        //    }

        //    var doctor = _doctor.GetSingle(x => x.DoctorId == id);
        //    if(doctor==null)
        //    {
        //        doctor=

        //    }
        //    return View(doctor);
        //}
        //[HttpPost, Route("ActiveDeActiveDoctor/{flag}/{id}")]
        //public JsonResult ActiveDeActiveDoctor(bool flag, int id)
        //{
        //    var doctor = _doctor.GetById(id);
        //    doctor.IsActive = !flag;
        //    doctor.DoctorUser.IsActive = !flag;
        //    _doctor.UpdateData(doctor);
        //    _doctor.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = $@"The doctor has been {(flag ? "deactivated" : "reactivated")} successfully" });
        //}
        #endregion
        //#region DoctorProfile

        //[HttpPost, Route("ActiveDeDoctorProfile/{flag}/{id}")]
        //public JsonResult ActiveDeDoctorProfile(bool flag, int id)
        //{
        //    int count = _repo.ExecuteSQLQuery("spDoctorProfile @Activity ,@Id,@IsActive  ",
        //                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Activate" },
        //                  new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
        //                  new SqlParameter("IsActive", System.Data.SqlDbType.Int) { Value = !flag })
        //                 ;
        //    //var doctor = _doctor.GetById(id);
        //    //doctor.IsActive = !flag;
        //    //doctor.DoctorUser.IsActive = !flag;
        //    //_doctor.UpdateData(doctor);
        //    //_doctor.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = $@"The Board Certification has been {(flag ? "deactivated" : "reactivated")} successfully" });
        //}

        //[HttpPost, Route("AddEditDoctorBoardCertification"), ValidateAntiForgeryToken]
        //public JsonResult AddEditDoctorBoardCertification(DoctorBoardCertificationViewModel model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {

        //            if (model.DoctorBoardCertificationId == 0)
        //            {

        //                var bResult = _repo.Find<DoctorBoardCertification>(x => x.BoardCertificationId == model.BoardCertificationId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
        //                if (bResult != null)
        //                {

        //                    txscope.Dispose();
        //                    return Json(new JsonResponse { Status = 0, Message = " Board Certification already exists. Could not add the data!" });
        //                }

        //                var BoardCertification = new DoctorBoardCertification()
        //                {
        //                    DoctorBoardCertificationId = model.DoctorBoardCertificationId,
        //                    BoardCertificationId = Convert.ToInt16(model.BoardCertificationId),
        //                    DoctorId = model.DoctorId,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    CreatedDate = DateTime.Now
        //                };


        //                _repo.Insert<DoctorBoardCertification>(BoardCertification, true);
        //                // _BoardCertification.SaveData();
        //                txscope.Complete();
        //                return Json(new JsonResponse { Status = 1, Message = " Board Certification save successfully" });
        //            }
        //            else
        //            {

        //                var bResultdata = _repo.Find<DoctorBoardCertification>(x => x.BoardCertificationId == model.BoardCertificationId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
        //                if (bResultdata != null)
        //                {
        //                    txscope.Dispose();
        //                    return Json(new JsonResponse { Status = 0, Message = " Board Certification already exists. Could not add the data!" });
        //                }

        //                var age = _repo.All<DoctorBoardCertification>().ToList().Where(x => x.DoctorBoardCertificationId == model.DoctorBoardCertificationId).FirstOrDefault();
        //                age.BoardCertificationId = Convert.ToInt16(model.BoardCertificationId);

        //                _repo.Update<DoctorBoardCertification>(age, true);
        //                // _BoardCertification.SaveData();
        //                txscope.Complete();
        //                return Json(new JsonResponse { Status = 1, Message = " Board Certification update successfully" });
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            Common.LogError(ex, "AddEditDoctorBoardCertification-Post");
        //            return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
        //        }
        //    }
        //}
        //[HttpGet, Route("AddEditDoctorBoardCertification/{id?}")]
        //public ActionResult AddEditDoctorBoardCertification(int? id)
        //{
        //    //int docBoardCertificationid = Convert.ToInt32(id);
        //    id = id ?? 0;
        //    var result = _repo.ExecWithStoreProcedure<DoctorBoardCertificationViewModel>("spDoctorBoardCertification @Activity ,@Id  ",
        //               new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
        //            new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
        //            .FirstOrDefault();
        //    // var result = _repo.All<DoctorBoardCertification>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();
        //    var list = new List<DropDownModel>();
        //    if (result != null)
        //    {
        //        //ViewBag.BoardCertificationList = .Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.BoardCertificationId.ToString(),
        //        //    Selected = (x.BoardCertificationId == result.BoardCertificationId)
        //        //}).ToList();
        //        list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
        //            new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
        //         ).ToList();

        //        //  list = _repo.All<BoardCertifications>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
        //    }
        //    else
        //    {
        //        //ViewBag.BoardCertificationList = _BoardCertification.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.BoardCertificationId.ToString(),

        //        //}).ToList();
        //        list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
        //             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
        //          ).ToList();

        //    }
        //    if (id != 0 && id != null)
        //    {
        //        var BoardCertification = result;

        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);
        //        var doctor = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
        //        BoardCertification.DoctorName =doctor==null?"":doctor.FirstName + " " + doctor.LastName;
        //        BoardCertification.BoardCertificationsList = list;
        //        BoardCertification.DoctorId = Convert.ToInt32(DoctorId);
        //        return View(BoardCertification);
        //    }
        //    else
        //    {
        //        var BoardCertification = new DoctorBoardCertificationViewModel();
        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);
        //        var doctor = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
        //        BoardCertification.DoctorName =doctor==null?"":doctor.FirstName + " " + doctor.LastName;
        //        BoardCertification.BoardCertificationsList = list;
        //        BoardCertification.DoctorId = Convert.ToInt32(DoctorId);
        //        return View(BoardCertification);
        //    }

        //}

        //[HttpGet, Route("AddEditViewDoctorBoardCertification/{id?}")]
        //public ActionResult AddEditViewDoctorBoardCertification(int? id)
        //{
        //    //int docBoardCertificationid = Convert.ToInt32(id);
        //    var result = _repo.All<DoctorBoardCertificationViewModel>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();
        //    var list = new List<DropDownModel>();
        //    if (result != null)
        //    {
        //        //ViewBag.BoardCertificationList = .Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.BoardCertificationId.ToString(),
        //        //    Selected = (x.BoardCertificationId == result.BoardCertificationId)
        //        //}).ToList();
        //        list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
        //         new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
        //      ).ToList();

        //    }
        //    else
        //    {
        //        //ViewBag.BoardCertificationList = _BoardCertification.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.BoardCertificationId.ToString(),

        //        //}).ToList();
        //        list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
        //         new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
        //      ).ToList();


        //    }
        //    if (id != 0 && id != null)
        //    {



        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);

        //        result.BoardCertificationsList = list;
        //        // result.DoctorId = Convert.ToInt32(DoctorId);
        //        return View("AddEditDoctorBoardCertification", result);
        //    }
        //    else
        //    {
        //        var BoardCertification = new DoctorBoardCertificationViewModel();
        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);
        //        var doctor = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
        //        BoardCertification.DoctorName =doctor==null?"":doctor.FirstName + " " + doctor.LastName;
        //        BoardCertification.BoardCertificationsList = list;

        //        BoardCertification.DoctorId = Convert.ToInt32(DoctorId);
        //        return View("AddEditDoctorBoardCertification", BoardCertification);
        //    }

        //}


        //[Route("DoctorBoardCertification/{id}")]
        //public ActionResult DoctorBoardCertification(string id)
        //{
        //    Session["DoctorSearch"] = id;
        //    DoctorBoardCertificationViewModel doc = new DoctorBoardCertificationViewModel()
        //    {
        //        DoctorId = Convert.ToInt32(id)
        //    };
        //    return View(doc);
        //}

        //[Route("GetDoctorBoardCertificationList")]
        //public ActionResult GetDoctorBoardCertificationList(JQueryDataTableParamModel param)
        //{
        //    var id = Convert.ToInt32(Session["DoctorSearch"]);
        //    Dictionary<int, string> SortColumns = new Dictionary<int, string>();
        //    SortColumns.Add(0, "Name");
        //    SortColumns.Add(1, "Description");
        //    SortColumns.Add(2, "IsActive");

        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
        //    Pagination objPagination = new Pagination()
        //    {
        //        StartIndex = param.iDisplayStart,
        //        PageSize = param.iDisplayLength,
        //        SortColumnName = SortColumns[sortColumnIndex],
        //        SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
        //    };

        //    objPagination.Search = param.sSearch ?? "";

        //    var allBoardCertification = _doctor.GetDoctorBoardCertifications(objPagination, id).Where(x => !x.IsDeleted && x.DoctorId == id).Select(x => new DoctorBoardCertificationViewModel()
        //    {
        //        BoardCertificationId = x.BoardCertificationId,
        //        Name = x.BoardCertificationName,
        //        Description = x.Description,
        //        IsActive = x.IsActive,
        //        DoctorName = x.DoctorName,
        //        Id = x.DoctorBoardCertificationId,
        //        TotalRows = x.TotalRows
        //    }).ToList();


        //    var total = allBoardCertification.Count > 0 ? allBoardCertification.FirstOrDefault().TotalRows : 0;

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = allBoardCertification
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost, Route("RemoveDoctorBoardCertification/{id?}")]
        //public JsonResult RemoveDoctorBoardCertification(int id)
        //{
        //    int count = _repo.ExecuteSQLQuery("spDoctorBoardCertification @Activity ,@Id  ",
        //                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Delete" },
        //                  new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
        //                 ;
        //    // var BoardCertification = _repo.All<DoctorBoardCertification>().ToList().Where(x=>x.DoctorBoardCertificationId==id).FirstOrDefault();
        //    //BoardCertification.IsDeleted = true;
        //    //_repo.Update<DoctorBoardCertification>(BoardCertification,true);

        //    return Json(new JsonResponse() { Status = 1, Message = " Board Certification remove successfully" });
        //}

        //#endregion
        #region MyProfile
        [HttpGet, Route("BasicInformation/{id?}")]
        public ActionResult BasicInformation(int id = 0)
        {
            if (User.IsInRole("SeniorCare"))
            {
                return RedirectToAction("SeniorCare", "Dashboard");
            }
            ViewBag.SpecialityList = _speciality.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.SpecialityName,
                Value = x.SpecialityId.ToString()
            }).ToList();
            ViewBag.StateList = null;
            //ViewBag.StateList = _cityStateZip.GetAll().Take(5).Select(x => new SelectListItem
            //{
            //    Text = x.City,
            //    Value = x.CityStateZipCodeID.ToString()
            //}).ToList();

            ViewBag.AgegroupList = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.AgeGroupId.ToString()
            }).ToList();

            //ViewBag.InsuranceAcceptedList = _doctorInsuranceAccepted.GetAll(x => x.IsActive && !x.IsDeleted && x.IsEnable).Select(x => new SelectListItem
            //{
            //    Text = x.Name,
            //    Value = x.InsuranceAcceptedId.ToString()
            //}).ToList();
            if (User.IsInRole(UserRoles.Admin))
                _doctorId = id;
            else
                _doctorId = GetDoctorId();
            var doctor = _doctor.GetById(_doctorId);
            DoctorBasicInformation result = new DoctorBasicInformation();
            //   var result = AutoMapper.Mapper.Map<DoctorBasicInformation>(doctor);
            //   result.Prefix = doctor.DoctorUser?.Prefix ?? "";
            //   result.FirstName = doctor.DoctorUser?.FirstName ?? "";
            //   result.MiddleName = doctor.DoctorUser?.MiddleName ?? "";
            //   result.LastName = doctor.DoctorUser?.LastName ?? "";
            //   result.Suffix = doctor.DoctorUser?.Suffix ?? "";
            //   result.Gender = doctor.DoctorUser?.Gender ?? "";
            //   result.PhoneNumber = doctor.DoctorUser?.PhoneNumber;
            //   result.FaxNumber = doctor.DoctorUser?.FaxNumber;
            //   result.ProfilePicture = doctor.DoctorUser?.ProfilePicture ?? "";
            //   result.ShortDescription = doctor.ShortDescription ?? "";
            //   result.LongDescription = doctor.LongDescription ?? "";
            //   result.Education = doctor.Education ?? "";
            //   result.Npi = doctor.NPI;
            //   result.Speciality = doctor.DoctorSpecialities.Where(x => x.IsActive && !x.IsDeleted).Select(x => x.SpecialityId).ToList();
            // //  result.IssuranceAccepted = doctor..Where(x => x.IsActive && !x.IsDeleted).Select(x => x.InsuranceAcceptedId).ToList();
            //   result.AgeGroup =  doctor.DoctorAgeGroups.Where(x => x.IsActive && !x.IsDeleted).Select(x => x.AgeGroupId).ToList();
            ////   result.AddressView = AutoMapper.Mapper.Map<AddressViewModel>(doctor.Address.FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.IsDefault));
            //   result.SocialMediaViewModel = AutoMapper.Mapper.Map<SocialMediaViewModel>(doctor.SocialMediaLinks.FirstOrDefault());


            return View(result);
        }

        [HttpPost, Route("BasicInformation")]
        public JsonResult BasicInformation(DoctorBasicInformation model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //int userId = User.Identity.GetUserId<int>();
                    var doctor = _doctor.GetById(model.DoctorId);

                    #region Doctor Basic
                    doctor.DoctorUser.Prefix = model.Prefix;
                    doctor.DoctorUser.FirstName = model.FirstName ?? "";
                    doctor.DoctorUser.MiddleName = model.MiddleName ?? "";
                    doctor.DoctorUser.LastName = model.LastName ?? "";
                    doctor.DoctorUser.Suffix = model.Suffix ?? "";
                    doctor.DoctorUser.Gender = model.Gender ?? "";
                    doctor.DoctorUser.PhoneNumber = model.PhoneNumber;
                    doctor.DoctorUser.FaxNumber = model.FaxNumber;
                    doctor.DoctorUser.Uniquekey = Convert.ToString(model.Npi);
                    doctor.NPI = model.Npi;
                    doctor.IsAllowNewPatient = model.IsAllowNewPatient;

                    doctor.DoctorUser.ProfilePicture = model.ProfilePicture;

                    doctor.IsNtPcp = model.IsNtPcp;
                    doctor.Education = model.Education;
                    doctor.ShortDescription = model.ShortDescription;
                    doctor.LongDescription = model.LongDescription;
                    doctor.IsPrimaryCare = model.IsPrimaryCare;
                    _doctor.UpdateData(doctor);
                    _doctor.SaveData();
                    #endregion

                    #region Doctor Address

                    var address = AutoMapper.Mapper.Map<AddressViewModel, Address>(model.AddressView);
                    address.AddressId = model.AddressView.AddressId;
                    if (address.AddressId == 0)
                    {
                        address.ReferenceId = model.DoctorId;
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

                    #region Specialitys
                    var specialitys = doctor?.DoctorSpecialities.Where(x => x.IsActive && !x.IsDeleted).Select(x => x.SpecialityId).ToList();

                    var addSelectedList = model.Speciality?.Except(specialitys) ?? new List<short>();
                    foreach (var item in addSelectedList)
                    {
                        if (item != 0)
                        {
                            var addSpecialitys = new DoctorSpeciality()
                            {
                                DoctorId = model.DoctorId,
                                SpecialityId = item,
                                IsActive = true
                            };
                            _doctorSpeciality.InsertData(addSpecialitys);
                        }
                    }
                    var removeSelectedList = model.Speciality?.Except(model.Speciality) ?? new List<short>();
                    foreach (var item in removeSelectedList)
                    {
                        var single = _doctorSpeciality.GetSingle(x => x.IsActive && !x.IsDeleted && x.SpecialityId == item && x.DoctorId == model.DoctorId);
                        if (single == null) continue;
                        single.UpdatedDate = DateTime.UtcNow;
                        single.IsDeleted = true;
                        single.IsActive = false;
                        _doctorSpeciality.UpdateData(single);
                    }
                    _doctorSpeciality.SaveData();
                    #endregion

                    #region Insurance
                    //var Insurancesdata = doctor?.DoctorInsurances.Where(x => x.IsActive && !x.IsDeleted).Select(x => x.InsuranceAcceptedId).ToList();

                    //var addSelectedListforInsurance = model.IssuranceAccepted?.Except(Insurancesdata) ?? new List<int>();
                    //foreach (var item in addSelectedListforInsurance)
                    //{
                    //    if (item != 0)
                    //    {
                    //        var addInsurances = new DoctorInsurance()
                    //        {
                    //            DoctorId = model.DoctorId,

                    //            InsuranceAcceptedId = item,
                    //            IsActive = true
                    //        };
                    //        _doctorInsurance.InsertData(addInsurances);
                    //    }
                    //}
                    //var removeSelectedListforInsurance = model.IssuranceAccepted?.Except(model.IssuranceAccepted) ?? new List<int>();
                    //foreach (var item in removeSelectedListforInsurance)
                    //{
                    //    var single = _doctorInsurance.GetSingle(x => x.IsActive && !x.IsDeleted && x.InsuranceAcceptedId == item && x.DoctorId == model.DoctorId);
                    //    if (single == null) continue;
                    //    single.UpdatedDate = DateTime.UtcNow;
                    //    single.IsDeleted = true;
                    //    single.IsActive = false;
                    //    _doctorInsurance.UpdateData(single);
                    //}
                    //_doctorInsurance.SaveData();
                    #endregion

                    #region Age Group
                    var agegroupdata = doctor?.DoctorAgeGroups.Where(x => x.IsActive && !x.IsDeleted).Select(x => x.AgeGroupId).ToList();

                    var addSelectedListforagegroup = model.AgeGroup?.Except(agegroupdata) ?? new List<int>();
                    foreach (var item in addSelectedListforagegroup)
                    {
                        if (item != 0)
                        {
                            var addDoctorAgeGroup = new DoctorAgeGroup()
                            {
                                DoctorId = model.DoctorId,

                                AgeGroupId = item,
                                IsActive = true
                            };
                            _doctoragegroup.InsertData(addDoctorAgeGroup);
                        }
                    }
                    var removeSelectedListforagegroup = model.AgeGroup?.Except(model.AgeGroup) ?? new List<int>();
                    foreach (var item in removeSelectedListforagegroup)
                    {
                        var single = _doctoragegroup.GetSingle(x => x.IsActive && !x.IsDeleted && x.AgeGroupId == item && x.DoctorId == model.DoctorId);
                        if (single == null) continue;
                        single.UpdatedDate = DateTime.UtcNow;
                        single.IsDeleted = true;
                        single.IsActive = false;
                        _doctoragegroup.UpdateData(single);
                    }
                    _doctoragegroup.SaveData();
                    #endregion

                    #region Doctor Social Media
                    var socialMedia = AutoMapper.Mapper.Map<SocialMediaViewModel, SocialMedia>(model.SocialMediaViewModel);
                    socialMedia.SocialMediaId = model.SocialMediaViewModel.SocialMediaId;
                    if (model.SocialMediaViewModel.SocialMediaId == 0)
                    {
                        socialMedia.ReferenceId = model.DoctorId;
                        _socialMedia.InsertData(socialMedia);
                        _socialMedia.SaveData();
                    }
                    else
                    {
                        var editSocialMedia = _socialMedia.GetById(socialMedia.SocialMediaId);
                        if (editSocialMedia != null)
                        {
                            editSocialMedia.Facebook = socialMedia.Facebook;
                            editSocialMedia.Twitter = socialMedia.Twitter;
                            editSocialMedia.LinkedIn = socialMedia.LinkedIn;
                            editSocialMedia.Instagram = socialMedia.Instagram;
                            _socialMedia.UpdateData(editSocialMedia);
                            _socialMedia.SaveData();
                        }
                    }
                    #endregion

                    txscope.Complete();
                    return Json(new JsonResponse { Status = 1, Message = "Doctor basic information updated." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "BasicInformation-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult UploadProfile(int DoctorId, HttpPostedFileBase profilePic)
        {
            using (var txscopr = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (DoctorId > 0 && profilePic != null)
                    {
                        _doctorId = DoctorId;
                        var doctor = _doctor.GetById(_doctorId);
                        var oldPic = doctor.DoctorUser.ProfilePicture;
                        string extension = Path.GetExtension(profilePic.FileName);
                        string profilePicture = $@"Doctor-{DateTime.Now.Ticks}{extension}";
                        filePath = Path.Combine(Server.MapPath(FilePathList.ProfilePic), profilePicture);
                        Common.CheckServerPath(Server.MapPath(FilePathList.ProfilePic));
                        profilePic.SaveAs(filePath);
                        doctor.DoctorUser.ProfilePicture = $@"{FilePathList.ProfilePic}{profilePicture}";
                        User.Identity.AddUpdateClaim(UserClaims.ProfilePicture, doctor.DoctorUser.ProfilePicture);

                        _doctor.UpdateData(doctor);
                        _doctor.SaveData();
                        txscopr.Complete();
                        if (oldPic != StaticFilePath.ProfilePicture)
                            Common.DeleteFile(oldPic);
                        return Json(new JsonResponse { Status = 1, Message = "Profile Picture is updated.", Data = doctor.DoctorUser.ProfilePicture }, JsonRequestBehavior.AllowGet);
                    }
                    txscopr.Dispose();
                    return Json(new JsonResponse { Status = 0, Message = "Profile Picture is not updated." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(filePath))
                        Common.DeleteFile(filePath, true);
                    txscopr.Dispose();
                    Common.LogError(ex, "UploadProfile-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Profile Picture is not updated." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public JsonResult IsNPiExist(string npi)
        {
            var result = _doctor.GetSingle(x => x.NPI == npi) == null;

            return Json(new JsonResponse { Status = (result ? 1 : 0), Message = "Npi already exists in database." }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region My Images
        [HttpGet, Route("MyImages/{id}")]
        public ActionResult MyImages(int? id)
        {
            if (User.Identity.GetClaimValue("UserRole") == "Doctor")
            {
                _doctorId = GetDoctorId();

                Session["DoctorSearch"] = _doctorId;


            }
            else
            {
                Session["DoctorSearch"] = id;
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }

            return View();
        }

        public ActionResult GetMyImagesList(JQueryDataTableParamModel param)
        {
            if (User.Identity.GetClaimValue("UserRole") == "Doctor")
            {
                _doctorId = GetDoctorId();

            }
            else
            {
                _doctorId = Convert.ToInt32(Session["DoctorSearch"]);
            }

            var allImages = _doctorImage.GetAll(x => !x.IsDeleted && x.ReferenceId == _doctorId).Select(x => new DoctorImage()
            {
                Id = x.SiteImageId,
                ImagePath = FilePathList.Doctor + x.ImagePath,
                Name = x.Name,
                IsProfile = x.IsProfile
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allImages = allImages.Where(x => x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                              || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DoctorImage, string> orderingFunction =
                c => sortColumnIndex == 2 ? c.CreatedDate
                    : sortColumnIndex == 3 ? c.UpdatedDate
                            : c.CreatedDate;
            allImages = sortDirection == "asc" ? allImages.OrderBy(orderingFunction).ToList() : allImages.OrderByDescending(orderingFunction).ToList();


            var display = allImages.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allImages.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_MyImages/{id?}")]
        public PartialViewResult MyImages(int id)
        {
            _doctorId = GetDoctorId();
            if (id == 0) return PartialView(@"Partial/_MyImages", new DoctorImage() { DoctorId = _doctorId });
            var doctorImage = _doctorImage.GetById(id);
            var result = AutoMapper.Mapper.Map<DoctorImage>(doctorImage);
            if (result == null) return PartialView(@"Partial/_MyImages", new DoctorImage());
            return PartialView(@"Partial/_MyImages", result);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MyImagesSave(DoctorImage model)
        {
            var filePath = new List<string>();
            model.DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var doctorImages = new List<SiteImage>();
                    foreach (var item in model.ImagePath.TrimEnd(',').Split(','))
                    {
                        string extension = Path.GetExtension(item);

                        doctorImages.Add(new SiteImage()
                        {
                            ReferenceId = model.DoctorId,
                            ImagePath = item,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now,
                            CreatedBy = User.Identity.GetUserId<int>(),
                            IsProfile = model.IsProfile,
                            Name = model.Name
                        });
                    }
                    _doctorImage.InsertData(doctorImages);
                    _doctorImage.SaveData();
                    txscope.Complete();
                    return Json(new JsonResponse() { Status = 1, Message = "Image upload successfully" });
                }
                catch (Exception ex)
                {
                    if (filePath.Any())
                    {
                        filePath.ForEach(x => Common.DeleteFile(x, true));
                    }
                    txscope.Dispose();
                    Common.LogError(ex, "MyImages-Post");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
                }
            }
        }
        [HttpPost]
        public JsonResult MyImages(DoctorImage model)
        {
            var filePath = new List<string>();
            model.DoctorId = Convert.ToInt32(Session["DoctorSearch"]);

            try
            {
                List<string> list = new List<string>();
                var doctorImages = new List<SiteImage>();
                foreach (var item in model.Image)
                {
                    string extension = Path.GetExtension(item.FileName);
                    model.ImagePath = $@"Doctor-{DateTime.Now.Ticks}{extension}";
                    string singleFile = Path.Combine(Server.MapPath(FilePathList.Doctor), model.ImagePath);
                    filePath.Add(singleFile);
                    Common.CheckServerPath(Server.MapPath(FilePathList.Doctor));

                    item.SaveAs(singleFile);
                    list.Add(model.ImagePath);
                    doctorImages.Add(new SiteImage()
                    {
                        ReferenceId = model.DoctorId,
                        ImagePath = model.ImagePath,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.GetUserId<int>(),
                        IsProfile = model.IsProfile,
                        Name = model.Name
                    });
                }
                // _doctorImage.InsertData(doctorImages);
                //_doctorImage.SaveData();
                //txscope.Complete();
                string images = string.Join(",", list);
                return Json(new JsonResponse() { Status = 1, Message = images });
            }
            catch (Exception ex)
            {
                if (filePath.Any())
                {
                    filePath.ForEach(x => Common.DeleteFile(x, true));
                }

                Common.LogError(ex, "MyImages-Post");
                return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
            }

        }

        [HttpPost, Route("RemoveMyImage/{id?}")]
        public JsonResult RemoveMyImage(int id)
        {
            var doctorImage = _doctorImage.GetById(id);
            Common.DeleteFile(doctorImage.ImagePath);
            _doctorImage.DeleteData(doctorImage);
            _doctorImage.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Image deleted successfully" });
        }
        #endregion

        #region Experience
        [HttpPost, Route("ActiveDeExperience/{flag}/{id}")]
        public JsonResult ActiveDeExperience(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorExperience @Activity ,@Id,@IsActive  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Activate" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                          new SqlParameter("IsActive", System.Data.SqlDbType.Int) { Value = !flag })
                         ;

            return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor Experience has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        [Route("DoctorExperience/{id}")]
        public ActionResult DoctorExperience(string id)
        {
            Session["DoctorSearch"] = id;
            if (User.IsInRole("Admin"))
            {
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            return View();
        }
        [Route("GetDoctorExperienceList")]
        public ActionResult GetDoctorExperienceList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            Dictionary<int, string> SortColumns = new Dictionary<int, string>();
            SortColumns.Add(0, "Designation");
            SortColumns.Add(1, "Organization");
            SortColumns.Add(2, "StartDate");
            SortColumns.Add(3, "EndDate");
            SortColumns.Add(4, "IsActive");
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            Pagination objPagination = new Pagination()
            {
                StartIndex = param.iDisplayStart,
                PageSize = param.iDisplayLength,
                SortColumnName = SortColumns[sortColumnIndex],
                SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
            };

            objPagination.Search = param.sSearch ?? "";

            var list = _doctor.GetDoctorExperiences(objPagination, id).Where(x => !x.IsDeleted && x.DoctorId == id).ToList();



            var total = list.Count > 0 ? list.FirstOrDefault().TotalRows : 0;


            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = list
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, Route("AddEditDoctorExperience")]
        public JsonResult AddEditDoctorExperience(ExperienceViewModel model)
        {
            model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spExperience " +
                        "@Activity," +
                        "@Id," +
                        "@DoctorId," +
                        "@Designation," +
                        "@Organization," +
                        "@StartDate," +
                        "@EndDate," +
                        "@IsActive," +
                        "@CreatedBy"
                         , new SqlParameter("Activity", System.Data.SqlDbType.VarChar) { Value = "Insert" },
                           new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = model.ExperienceId },
                            new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                      new SqlParameter("Designation", System.Data.SqlDbType.VarChar) { Value = model.Designation },
                                      new SqlParameter("Organization", System.Data.SqlDbType.VarChar) { Value = (object)model.Organization ?? DBNull.Value },
                                      new SqlParameter("StartDate", System.Data.SqlDbType.DateTime) { Value = model.StartDate },
                                      new SqlParameter("EndDate", System.Data.SqlDbType.DateTime) { Value = model.EndDate },
                                       new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },


                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() }
                       );
            return Json(new JsonResponse() { Status = 1, Message = "Experience save successfully" });
        }
        [HttpGet, Route("AddEditDoctorExperience/{id?}")]
        public ActionResult AddEditDoctorExperience(int? id)
        {
            //int docBoardCertificationid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorExperienceViewModel>("spExperience @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            // var result = _repo.All<DoctorBoardCertification>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();

            if (id != 0 && id != null)
            {
                var Experience = result;

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Experience.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;

                Experience.DoctorId = Convert.ToInt32(DoctorId);
                return View(Experience);
            }
            else
            {
                var Experience = new DoctorExperienceViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Experience.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;

                Experience.DoctorId = Convert.ToInt32(DoctorId);
                return View(Experience);
            }

        }
        [HttpGet, Route("AddEditViewDoctorExperience/{id?}")]
        public ActionResult AddEditViewDoctorExperience(int? id)
        {
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorExperienceViewModel>("spExperience @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();

            if (id != 0 && id != null)
            {



                var DoctorId = Convert.ToString(Session["DoctorSearch"]);

                result.IsViewMode = true;
                // result.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorExperience", result);
            }
            else
            {
                var Experience = new DoctorExperienceViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Experience.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Experience.IsViewMode = true;

                Experience.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorExperience", Experience);
            }

        }
        [HttpPost, Route("RemoveDoctorExperience/{id?}")]
        public JsonResult RemoveDoctorExperience(int id)
        {
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spExperience " +
                            "@Activity," +
                            "@Id"
                        ,

                                    new SqlParameter("Activity", System.Data.SqlDbType.VarChar) { Value = "Delete" },
                                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id }

                     );
            return Json(new JsonResponse() { Status = 1, Message = "Experience deleted successfully" });
        }
        #endregion
        #region DoctorAccount
        [HttpGet, Route("UserAccount/{id?}")]
        public ActionResult UserAccount(int? id)
        {
            if (User.Identity.GetClaimValue("UserRole") == "Doctor")
            {
                _doctorId = GetDoctorId();

                Session["DoctorSearch"] = _doctorId;

            }
            else
            {
                Session["DoctorSearch"] = id;
            }

            return View();

        }
        [HttpGet, Route("DoctorAccount/{id?}")]
        public PartialViewResult DoctorAccount(int? id)
        {
            ViewBag.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgAccount = _repo.ExecWithStoreProcedure<RegisterJsonModel>("sprGetUserAccountDetailById @Id",
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId() }
                    ).FirstOrDefault();

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(orgAccount.RegisterViewModels);



                return PartialView(@"/Views/Doctor/Partial/_DoctorAccount.cshtml", obj);
            }
            else
            {
                ViewBag.ID = ViewBag.PharmacyID;

                var orgAccount = _repo.ExecWithStoreProcedure<RegisterJsonModel>("sprGetUserAccountDetailById @Id",
                      new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId() }
                      ).FirstOrDefault();
                var obj = new RegisterViewModel();
                if (orgAccount != null)
                {

                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(orgAccount.RegisterViewModels);

                }

                return PartialView(@"/Views/Doctor/Partial/_DoctorAccount.cshtml", obj);
            }
        }
        #endregion

        [HttpPost, Route("AddEditDoctorAccount"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorAccount(RegisterViewModel model)
        {
            ViewBag.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);
            if (model.ConfirmPassword != model.Password)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Password and Confirm Password didn't Matched." });
            }
            try
            {
                var code = _userManager.GeneratePasswordResetToken(User.Identity.GetUserId<int>());
                var result = _userManager.ResetPassword(User.Identity.GetUserId<int>(), code, model.Password);
                model.UserType = "3";
                if (ViewBag.DoctorID > 0)
                {
                    var user = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        MiddleName = model.MiddleName,
                        LastName = model.LastName,
                        UserName = model.Email,
                        Email = model.Email,
                        LastResetPassword = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                        Uniquekey = model.Uniquekey,
                        UserTypeId = 2,
                        RegisterViewModel = Newtonsoft.Json.JsonConvert.SerializeObject(model)
                    };

                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spUpdateUserAccount " +
                           "@FirstName," +
                           "@MiddleName," +
                           "@LastName," +
                           "@RegisterViewModel," +
                           "@Email," +
                           "@LastResetPassword," +
                           "@Id," +
                           "@Uniquekey",

                                         new SqlParameter("FirstName", System.Data.SqlDbType.VarChar) { Value = model.FirstName == null ? "" : model.FirstName },
                                         new SqlParameter("MiddleName", System.Data.SqlDbType.VarChar) { Value = model.MiddleName == null ? "" : model.MiddleName },
                                         new SqlParameter("LastName", System.Data.SqlDbType.VarChar) { Value = model.LastName == null ? "" : model.LastName },
                                         new SqlParameter("RegisterViewModel", System.Data.SqlDbType.VarChar) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(model) },
                                         new SqlParameter("Email", System.Data.SqlDbType.VarChar) { Value = model.Email },
                                         new SqlParameter("LastResetPassword", System.Data.SqlDbType.DateTime) { Value = DateTime.UtcNow },
                                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                           new SqlParameter("Uniquekey", System.Data.SqlDbType.VarChar) { Value = model.Uniquekey }



                                     );

                    return Json(new JsonResponse() { Status = 1, Message = "Account Info saved successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Doctor Info Not Found! Should be select Doctor Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Account info saving Error.." });
            }

        }
        #region Qualification
        [HttpPost, Route("ActiveDeQualification/{flag}/{id}")]
        public JsonResult ActiveDeQualification(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorQualification @Activity ,@Id,@IsActive  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Activate" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                          new SqlParameter("IsActive", System.Data.SqlDbType.Int) { Value = !flag })
                         ;

            return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor Qualification has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }


        [Route("DoctorQualification/{id}")]
        public ActionResult DoctorQualification(string id)
        {
            Session["DoctorSearch"] = id;
            if (User.IsInRole("Admin"))
            {
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            return View();
        }
        [Route("GetDoctorQualificationList")]
        public ActionResult GetDoctorQualificationList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            Dictionary<int, string> SortColumns = new Dictionary<int, string>();
            SortColumns.Add(0, "Institute");
            SortColumns.Add(1, "Degree");
            SortColumns.Add(2, "PassingYear");
            SortColumns.Add(3, "Notes");
            SortColumns.Add(4, "IsActive");
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            Pagination objPagination = new Pagination()
            {
                StartIndex = param.iDisplayStart,
                PageSize = param.iDisplayLength,
                SortColumnName = SortColumns[sortColumnIndex],
                SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
            };

            objPagination.Search = param.sSearch ?? "";

            var allQualification = _doctor.GetDoctorQualifications(objPagination, id).Where(x => !x.IsDeleted && x.DoctorId == id).Select(x => new DoctorQualificationViewModel()
            {
                QualificationId = x.QualificationId,
                Institute = x.Institute,
                Degree = x.Degree,
                PassingYear = x.PassingYear,
                DoctorName = x.DoctorName,
                DoctorId = x.DoctorId,
                TotalRows = x.TotalRows,
                IsActive = x.IsActive,
                Notes = x.Notes
            }).ToList();


            var total = allQualification.Count > 0 ? allQualification.FirstOrDefault().TotalRows : 0;


            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allQualification
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_Qualification/{id?}")]
        public PartialViewResult Qualification(int id)
        {
            if (User.IsInRole(UserRoles.Admin))
                _doctorId = id;
            else
                _doctorId = GetDoctorId();
            if (id == 0) return PartialView(@"Partial/_Qualification", new QualificationViewModel() { DoctorId = _doctorId });
            var qualification = _qualification.GetById(id);
            var result = AutoMapper.Mapper.Map<QualificationViewModel>(qualification);
            if (result == null) return PartialView(@"Partial/_Qualification", new QualificationViewModel());


            return PartialView(@"Partial/_Qualification", result);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("AddEditDoctorQualification")]
        public JsonResult AddEditDoctorQualification(DoctorQualificationViewModel model)
        {
            model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spQualification " +
                        "@Institute," +
                        "@Degree," +
                        "@PassingYear," +
                        "@Id," +
                        "@CreatedBy," +
                        "@Notes," +
                        "@Activity," +
                         "@DoctorId," +
                         "@IsActive",
                                      new SqlParameter("Institute", System.Data.SqlDbType.VarChar) { Value = model.Institute },
                                      new SqlParameter("Degree", System.Data.SqlDbType.VarChar) { Value = (object)model.Degree ?? DBNull.Value },
                                      new SqlParameter("PassingYear", System.Data.SqlDbType.SmallInt) { Value = model.PassingYear },
                                      new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = model.QualificationId },
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                      new SqlParameter("Notes", System.Data.SqlDbType.VarChar) { Value = model.Notes },
                                      new SqlParameter("Activity", System.Data.SqlDbType.VarChar) { Value = "Insert" },
                                      new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.Int) { Value = model.IsActive }


                       );
            return Json(new JsonResponse() { Status = 1, Message = "Qualification save successfully" });
        }
        [HttpGet, Route("AddEditDoctorQualification/{id?}")]
        public ActionResult AddEditDoctorQualification(int? id)
        {
            //int docBoardCertificationid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorQualificationViewModel>("spQualificationById @Id  ",

                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            // var result = _repo.All<DoctorBoardCertification>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();

            if (id != 0 && id != null)
            {
                var Quallification = result;

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Quallification.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;

                Quallification.DoctorId = Convert.ToInt32(DoctorId);
                return View(Quallification);
            }
            else
            {
                var Quallification = new DoctorQualificationViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Quallification.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;

                Quallification.DoctorId = Convert.ToInt32(DoctorId);
                return View(Quallification);
            }

        }
        [HttpGet, Route("AddEditViewDoctorQualification/{id?}")]
        public ActionResult AddEditViewDoctorQualification(int? id)
        {
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorQualificationViewModel>("spQualificationById @Id  ",

                   new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                   .FirstOrDefault();

            if (id != 0 && id != null)
            {



                var DoctorId = Convert.ToString(Session["DoctorSearch"]);
                result.IsViewMode = true;
                //   Qualification.IsViewMode = true;
                // result.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorQualification", result);
            }
            else
            {
                var Qualification = new DoctorQualificationViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Qualification.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Qualification.IsViewMode = true;

                Qualification.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorQualification", Qualification);
            }

        }
        [HttpPost, Route("RemoveDoctorQualification/{id?}")]
        public JsonResult RemoveDoctorQualification(int id)
        {
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spQualification_Delete " +
                     "@Id"
                        ,
                             new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id }
                        );
            return Json(new JsonResponse() { Status = 1, Message = "Qualification deleted successfully" });
        }
        #endregion
        #region DocOrgInsurances
        [Route("DoctorInsurances/{id}")]
        public ActionResult DoctorInsurances(string id)
        {
            Session["DoctorSearch"] = id;
            if (User.IsInRole("Admin"))
            {
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            return View();
        }
        [Route("GetDoctorInsurancesList")]
        public ActionResult GetDoctorInsurancesList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            Dictionary<int, string> SortColumns = new Dictionary<int, string>();
            SortColumns.Add(0, "InsurancePlanName");
            SortColumns.Add(1, "ReferenceName");

            SortColumns.Add(2, "IsActive");
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            Pagination objPagination = new Pagination()
            {
                StartIndex = param.iDisplayStart,
                PageSize = param.iDisplayLength,
                SortColumnName = SortColumns[sortColumnIndex],
                SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
            };

            objPagination.Search = param.sSearch ?? "";

            var allInsurances = _doctor.GetDoctorInsurancess(objPagination, id).Where(x => !x.IsDeleted).Select(x => new DoctorInsurancesViewModel()
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


            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allInsurances
            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateAntiForgeryToken, Route("AddEditDoctorInsurances")]
        public JsonResult AddEditDoctorInsurances(DoctorInsurancesViewModel model)
        {
            model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
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
                                      new SqlParameter("InsuranceIdentifierId", System.Data.SqlDbType.VarChar) { Value = model.InsuranceIdentifierId == null ? "" : model.InsuranceIdentifierId },
                                      new SqlParameter("StateId", System.Data.SqlDbType.VarChar) { Value = model.StateId },
                                        new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                           new SqlParameter("CreatedBy", System.Data.SqlDbType.Int)
                                           {
                                               Value = User.Identity.GetUserId<int>()
                                           },
                                       new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 }


                       );
            return Json(new JsonResponse() { Status = 1, Message = "Insurances save successfully" });
        }
        [HttpGet, Route("AddEditDoctorInsurances/{id?}")]
        public ActionResult AddEditDoctorInsurances(int? id)
        {
            //int docBoardCertificationid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorInsurancesViewModel>("spDocOrgInsurances @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            // var result = _repo.All<DoctorBoardCertification>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();
            var ReferenceList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                   new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "ReferenceList" }
               ).ToList();
            var StateList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "StateList" }
            ).ToList();
            if (id != 0 && id != null)
            {
                var Insurances = result;
                var InsurancesPlanList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "InsurancePlanList" }
                ).ToList();

                Insurances.InsurancesPlanList = InsurancesPlanList;
                Insurances.ReferencesList = ReferenceList;
                Insurances.StateList = StateList;
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Insurances.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;

                Insurances.DoctorId = Convert.ToInt32(DoctorId);
                return View(Insurances);
            }
            else
            {
                var Insurances = new DoctorInsurancesViewModel();

                var InsurancesPlanList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "InsurancePlanList" }
                ).ToList();
                Insurances.InsurancesPlanList = InsurancesPlanList;
                Insurances.ReferencesList = ReferenceList;
                Insurances.StateList = StateList;

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Insurances.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;

                Insurances.DoctorId = Convert.ToInt32(DoctorId);
                return View(Insurances);
            }

        }
        [HttpGet, Route("AddEditViewDoctorInsurances/{id?}")]
        public ActionResult AddEditViewDoctorInsurances(int? id)
        {
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorInsurancesViewModel>("sprDocOrgInsuranceById @Id  ",
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            var ReferenceList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "ReferenceList" }
            ).ToList();
            var StateList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "StateList" }
            ).ToList();
            if (id != 0 && id != null)
            {



                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);

                var InsurancesPlanList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "InsurancePlanList" }
              ).ToList();
                result.InsurancesPlanList = InsurancesPlanList;
                result.ReferencesList = ReferenceList;
                result.StateList = StateList;
                result.IsViewMode = true;
                // result.DoctorId = Convert.ToInt32(DoctorId);
                DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                result.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;

                result.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorInsurances", result);
            }
            else
            {
                var Insurances = new DoctorInsurancesViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Insurances.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Insurances.IsViewMode = true;
                var InsurancesPlanList = _repo.ExecWithStoreProcedure<DropDownModel>("spDocOrgInsurances @Activity  ",
                    new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "InsurancePlanList" }
               ).ToList();
                Insurances.InsurancesPlanList = InsurancesPlanList;

                Insurances.ReferencesList = ReferenceList;
                Insurances.StateList = StateList;
                Insurances.DoctorId = Convert.ToInt32(DoctorId);
                DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Insurances.DoctorName = doctors == null ? "" : doctors.FirstName + " " + doctors.LastName;

                Insurances.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorInsurances", Insurances);
            }

        }
        [HttpPost, Route("RemoveDoctorInsurances/{id?}")]
        public JsonResult RemoveDoctorInsurances(int id)
        {
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgInsurances " +
                  "@Activity," +
                         "@Id"


                           ,

                                       new SqlParameter("Activity", System.Data.SqlDbType.VarChar) { Value = "Delete" },
                                       new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id }

                        );
            return Json(new JsonResponse() { Status = 1, Message = "Insurances deleted successfully" });
        }
        #endregion


        #region Slot
        [Route("DoctorSlot/{id?}")]
        public ActionResult DoctorSlot(int? id)
        {
            Session["DoctorSearch"] = id;
            return View();
        }

        public ActionResult GetSlotList(JQueryDataTableParamModel param)
        {
            var sortingColumnName = Request.QueryString["mDataProp_" + param.iSortCol_0];
            if (sortingColumnName.Contains("StSlotDate"))
            {
                sortingColumnName = sortingColumnName.Replace("StSlotDate", "SlotDate");
            }
            if (sortingColumnName.Contains("CreatedDate") || sortingColumnName.Contains("UpdatedDate"))
            {
                sortingColumnName = sortingColumnName.Replace("Date", "");
            }
            _doctorId = Convert.ToInt32(Session["DoctorSearch"]); //GetDoctorId();

            var searchRecords = new SqlParameter() { ParameterName = "@SearchRecords", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@DoctorId",SqlDbType.Int){Value = _doctorId},
                new SqlParameter("@iDisplayStart",SqlDbType.Int){Value = param.iDisplayStart},
                new SqlParameter("@iDisplayLength",SqlDbType.Int){Value = param.iDisplayLength},
                new SqlParameter("@SortColumn",SqlDbType.VarChar){Value = sortingColumnName},
                new SqlParameter("@SortDir",SqlDbType.VarChar){Value = param.sSortDir_0},
                new SqlParameter("@Search",SqlDbType.NVarChar){Value = param.sSearch ?? ""},
                searchRecords
            };

            var allslotList = _slot.GetSlotList(StoredProcedureList.GetSlotList, parameters.ToArray<object>());

            var totalRecords = allslotList.FirstOrDefault()?.TotalRecords ?? 0;

            allslotList.ForEach(x =>
            {
                x.StSlotDate = x.SlotDate.ToString("dd-MMM-yyyy");
                x.CreatedDate = x.Created.ToDefaultFormate();
                x.UpdatedDate = x.Updated.ToDefaultFormate();
            });

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = allslotList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_ViewSlot/{slotDate}")]
        public PartialViewResult ViewSlot(string slotDate)
        {
            _doctorId = GetDoctorId();
            DateTime toDate = Convert.ToDateTime(slotDate);
            var slot = _slot.GetAll(x => x.ReferenceId == _doctorId
            && DbFunctions.TruncateTime(Convert.ToDateTime(x.SlotDate)) == toDate.Date).ToList();

            return PartialView(@"Partial/_ViewSlot", slot);
        }

        [HttpGet, Route("_Slot/{id?}")]
        public PartialViewResult Slot(int id)
        {
            _doctorId = GetDoctorId();
            ViewBag.SlotSizeList = new List<SelectListItem>()
            {
                new SelectListItem{ Text = "5 Min", Value = "5" },
                new SelectListItem{ Text = "10 Min", Value = "10" },
                new SelectListItem{ Text = "15 Min", Value = "15" },
                new SelectListItem{ Text = "30 Min", Value = "30" },
                new SelectListItem{ Text = "45 Min", Value = "45" },
                new SelectListItem{ Text = "60 Min", Value = "60" },
            };
            if (id == 0) return PartialView(@"Partial/_Slot", new SlotViewModel() { DoctorId = _doctorId });
            var slot = _slot.GetById(id);
            var result = AutoMapper.Mapper.Map<SlotViewModel>(slot);
            if (result == null) return PartialView(@"Partial/_Slot", new SlotViewModel());


            return PartialView(@"Partial/_Slot", result);
        }

        [HttpPost, Route("AddEditSlot")]
        public JsonResult AddEditSlot(SlotViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var startDate = Convert.ToDateTime(model.StartDate);
                    var endDate = Convert.ToDateTime(model.EndDate);
                    var slotDates = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days).Select(offset => startDate.AddDays(offset)).ToArray();
                    foreach (var slotDate in slotDates)
                    {
                        var timeSlotList = new List<Slot>();
                        for (int i = model?.FromTime ?? 0; i <= model.ToTime; i += model.SlotSize)
                        {
                            var timeSpan = new TimeSpan(Convert.ToInt32(i) / 60, Convert.ToInt32(i) % 60, 00);
                            var slotDateTime = DateTime.Today.Add(timeSpan);

                            var bResult = _slot.GetAll(x => x.SlotDate == slotDate.ToString() && x.IsActive && !x.IsDeleted);
                            if (bResult == null)
                            {
                                timeSlotList.Add(new Slot { ReferenceId = model.DoctorId, SlotTime = slotDateTime.ToString(), SlotDate = slotDate.ToString(), IsActive = true });
                            }
                            else
                            {
                                var slotSingleResult = bResult.FirstOrDefault(x => x.SlotDate == slotDate.ToString() && Convert.ToDateTime(x.SlotTime).TimeOfDay == slotDateTime.TimeOfDay && x.IsActive && !x.IsDeleted);
                                if (slotSingleResult == null)
                                {
                                    timeSlotList.Add(new Slot { ReferenceId = model.DoctorId, SlotTime = slotDateTime.ToString(), SlotDate = slotDate.ToString(), IsActive = true });
                                }
                            }
                        }
                        if (timeSlotList.Any())
                        {
                            _slot.InsertData(timeSlotList);
                            _slot.SaveData();
                        }
                    }
                    txscope.Complete();
                    return Json(new JsonResponse() { Status = 1, Message = "Slot save successfully" });
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditSlot-Post");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveSlot/{date}")]
        public JsonResult RemoveSlot(string date)
        {
            DateTime toDate = Convert.ToDateTime(date);
            var slot = _slot.GetAll(x => DbFunctions.TruncateTime(Convert.ToDateTime(x.SlotDate)) == toDate.Date).ToList();
            slot.ForEach(x =>
            {
                x.IsActive = false;
                x.IsDeleted = true;
            });
            _slot.UpdateData(slot);
            _slot.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Slot deleted successfully" });
        }
        #endregion



        #region Insurance Accepted



        [HttpPost, Route("AddEditInsuranceAccepted"), ValidateAntiForgeryToken]
        public JsonResult AddEditInsuranceAccepted(InsuranceAcceptedViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.InsuranceAcceptedId == 0)
                        {
                            // var bResult = _doctorInsuranceAccepted.GetSingle(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            //var bResult = _doctorInsuranceAccepted.GetSingle(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            //if (bResult != null)
                            //{

                            //    txscope.Dispose();
                            //    return Json(new JsonResponse { Status = 0, Message = "Insurance Accepted already exists. Could not add the data!" });
                            //}

                            var Insurance = new DoctorInsuranceAccepted()
                            {
                                Name = model.Name,
                                IsEnable = model.IsEnable,
                                IsActive = true,
                                IsDeleted = false,
                            };


                            _doctorInsuranceAccepted.InsertData(Insurance);
                            _doctorInsuranceAccepted.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Insurance Accepted save successfully" });
                        }
                        else
                        {

                            var bResultdata = _doctorInsuranceAccepted.GetSingle(x => x.InsuranceAcceptedId != model.InsuranceAcceptedId && x.IsActive && !x.IsDeleted && x.Name == model.Name);
                            if (bResultdata != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Insurance Accepted already exists. Could not add the data!" });
                            }

                            var Insurance = _doctorInsuranceAccepted.GetById(model.InsuranceAcceptedId);
                            Insurance.Name = model.Name;
                            Insurance.IsEnable = model.IsEnable;
                            _doctorInsuranceAccepted.UpdateData(Insurance);
                            _doctorInsuranceAccepted.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Insurance Accepted update successfully" });
                        }
                    }
                    else
                    {
                        return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditInsuranceAccepted-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpGet, Route("_InsuranceAccepted/{id?}")]
        public PartialViewResult _InsuranceAccepted(int id)
        {

            if (id == 0) return PartialView(@"Partial/_InsuranceAccepted", new InsuranceAcceptedViewModel());
            var result = _doctorInsuranceAccepted.GetById(id);
            var Insurance = new InsuranceAcceptedViewModel()
            {
                InsuranceAcceptedId = result.InsuranceAcceptedId,
                IsEnable = result.IsEnable,
                Name = result.Name
            };
            if (result == null) return PartialView(@"Partial/_InsuranceAccepted", new InsuranceAcceptedViewModel());

            return PartialView(@"Partial/_InsuranceAccepted", Insurance);
        }

        [Route("InsuranceAccepted")]
        public ActionResult InsuranceAccepted()
        {
            return View();
        }

        [Route("GetInsuranceAcceptedList")]
        public ActionResult GetInsuranceAcceptedList(JQueryDataTableParamModel param)
        {
            try
            {
                //var data = _doctorInsurance.GetAll().ToList();
                var allDoctorInsurance = _doctorInsurance.GetAll(x => x.IsActive == true).Select(x => new DoctorInsuranceAcceptedModel()
                {
                    Id = x.InsurancePlanId,
                    Name = x.InsurancePlan == null ? "" : x.InsurancePlan.Name,
                    IsEnable = x.IsActive.Value,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
                    IsActive = x.IsActive.Value
                }).ToList();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    allDoctorInsurance = allDoctorInsurance.Where(x => x.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

                Func<DoctorInsuranceAcceptedModel, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.Name
                        : sortColumnIndex == 2 ? c.CreatedDate
                            : sortColumnIndex == 3 ? c.UpdatedDate
                                : c.Name;
                allDoctorInsurance = sortDirection == "asc" ? allDoctorInsurance.OrderBy(orderingFunction).ToList() : allDoctorInsurance.OrderByDescending(orderingFunction).ToList();

                var display = allDoctorInsurance.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var total = allDoctorInsurance.Count;

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = display
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost, Route("RemoveInsuranceAccepted/{id?}")]
        public JsonResult RemoveInsuranceAccepted(int id)
        {
            var insuranceAccepted = _doctorInsuranceAccepted.GetById(id);
            insuranceAccepted.IsDeleted = true;
            _doctorInsuranceAccepted.UpdateData(insuranceAccepted);
            _doctorInsuranceAccepted.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Insurance Accepted remove successfully" });
        }

        #endregion



        #region Facility Affiliation
        public ActionResult FacilityAffiliation()
        {
            return View();
        }


        [Route("GetFacilityAffiliationList")]
        public ActionResult GetFacilityAffiliationList(JQueryDataTableParamModel param)
        {
            var allDoctorFacilitys = _doctorFacilityAffiliation.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new DoctorAffiliationModel()
            {
                Id = x.FacilityId,
                //FacilityName = x.Facility.FacilityName,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allDoctorFacilitys = allDoctorFacilitys.Where(x => x.FacilityName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DoctorAffiliationModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.FacilityName
                    : sortColumnIndex == 2 ? c.CreatedDate
                        : sortColumnIndex == 3 ? c.UpdatedDate
                            : c.FacilityName;
            allDoctorFacilitys = sortDirection == "asc" ? allDoctorFacilitys.OrderBy(orderingFunction).ToList() : allDoctorFacilitys.OrderByDescending(orderingFunction).ToList();

            var display = allDoctorFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allDoctorFacilitys.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_FacilityAffiliation/{id?}")]
        public PartialViewResult _FacilityAffiliation(int id)
        {
            _doctorId = GetDoctorId();
            ViewBag.FacilityList = _facility.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.AuthorizedOfficialLastName,
                Value = x.DoctorAffiliations.ToString()
            }).ToList();
            if (id == 0) return PartialView(@"Partial/_FacilityAffiliation", new DoctorAffiliationModel() { DoctorId = _doctorId });
            var dfAffiliation = _doctorFacilityAffiliation.GetById(id);
            var result = AutoMapper.Mapper.Map<DoctorAffiliationModel>(dfAffiliation);
            if (result == null) return PartialView(@"Partial/_FacilityAffiliation", new DoctorAffiliationModel());

            return PartialView(@"Partial/_FacilityAffiliation", result);
        }

        [HttpPost, Route("AddEditFacilityAffiliation"), ValidateAntiForgeryToken]
        public JsonResult AddEditFacilityAffiliation(DoctorAffiliationModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.AffiliationId == 0)
                        {
                            var bResult = _doctorFacilityAffiliation.GetSingle(x => x.FacilityId == model.FacilityId && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Facility Affiliation already exists. Could not add the data!" });
                            }

                            var oHour = AutoMapper.Mapper.Map<DoctorAffiliationModel, DoctorFacilityAffiliation>(model);
                            oHour.IsActive = true;
                            _doctorFacilityAffiliation.InsertData(oHour);
                            _doctorFacilityAffiliation.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Facility Affiliation save successfully" });
                        }
                        else
                        {
                            var bResult = _doctorFacilityAffiliation.GetSingle(x => x.FacilityId != model.AffiliationId && x.IsActive && !x.IsDeleted
                                                            && x.FacilityId == model.FacilityId);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Facility Affiliation already exists. Could not add the data!" });
                            }

                            var oHour = _doctorFacilityAffiliation.GetById(model.AffiliationId);
                            oHour.FacilityId = model.FacilityId;
                            _doctorFacilityAffiliation.UpdateData(oHour);
                            _doctorFacilityAffiliation.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Facility Affiliation update successfully" });
                        }
                    }
                    else
                    {
                        return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditFacilityAffiliation-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveFacilityAffiliation/{id?}")]
        public JsonResult RemoveFacilityAffiliation(int id)
        {
            var facilityAffiliation = _doctorFacilityAffiliation.GetById(id);
            facilityAffiliation.IsDeleted = true;
            _doctorFacilityAffiliation.UpdateData(facilityAffiliation);
            _doctorFacilityAffiliation.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Facility Affiliation remove successfully" });
        }
        #endregion

        #region Featured Doctors
        [Route("FeaturedDoctors")]
        public ActionResult FeaturedDoctors()
        {
            return View();
        }

        [Route("GetFeaturedDoctorList")]
        public ActionResult GetFeaturedDoctorList(JQueryDataTableParamModel param)
        {
            var allFeaturedDoctor = _featuredDoctor.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new FeaturedDoctorViewModel()
            {
                Id = x.FeaturedDoctorId,
                DoctorName = x.Doctor.NamePrefix + " " + x.Doctor.FirstName + " " + x.Doctor.MiddleName + " " + x.Doctor.LastName,   //DoctorUser.FullForDoctor,
                ProfilePicture = FilePathList.FeaturedDoctor + x.ProfilePicture,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allFeaturedDoctor = allFeaturedDoctor.Where(x => x.DoctorName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<FeaturedDoctorViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.DoctorName
                    : sortColumnIndex == 3 ? c.CreatedDate
                        : sortColumnIndex == 4 ? c.UpdatedDate
                            : c.DoctorName;
            allFeaturedDoctor = sortDirection == "asc" ? allFeaturedDoctor.OrderBy(orderingFunction).ToList() : allFeaturedDoctor.OrderByDescending(orderingFunction).ToList();

            var display = allFeaturedDoctor.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allFeaturedDoctor.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_FeaturedDoctor/{id?}")]
        public PartialViewResult _FeaturedDoctor(int id)
        {
            ViewBag.DoctorList = _doctor.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.DoctorUser.FullForDoctor,
                Value = x.DoctorId.ToString()
            }).ToList();
            if (id == 0) return PartialView(@"Partial/_FeaturedDoctor", new FeaturedDoctorViewModel());
            var dfDoctor = _featuredDoctor.GetById(id);
            var result = AutoMapper.Mapper.Map<FeaturedDoctorViewModel>(dfDoctor);
            if (result == null) return PartialView(@"Partial/_FeaturedDoctor", new FeaturedDoctorViewModel());

            return PartialView(@"Partial/_FeaturedDoctor", result);
        }

        [HttpGet]
        public JsonResult GetDoctorList(string query = "")
        {
            var doctorList = _doctor.GetAll(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.DoctorId.ToString(),
                    Name = x.DoctorUser.FullForDoctor
                }).ToList();
            doctorList = doctorList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = doctorList });
        }

        [HttpPost, Route("AddEditFeaturedDoctor"), ValidateAntiForgeryToken]
        public JsonResult AddEditFeaturedDoctor(FeaturedDoctorViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.FeaturedDoctorId == 0)
                        {
                            var bResult = _featuredDoctor.GetSingle(x => x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Featured Doctor already exists. Could not add the data!" });
                            }
                            if (Request.Files.Count == 0)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 1, Message = "Featured Doctor save successfully" });
                            }
                            model.ProfilePic = Request.Files[0];
                            string extension = Path.GetExtension(model.ProfilePic.FileName);
                            model.ProfilePicture = $@"Featured-Doctor-{DateTime.Now.Ticks}{extension}";
                            filePath = Path.Combine(Server.MapPath(FilePathList.FeaturedDoctor), model.ProfilePicture);
                            Common.CheckServerPath(Server.MapPath(FilePathList.FeaturedDoctor));

                            model.ProfilePic.SaveAs(filePath);
                            var oHour = AutoMapper.Mapper.Map<FeaturedDoctorViewModel, FeaturedDoctor>(model);
                            oHour.IsActive = true;
                            _featuredDoctor.InsertData(oHour);
                            _featuredDoctor.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Featured Doctor save successfully" });
                        }
                        else
                        {
                            var bResult = _featuredDoctor.GetSingle(x => x.FeaturedDoctorId != model.FeaturedDoctorId && x.IsActive && !x.IsDeleted
                                                            && x.DoctorId == model.DoctorId);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Featured Doctor already exists. Could not add the data!" });
                            }

                            var oHour = _featuredDoctor.GetById(model.FeaturedDoctorId);
                            oHour.DoctorId = model.DoctorId;
                            _featuredDoctor.UpdateData(oHour);
                            _featuredDoctor.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Featured Doctor update successfully" });
                        }
                    }
                    else
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse { Status = 4, Data = ModelState.Errors() });
                    }
                }
                catch (Exception ex)
                {
                    Common.DeleteFile(filePath, true);
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditFeaturedDoctor-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveFeaturedDoctor/{id?}")]
        public JsonResult RemoveFeaturedDoctor(int id)
        {
            var featuredDoctor = _featuredDoctor.GetById(id);
            Common.DeleteFile($@"{FilePathList.FeaturedDoctor}{featuredDoctor.ProfilePicture}");
            _featuredDoctor.DeleteData(featuredDoctor);
            _featuredDoctor.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Featured Doctor remove successfully" });
        }
        #endregion

        #region Featured Specialitys
        [Route("FeaturedSpecialitys")]
        public ActionResult FeaturedSpeciality()
        {
            return View();
        }

        [Route("GetFeaturedSpecialityList")]
        public ActionResult GetFeaturedSpecialityList(JQueryDataTableParamModel param)
        {
            var allFeaturedSpeciality = _featuredSpeciality.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new FeaturedSpecialityViewModel()
            {
                Id = x.FeaturedSpecialityId,
                SpecialityName = x.Speciality.SpecialityName,
                ProfilePicture = FilePathList.FeaturedSpeciality + x.ProfilePicture,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allFeaturedSpeciality = allFeaturedSpeciality.Where(x => x.SpecialityName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<FeaturedSpecialityViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.SpecialityName
                    : sortColumnIndex == 3 ? c.CreatedDate
                        : sortColumnIndex == 4 ? c.UpdatedDate
                            : c.SpecialityName;
            allFeaturedSpeciality = sortDirection == "asc" ? allFeaturedSpeciality.OrderBy(orderingFunction).ToList() : allFeaturedSpeciality.OrderByDescending(orderingFunction).ToList();

            var display = allFeaturedSpeciality.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allFeaturedSpeciality.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("_FeaturedSpeciality/{id?}")]
        [HttpGet]
        public PartialViewResult _FeaturedSpeciality(int id)
        {
            ViewBag.SpecialityList = _speciality.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.SpecialityName,
                Value = x.SpecialityId.ToString()
            }).ToList();
            if (id == 0) return PartialView(@"Partial/_FeaturedSpeciality", new FeaturedSpecialityViewModel());
            var dfSpeciality = _featuredSpeciality.GetById(id);
            var result = AutoMapper.Mapper.Map<FeaturedSpecialityViewModel>(dfSpeciality);
            if (result == null) return PartialView(@"Partial/_FeaturedSpeciality", new FeaturedSpecialityViewModel());

            return PartialView(@"Partial/_FeaturedSpeciality", result);
        }

        [HttpPost, Route("AddEditFeaturedSpeciality"), ValidateAntiForgeryToken]
        public JsonResult AddEditFeaturedSpeciality(FeaturedSpecialityViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.FeaturedSpecialityId == 0)
                        {
                            var bResult = _featuredSpeciality.GetSingle(x => x.SpecialityId == model.SpecialityId && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Featured Speciality already exists. Could not add the data!" });
                            }
                            if (Request.Files.Count == 0)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 1, Message = "Featured Speciality save successfully" });
                            }
                            model.ProfilePic = Request.Files[0];
                            string extension = Path.GetExtension(model.ProfilePic.FileName);
                            model.ProfilePicture = $@"Featured-Speciality-{DateTime.Now.Ticks}{extension}";
                            filePath = Path.Combine(Server.MapPath(FilePathList.FeaturedSpeciality), model.ProfilePicture);
                            Common.CheckServerPath(Server.MapPath(FilePathList.FeaturedSpeciality));

                            model.ProfilePic.SaveAs(filePath);
                          
                            
                             //var oHour = AutoMapper.Mapper.Map<FeaturedSpecialityViewModel, FeaturedSpeciality>(model);
                            var oHour = new FeaturedSpeciality();
                            oHour.FeaturedSpecialityId = model.FeaturedSpecialityId;
                            oHour.SpecialityId = model.SpecialityId;
                            //oHour.Speciality = model.SpecialityName;
                            oHour.ProfilePicture = model.ProfilePicture;
                            oHour.Description = model.Description;
                            oHour.CreatedDate = DateTime.Now;
                            oHour.CreatedBy = User.Identity.GetUserId<int>();
                            //sonu20211023-end
                            oHour.IsActive = true;
                            _featuredSpeciality.InsertData(oHour);
                            _featuredSpeciality.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Featured Speciality save successfully" });
                        }
                        else
                        {
                            var bResult = _featuredSpeciality.GetSingle(x => x.FeaturedSpecialityId != model.FeaturedSpecialityId && x.IsActive && !x.IsDeleted
                                                            && x.SpecialityId == model.SpecialityId);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Featured Speciality already exists. Could not add the data!" });
                            }

                            var oHour = _featuredSpeciality.GetById(model.FeaturedSpecialityId);
                            oHour.SpecialityId = model.SpecialityId;
                            _featuredSpeciality.UpdateData(oHour);
                            _featuredSpeciality.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Featured Speciality update successfully" });
                        }
                    }
                    else
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse { Status = 4, Data = ModelState.Errors() });
                    }
                }
                catch (Exception ex)
                {
                    Common.DeleteFile(filePath, true);
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditFeaturedSpeciality-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveFeaturedSpeciality/{id?}")]
        public JsonResult RemoveFeaturedSpeciality(int id)
        {
            var featuredSpeciality = _featuredSpeciality.GetById(id);
            Common.DeleteFile($@"{FilePathList.FeaturedSpeciality}{featuredSpeciality.ProfilePicture}");
            _featuredSpeciality.DeleteData(featuredSpeciality);
            _featuredSpeciality.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Featured Speciality remove successfully" });
        }
        #endregion

        #region Languages
        [HttpPost, Route("ActiveDeLanguage/{flag}/{id}")]
        public JsonResult ActiveDeLanguage(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("spActiveDoctorLanguage @Id,@IsActive  ",

                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = !flag })
                         ;
            //var doctor = _doctor.GetById(id);
            //doctor.IsActive = !flag;
            //doctor.DoctorUser.IsActive = !flag;
            //_doctor.UpdateData(doctor);
            //_doctor.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Language has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        [HttpGet, Route("DoctorLanguage/{id}")]
        public ActionResult DoctorLanguage(string id)
        {
            Session["DoctorSearch"] = id;

            return View();
        }
        [HttpGet, Route("GetDoctorLanguageList")]
        public ActionResult GetDoctorLanguageList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            Dictionary<int, string> SortColumns = new Dictionary<int, string>();
            SortColumns.Add(0, "LanguageName");
            SortColumns.Add(1, "LanguageCode");
            SortColumns.Add(2, "IsActive");

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            Pagination objPagination = new Pagination()
            {
                StartIndex = param.iDisplayStart,
                PageSize = param.iDisplayLength,
                SortColumnName = SortColumns[sortColumnIndex],
                SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
            };

            objPagination.Search = param.sSearch ?? "";
            var languages = _doctor.GetDoctorLanguages(objPagination, id).ToList();
            var total = languages.Count > 0 ? languages.FirstOrDefault().TotalRows : 0;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = languages
            }, JsonRequestBehavior.AllowGet);


        }
        [HttpPost, Route("AddEditDoctorLanguage"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorLanguage(DoctorLanguageViewModel model)
        {
            model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    if (model.DoctorLanguageId == 0)
                    {



                        var Language = new DoctorLanguage()
                        {
                            DoctorLanguageId = model.DoctorLanguageId,
                            LanguageId = Convert.ToInt16(model.LanguageId),
                            DoctorId = model.DoctorId,
                            IsActive = model.IsActive,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };


                        _repo.Insert<DoctorLanguage>(Language, true);
                        // _Language.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = " Language save successfully" });
                    }
                    else
                    {

                        /*    var bResultdata = _repo.Find<DoctorLanguage>(x => x.DoctorLanguageId != model.DoctorLanguageId && x.DoctorId == model.DoctorId  && !x.IsDeleted);
                            if (bResultdata != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = " Language already exists. Could not add the data!" });
                            }
                            */
                        var age = _repo.Find<DoctorLanguage>(x => x.DoctorLanguageId == model.DoctorLanguageId);
                        age.LanguageId = Convert.ToInt16(model.LanguageId);
                        age.IsActive = model.IsActive;
                        _repo.Update<DoctorLanguage>(age, true);
                        // _Language.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = " Language update successfully" });
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditDoctorLanguage-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }
        [HttpGet, Route("AddEditDoctorLanguage/{id?}")]
        public ActionResult AddEditDoctorLanguage(int? id)
        {
            //int docLanguageid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorLanguageViewModel>("spDoctorLanguage @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            // var result = _repo.All<DoctorLanguage>().Where(x => x.DoctorLanguageId == id).FirstOrDefault();
            var list = new List<DropDownModel>();
            if (result != null)
            {
                //ViewBag.LanguageList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.LanguageId.ToString(),
                //    Selected = (x.LanguageId == result.LanguageId)
                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorLanguage @Activity  ",
                    new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "LanguageList" }
                 ).ToList();

                //  list = _repo.All<Languages>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.LanguageList = _Language.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.LanguageId.ToString(),

                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorLanguage @Activity  ",
                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "LanguageList" }
                  ).ToList();

            }
            if (id != 0 && id != null)
            {
                var Language = result;

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Language.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Language.LanguagesList = list;
                Language.DoctorId = Convert.ToInt32(DoctorId);
                return View(Language);
            }
            else
            {
                var Language = new DoctorLanguageViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Language.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Language.LanguagesList = list;
                Language.DoctorId = Convert.ToInt32(DoctorId);
                return View(Language);
            }

        }

        [HttpGet, Route("AddEditViewDoctorLanguage/{id?}")]
        public ActionResult AddEditViewDoctorLanguage(int? id)
        {
            //int docLanguageid = Convert.ToInt32(id);
            id = id ?? 0;

            var result = _repo.ExecWithStoreProcedure<DoctorLanguageViewModel>("spDoctorLanguage @Activity,@Id",
               new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                new SqlParameter("Id", System.Data.SqlDbType.NVarChar) { Value = id }
            ).ToList().FirstOrDefault();
            var list = new List<DropDownModel>();
            if (result != null)
            {
                //ViewBag.LanguageList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.LanguageId.ToString(),
                //    Selected = (x.LanguageId == result.LanguageId)
                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorLanguage @Activity  ",
                 new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "LanguageList" }
              ).ToList();

            }
            else
            {
                //ViewBag.LanguageList = _Language.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.LanguageId.ToString(),

                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorLanguage @Activity  ",
                 new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "LanguageList" }
              ).ToList();


            }
            if (id != 0 && id != null)
            {



                var DoctorId = Convert.ToString(Session["DoctorSearch"]);

                result.LanguagesList = list;
                result.IsViewMode = true;
                // result.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorLanguage", result);
            }
            else
            {
                var Language = new DoctorLanguageViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Language.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Language.LanguagesList = list;
                Language.IsViewMode = true;
                Language.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorLanguage", Language);
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AddDoctorLanguage(DoctorLanguageViewModel model)
        {
            var language = new DoctorLanguage
            {
                LanguageId = (short)model.LanguageId,
                DoctorId = model.DoctorId,
                IsActive = true,
                IsDeleted = false
            };
            _doctorLanguageService.InsertData(language);
            _doctorLanguageService.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Languauge added successfully" });
        }

        [HttpPost, Route("RemoveDoctorLanguage/{id?}")]
        public JsonResult RemoveDoctorLanguage(int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorLanguage @Activity ,@Id  ",
                         new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Delete" },
                      new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                     ;
            return Json(new JsonResponse() { Status = 1, Message = "Languauge removed successfully" });
        }

        #endregion

        #region Locations

        [HttpGet, Route("MyLocations")]
        public ActionResult MyLocations()
        {
            return View();
        }

        public ActionResult GetLocationList(JQueryDataTableParamModel param)
        {
            _doctorId = GetDoctorId();
            var locations = _address.GetAll(x => x.IsActive && !x.IsDeleted && x.ReferenceId == _doctorId).Select(x => new AddressViewModel()
            {
                Id = x.AddressId,
                //CityName = x.CityStateZip.City,
                //StateName = x.CityStateZip.State,
                Country = x.Country,
                Address1 = x.Address1,
                Address2 = x.Address2,
                ZipCode = x.ZipCode,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate.ToDefaultFormate()
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                locations = locations.Where(x => x.CityName.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.StateName.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.Country.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.Address1.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.Address2.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.ZipCode.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<AddressViewModel, string> orderingFunction =
                     c => sortColumnIndex == 1 ? c.Address1
                         : sortColumnIndex == 2 ? c.Address2
                            : sortColumnIndex == 3 ? c.CityName
                                : sortColumnIndex == 4 ? c.StateName
                                    : sortColumnIndex == 5 ? c.Country
                                        : sortColumnIndex == 6 ? c.ZipCode
                                            : sortColumnIndex == 7 ? c.CreatedDate
                                                : c.UpdatedDate;
            locations = sortDirection == "asc" ? locations.OrderBy(orderingFunction).ToList() : locations.OrderByDescending(orderingFunction).ToList();


            var display = locations.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = locations.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("MyDoctorLocations/{id?}")]
        public PartialViewResult MyDoctorLocations(int id)
        {
            _doctorId = GetDoctorId();
            ViewBag.StateList = _state.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.StateName,
                Value = x.StateId.ToString()
            }).ToList();

            if (id == 0) return PartialView(@"Partial/_MyLocation", new AddressViewModel() { DoctorId = _doctorId });
            var doctorAddress = _address.GetById(id);
            var result = AutoMapper.Mapper.Map<AddressViewModel>(doctorAddress);
            if (result == null) return PartialView(@"Partial/_MyLocation", new AddressViewModel());
            return PartialView(@"Partial/_MyLocation", result);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("AddEditDoctorLocation")]
        public JsonResult AddEditDoctorLocation(AddressViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var address = new Address
                    {
                        ReferenceId = model.DoctorId,
                        IsActive = true,
                        IsDeleted = false,
                        Address1 = model.Address1,
                        Address2 = model.Address2,
                        CityId = model.CityId,
                        StateId = model.StateId,
                        ZipCode = model.ZipCode,
                        Country = model.Country
                        //IsDefault = model.IsDefault
                    };
                    if (model.IsDefault)
                    {
                        var locations = _address.GetAll(x => x.ReferenceId == model.DoctorId);
                        foreach (var item in locations)
                        {
                            //item.IsDefault = false;
                            _address.UpdateData(item);
                        }
                    }
                    if (model.AddressId > 0)
                    {
                        address.AddressId = model.AddressId;
                        _address.UpdateData(address);
                    }
                    else
                    {
                        _address.InsertData(address);
                    }
                    _address.SaveData();
                    txscope.Complete();
                    return Json(new JsonResponse() { Status = 1, Message = "Address saved successfully" });
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddReview-Post");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost, Route("RemoveDoctorLocation/{id?}")]
        public JsonResult RemoveDoctorLocation(int id)
        {
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("sprDeleteDocAddress " +
                         "@Id"
,
                                       new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id }

                        );
            return Json(new JsonResponse() { Status = 1, Message = "Address removed successfully" });
        }

        #endregion

        #region Controller Common
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
        #endregion

        #region DoctorAgeGroup



        [HttpPost, Route("AddEditDoctorAgeGroup"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorAgeGroup(AgeGroupViewModel model)
        {
            model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    if (model.Id == 0)
                    {



                        var agegroup = new Doctyme.Model.DoctorAgeGroup()
                        {
                            DoctorAgeGroupId = model.Id,
                            AgeGroupId = model.AgeGroupId,
                            DoctorId = model.DoctorId,
                            IsActive = model.IsActive,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };


                        _repo.Insert<DoctorAgeGroup>(agegroup, true);
                        // _agegroup.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = "Age Group save successfully" });
                    }
                    else
                    {



                        var age = _repo.Find<DoctorAgeGroup>(x => x.DoctorAgeGroupId == model.Id);
                        age.AgeGroupId = model.AgeGroupId;
                        age.IsActive = model.IsActive;
                        _repo.Update<DoctorAgeGroup>(age, true);
                        // _agegroup.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = "Age Group update successfully" });
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditDoctorAgeGroup-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }
        [HttpGet, Route("AddEditDoctorAgeGroup/{id?}")]
        public ActionResult AddEditDoctorAgeGroup(int? id)
        {
            //int docagegroupid = Convert.ToInt32(id);
            var result = _repo.Find<DoctorAgeGroup>(x => x.DoctorAgeGroupId == id);
            var list = new List<Doctyme.Model.AgeGroup>();
            if (result != null)
            {
                //ViewBag.AgegroupList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AgeGroupId.ToString(),
                //    Selected = (x.AgeGroupId == result.AgeGroupId)
                //}).ToList();
                list = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.AgegroupList = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AgeGroupId.ToString(),

                //}).ToList();
                list = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).ToList();

            }
            if (id != 0 && id != null)
            {
                var agegroup = new AgeGroupViewModel()
                {
                    AgeGroupId = result.AgeGroupId,
                    Name = result.AgeGroup?.Name,
                    Description = result.AgeGroup?.Description,
                    DoctorName = result.Doctor?.Name,
                    Id = result.DoctorAgeGroupId,
                    IsActive = result.IsActive
                };

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                agegroup.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                agegroup.AgeGroupsList = list;
                agegroup.DoctorId = Convert.ToInt32(DoctorId);
                return View(agegroup);
            }
            else
            {
                var agegroup = new AgeGroupViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);

                agegroup.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                agegroup.AgeGroupsList = list;
                agegroup.DoctorId = Convert.ToInt32(DoctorId);
                return View(agegroup);
            }

        }

        [HttpGet, Route("AddEditViewDoctorAgeGroup/{id?}")]
        public ActionResult AddEditViewDoctorAgeGroup(int? id)
        {
            //int docagegroupid = Convert.ToInt32(id);
            var result = _repo.Find<DoctorAgeGroup>(x => x.DoctorAgeGroupId == id);
            var list = new List<Doctyme.Model.AgeGroup>();
            if (result != null)
            {
                //ViewBag.AgegroupList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AgeGroupId.ToString(),
                //    Selected = (x.AgeGroupId == result.AgeGroupId)
                //}).ToList();
                list = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.AgegroupList = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AgeGroupId.ToString(),

                //}).ToList();
                list = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).ToList();

            }
            if (id != 0 && id != null)
            {
                var agegroup = new AgeGroupViewModel()
                {
                    AgeGroupId = result.AgeGroupId,
                    Name = result.AgeGroup?.Name,
                    Description = result.AgeGroup?.Description,
                    DoctorName = result.Doctor?.Name,
                    Id = result.DoctorAgeGroupId,
                    IsViewMode = true
                };

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                agegroup.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                agegroup.AgeGroupsList = list;
                agegroup.IsViewMode = true;
                agegroup.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorAgeGroup", agegroup);
            }
            else
            {
                var agegroup = new AgeGroupViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                agegroup.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                agegroup.AgeGroupsList = list;
                agegroup.IsViewMode = true;
                agegroup.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorAgeGroup", agegroup);
            }

        }
        [HttpGet, Route("_DoctorAgeGroup/{id?}")]
        public PartialViewResult _DoctorAgeGroup(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DoctorAgeGroup", new AgeGroupViewModel());
            var result = _repo.Find<DoctorAgeGroup>(id);
            ViewBag.AgegroupList = _agegroup.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.AgeGroupId.ToString(),
                Selected = (x.AgeGroupId == result.AgeGroupId)
            }).ToList();

            var agegroup = new AgeGroupViewModel()
            {
                AgeGroupId = result.AgeGroupId,
                Name = result.AgeGroup.Name,
                Description = result.AgeGroup.Description,
                DoctorName = result.Doctor.Name,
                Id = result.DoctorAgeGroupId
            };
            if (result == null) return PartialView(@"Partial/_DoctorAgeGroup", new AgeGroupViewModel());

            return PartialView(@"Partial/_DoctorAgeGroup", agegroup);
        }

        [Route("DoctorAgeGroup/{id}")]
        public ActionResult DoctorAgeGroup(string id)
        {
            Session["DoctorSearch"] = id;
            DoctorBasicInformation doc = new DoctorBasicInformation()
            {
                DoctorId = Convert.ToInt32(id)
            };
            return View(doc);
        }

        [Route("GetDoctorAgeGroupList")]
        public ActionResult GetDoctorAgeGroupList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            var allagegroup = _repo.All<DoctorAgeGroup>().Where(x => !x.IsDeleted && x.DoctorId == id).Select(x => new AgeGroupViewModel()
            {
                AgeGroupId = x.AgeGroupId,
                Name = x.AgeGroup.Name,
                Description = x.AgeGroup.Description,
                IsActive = x.IsActive,
                DoctorName = x.Doctor.FirstName,
                Id = x.DoctorAgeGroupId
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allagegroup = allagegroup.Where(x => x.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.CreatedDate != null && x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower()))
                                                || (x.UpdatedDate != null && x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower()))).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<AgeGroupViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.CreatedDate
                        : sortColumnIndex == 3 ? c.UpdatedDate
                            : c.Name;
            allagegroup = sortDirection == "asc" ? allagegroup.OrderBy(orderingFunction).ToList() : allagegroup.OrderByDescending(orderingFunction).ToList();

            var display = allagegroup.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allagegroup.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("RemoveDoctorAgeGroup/{id?}")]
        public JsonResult RemoveDoctorAgeGroup(int id)
        {
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("SprDeleteDoctorAgeGroup " +
                         "@Id"
,
                                       new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id }

                        );
            return Json(new JsonResponse() { Status = 1, Message = "Age Group remove successfully" });
        }

        [HttpPost, Route("ActiveDeAgeGroup/{flag}/{id}")]
        public JsonResult ActiveDeAgeGroup(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("SprDoctorAgeGroup @Id,@IsActive  ",

                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                          new SqlParameter("IsActive", System.Data.SqlDbType.Int) { Value = !flag })
                         ;

            return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor Age Group has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }

        #endregion

        #region DoctorSpeciality



        [HttpPost, Route("AddEditDoctorSpeciality"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorSpeciality(SpecialityViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    if (model.Id == 0)
                    {

                        var bResult = _repo.Find<DoctorSpeciality>(x => x.Speciality.SpecialityName.ToLower().Equals(model.SpecialityName.ToLower()) && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {

                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0, Message = "Speciality already exists. Could not add the data!" });
                        }

                        var Speciality = new Doctyme.Model.DoctorSpeciality()
                        {
                            DoctorSpecialityId = model.Id,
                            SpecialityId = model.SpecialityId,
                            DoctorId = model.DoctorId,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };


                        _repo.Insert<DoctorSpeciality>(Speciality, true);
                        // _Speciality.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = "Speciality save successfully" });
                    }
                    else
                    {

                        var bResultdata = _repo.Find<DoctorSpeciality>(x => x.SpecialityId == model.SpecialityId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                        if (bResultdata != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0, Message = "Speciality already exists. Could not add the data!" });
                        }

                        var age = _repo.Find<DoctorSpeciality>(x => x.DoctorSpecialityId == model.Id);
                        age.SpecialityId = model.SpecialityId;

                        _repo.Update<DoctorSpeciality>(age, true);
                        // _Speciality.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = "Speciality update successfully" });
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditDoctorSpeciality-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }
        [HttpGet, Route("AddEditDoctorSpeciality/{id?}")]
        public ActionResult AddEditDoctorSpeciality(int? id)
        {
            //int docSpecialityid = Convert.ToInt32(id);
            var result = _repo.Find<DoctorSpeciality>(x => x.DoctorSpecialityId == id);
            var list = new List<Doctyme.Model.Speciality>();
            if (result != null)
            {
                //ViewBag.SpecialityList = .Select(x => new SelectListItem
                //{
                //    Text = x.SpecialityName,
                //    Value = x.SpecialityId.ToString(),
                //    Selected = (x.SpecialityId == result.SpecialityId)
                //}).ToList();
                list = _repo.All<Speciality>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.SpecialityList = _Speciality.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.SpecialityName,
                //    Value = x.SpecialityId.ToString(),

                //}).ToList();
                list = _repo.All<Speciality>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();

            }
            if (id != 0 && id != null)
            {
                var Speciality = new SpecialityViewModel()
                {
                    SpecialityId = result.SpecialityId,
                    SpecialityName = result.Speciality?.SpecialityName,
                    Description = result.Speciality?.Description,
                    Id = result.DoctorSpecialityId
                };

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Speciality.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Speciality.SpecialitysList = list;
                Speciality.DoctorId = Convert.ToInt32(DoctorId);
                return View(Speciality);
            }
            else
            {
                var Speciality = new SpecialityViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Speciality.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Speciality.SpecialitysList = list;
                Speciality.DoctorId = Convert.ToInt32(DoctorId);
                return View(Speciality);
            }

        }

        [HttpGet, Route("AddEditViewDoctorSpeciality/{id?}")]
        public ActionResult AddEditViewDoctorSpeciality(int? id)
        {
            //int docSpecialityid = Convert.ToInt32(id);
            var result = _repo.Find<DoctorSpeciality>(x => x.DoctorSpecialityId == id);
            var list = new List<Doctyme.Model.Speciality>();
            if (result != null)
            {
                //ViewBag.SpecialityList = .Select(x => new SelectListItem
                //{
                //    Text = x.SpecialityName,
                //    Value = x.SpecialityId.ToString(),
                //    Selected = (x.SpecialityId == result.SpecialityId)
                //}).ToList();
                list = _repo.All<Speciality>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.SpecialityList = _Speciality.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.SpecialityName,
                //    Value = x.SpecialityId.ToString(),

                //}).ToList();
                list = _repo.All<Speciality>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();

            }
            if (id != 0 && id != null)
            {
                var Speciality = new SpecialityViewModel()
                {
                    SpecialityId = result.SpecialityId,
                    SpecialityName = result.Speciality?.SpecialityName,
                    Description = result.Speciality?.Description,
                    Id = result.DoctorSpecialityId,
                    IsViewMode = true
                };

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Speciality.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Speciality.SpecialitysList = list;
                Speciality.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorSpeciality", Speciality);
            }
            else
            {
                var Speciality = new SpecialityViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Speciality.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Speciality.SpecialitysList = list;

                Speciality.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorSpeciality", Speciality);
            }

        }


        [Route("DoctorSpeciality/{id}")]
        public ActionResult DoctorSpeciality(string id)
        {
            Session["DoctorSearch"] = id;
            SpecialityViewModel doc = new SpecialityViewModel()
            {
                DoctorId = Convert.ToInt32(id)
            };
            return View(doc);
        }

        [Route("GetDoctorSpecialityList")]
        public ActionResult GetDoctorSpecialityList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            var allSpeciality = _repo.All<DoctorSpeciality>().Where(x => x.IsActive && !x.IsDeleted && x.DoctorId == id).Select(x => new SpecialityViewModel()
            {
                SpecialityId = x.SpecialityId,
                SpecialityName = x.Speciality.SpecialityName,
                Description = x.Speciality.Description,
                IsActive = x.IsActive,
                Id = x.DoctorSpecialityId
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allSpeciality = allSpeciality.Where(x => x.SpecialityName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<SpecialityViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.SpecialityName
                    : sortColumnIndex == 2 ? c.CreatedDate
                        : sortColumnIndex == 3 ? c.UpdatedDate
                            : c.SpecialityName;
            allSpeciality = sortDirection == "asc" ? allSpeciality.OrderBy(orderingFunction).ToList() : allSpeciality.OrderByDescending(orderingFunction).ToList();

            var display = allSpeciality.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allSpeciality.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("RemoveDoctorSpeciality/{id?}")]
        public JsonResult RemoveDoctorSpeciality(int id)
        {
            var Speciality = _repo.Find<DoctorSpeciality>(x => x.DoctorSpecialityId == id);
            Speciality.IsDeleted = true;
            _repo.Update<DoctorSpeciality>(Speciality, true);
            // _Speciality.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Speciality remove successfully" });
        }

        #endregion

        #region DoctorGender



        [HttpPost, Route("AddEditDoctorGender"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorGender(DoctorGenderViewModel model)
        {
            model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
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
                            return Json(new JsonResponse { Status = 0, Message = "Gender already exists. Could not add the data!" });
                        }

                        var Gender = new Doctyme.Model.DoctorGender()
                        {
                            DoctorGenderId = model.DoctorGenderTypeId,
                            GenderTypeId = model.GenderTypeId,
                            DoctorId = model.DoctorId,
                            IsActive = model.IsActive,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };


                        _repo.Insert<DoctorGender>(Gender, true);
                        // _Gender.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = "Gender save successfully" });
                    }
                    else
                    {

                        var bResultdata = _repo.Find<DoctorGender>(x => x.DoctorGenderId == model.DoctorGenderTypeId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                        if (bResultdata != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0, Message = "Gender already exists. Could not add the data!" });
                        }

                        var gender = _repo.Find<DoctorGender>(x => x.DoctorGenderId == model.DoctorGenderTypeId);
                        gender.GenderTypeId = model.GenderTypeId;
                        gender.IsActive = model.IsActive;

                        _repo.Update<DoctorGender>(gender, true);
                        // _Gender.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = "Gender update successfully" });
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditDoctorGender-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }
        [HttpGet, Route("AddEditDoctorGender/{id?}")]
        public ActionResult AddEditDoctorGender(int? id)
        {
            //int docGenderid = Convert.ToInt32(id);
            var result = _repo.Find<DoctorGender>(x => x.DoctorGenderId == id);
            var list = new List<Doctyme.Model.GenderType>();
            if (result != null)
            {
                //ViewBag.GenderList = .Select(x => new SelectListItem
                //{
                //    Text = x.GenderName,
                //    Value = x.GenderId.ToString(),
                //    Selected = (x.GenderId == result.GenderId)
                //}).ToList();
                list = _repo.All<GenderType>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.GenderList = _Gender.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.GenderName,
                //    Value = x.GenderId.ToString(),

                //}).ToList();
                list = _repo.All<GenderType>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();

            }
            if (id != 0 && id != null)
            {
                var Gender = new DoctorGenderViewModel()
                {
                    DoctorGenderTypeId = result.DoctorGenderId,
                    GenderTypeId = result.GenderTypeId
                };

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Gender.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Gender.GendersList = list;
                Gender.DoctorId = Convert.ToInt32(DoctorId);
                return View(Gender);
            }
            else
            {
                var Gender = new DoctorGenderViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Gender.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Gender.GendersList = list;
                Gender.DoctorId = Convert.ToInt32(DoctorId);
                return View(Gender);
            }

        }

        [HttpGet, Route("AddEditViewDoctorGender/{id?}")]
        public ActionResult AddEditViewDoctorGender(int? id)
        {
            //int docGenderid = Convert.ToInt32(id);
            var result = _repo.Find<DoctorGender>(x => x.DoctorGenderId == id);
            var list = new List<Doctyme.Model.GenderType>();
            if (result != null)
            {
                //ViewBag.GenderList = .Select(x => new SelectListItem
                //{
                //    Text = x.GenderName,
                //    Value = x.GenderId.ToString(),
                //    Selected = (x.GenderId == result.GenderId)
                //}).ToList();
                list = _repo.All<GenderType>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.GenderList = _Gender.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.GenderName,
                //    Value = x.GenderId.ToString(),

                //}).ToList();
                list = _repo.All<GenderType>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();

            }
            if (id != 0 && id != null)
            {
                var Gender = new DoctorGenderViewModel()
                {
                    GenderTypeId = result.GenderTypeId,
                    DoctorGenderTypeId = result.DoctorGenderId,
                    IsViewMode = true
                };

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Gender.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Gender.GendersList = list;
                Gender.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorGender", Gender);
            }
            else
            {
                var Gender = new DoctorGenderViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Gender.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Gender.GendersList = list;

                Gender.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorGender", Gender);
            }

        }


        [Route("DoctorGender/{id}")]
        public ActionResult DoctorGender(string id)
        {
            Session["DoctorSearch"] = id;
            DoctorGenderViewModel doc = new DoctorGenderViewModel()
            {
                DoctorGenderTypeId = Convert.ToInt32(id)
            };
            return View(doc);
        }

        [Route("GetDoctorGenderList")]
        public ActionResult GetDoctorGenderList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            var allGender = _repo.All<DoctorGender>().Where(x => !x.IsDeleted && x.DoctorId == id).Select(x => new DoctorGenderViewModel()
            {
                GenderTypeId = x.GenderTypeId,
                Name = x.Gender.Name,
                Description = x.Gender.Description,
                IsActive = x.IsActive,
                DoctorGenderTypeId = x.DoctorGenderId,

            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allGender = allGender.Where(x => x.GenderTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DoctorGenderViewModel, int> orderingFunction =
                c => sortColumnIndex == 1 ? c.GenderTypeId

                            : c.DoctorGenderTypeId;
            allGender = sortDirection == "asc" ? allGender.OrderBy(orderingFunction).ToList() : allGender.OrderByDescending(orderingFunction).ToList();

            var display = allGender.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allGender.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("RemoveDoctorGender/{id?}")]
        public JsonResult RemoveDoctorGender(int id)
        {
            var Gender = _repo.Find<DoctorGender>(x => x.DoctorGenderId == id);
            Gender.IsDeleted = true;
            _repo.Update<DoctorGender>(Gender, true);
            // _Gender.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Gender remove successfully" });
        }

        #endregion

        #region DoctorBoardCertification

        [HttpPost, Route("ActiveDeBoardCertification/{flag}/{id}")]
        public JsonResult ActiveDeBoardCertification(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorBoardCertification @Activity ,@Id,@IsActive  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Activate" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                          new SqlParameter("IsActive", System.Data.SqlDbType.Int) { Value = !flag })
                         ;
            //var doctor = _doctor.GetById(id);
            //doctor.IsActive = !flag;
            //doctor.DoctorUser.IsActive = !flag;
            //_doctor.UpdateData(doctor);
            //_doctor.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Board Certification has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }

        [HttpPost, Route("AddEditDoctorBoardCertification"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorBoardCertification(DoctorBoardCertificationViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
                    if (model.DoctorBoardCertificationId == 0)
                    {

                        var bResult = _repo.Find<DoctorBoardCertification>(x => x.BoardCertificationId == model.BoardCertificationId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {

                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0, Message = " Board Certification already exists. Could not add the data!" });
                        }

                        var BoardCertification = new DoctorBoardCertification()
                        {
                            DoctorBoardCertificationId = model.DoctorBoardCertificationId,
                            BoardCertificationId = Convert.ToInt16(model.BoardCertificationId),
                            DoctorId = model.DoctorId,
                            IsActive = model.IsActive,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };


                        _repo.Insert<DoctorBoardCertification>(BoardCertification, true);
                        // _BoardCertification.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = " Board Certification save successfully" });
                    }
                    else
                    {

                        var bResultdata = _repo.Find<DoctorBoardCertification>(x => x.BoardCertificationId == model.BoardCertificationId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
                        if (bResultdata != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0, Message = " Board Certification already exists. Could not add the data!" });
                        }

                        var age = _repo.Find<DoctorBoardCertification>(x => x.DoctorBoardCertificationId == model.DoctorBoardCertificationId);
                        age.BoardCertificationId = Convert.ToInt16(model.BoardCertificationId);
                        age.IsActive = model.IsActive;
                        _repo.Update<DoctorBoardCertification>(age, true);
                        // _BoardCertification.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse { Status = 1, Message = " Board Certification update successfully" });
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditDoctorBoardCertification-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }
        [HttpGet, Route("AddEditDoctorBoardCertification/{id?}")]
        public ActionResult AddEditDoctorBoardCertification(int? id)
        {
            //int docBoardCertificationid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorBoardCertificationViewModel>("spDoctorBoardCertification @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            // var result = _repo.All<DoctorBoardCertification>().Where(x => x.DoctorBoardCertificationId == id).FirstOrDefault();
            var list = new List<DropDownModel>();
            if (result != null)
            {
                //ViewBag.BoardCertificationList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.BoardCertificationId.ToString(),
                //    Selected = (x.BoardCertificationId == result.BoardCertificationId)
                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
                    new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
                 ).ToList();

                //  list = _repo.All<BoardCertifications>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.BoardCertificationList = _BoardCertification.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.BoardCertificationId.ToString(),

                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
                  ).ToList();

            }
            if (id != 0 && id != null)
            {
                var BoardCertification = result;

                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                BoardCertification.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                BoardCertification.BoardCertificationsList = list;
                BoardCertification.DoctorId = Convert.ToInt32(DoctorId);
                return View(BoardCertification);
            }
            else
            {
                var BoardCertification = new DoctorBoardCertificationViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                BoardCertification.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                BoardCertification.BoardCertificationsList = list;
                BoardCertification.DoctorId = Convert.ToInt32(DoctorId);
                return View(BoardCertification);
            }

        }

        [HttpGet, Route("AddEditViewDoctorBoardCertification/{id?}")]
        public ActionResult AddEditViewDoctorBoardCertification(int? id)
        {
            //int docBoardCertificationid = Convert.ToInt32(id);
            var result = _repo.ExecWithStoreProcedure<DoctorBoardCertificationViewModel>("spDoctorBoardCertification @Activity ,@Id  ",
                      new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                   new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                   .FirstOrDefault();
            var list = new List<DropDownModel>();
            if (result != null)
            {
                //ViewBag.BoardCertificationList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.BoardCertificationId.ToString(),
                //    Selected = (x.BoardCertificationId == result.BoardCertificationId)
                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
                 new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
              ).ToList();

            }
            else
            {
                //ViewBag.BoardCertificationList = _BoardCertification.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.BoardCertificationId.ToString(),

                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorBoardCertification @Activity  ",
                 new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "BoardCertificationList" }
              ).ToList();


            }
            if (id != 0 && id != null)
            {



                var DoctorId = Convert.ToString(Session["DoctorSearch"]);

                result.BoardCertificationsList = list;
                result.IsViewMode = true;
                // result.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorBoardCertification", result);
            }
            else
            {
                var BoardCertification = new DoctorBoardCertificationViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                BoardCertification.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                BoardCertification.BoardCertificationsList = list;
                BoardCertification.IsViewMode = true;
                BoardCertification.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorBoardCertification", BoardCertification);
            }

        }


        [Route("DoctorBoardCertification/{id}")]
        public ActionResult DoctorBoardCertification(string id)
        {
            Session["DoctorSearch"] = id;
            DoctorBoardCertificationViewModel doc = new DoctorBoardCertificationViewModel()
            {
                DoctorId = Convert.ToInt32(id)
            };
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }
            return View(doc);
        }

        [Route("GetDoctorBoardCertificationList")]
        public ActionResult GetDoctorBoardCertificationList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            Dictionary<int, string> SortColumns = new Dictionary<int, string>();
            SortColumns.Add(0, "Name");
            SortColumns.Add(1, "Description");
            SortColumns.Add(2, "IsActive");

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            Pagination objPagination = new Pagination()
            {
                StartIndex = param.iDisplayStart,
                PageSize = param.iDisplayLength,
                SortColumnName = SortColumns[sortColumnIndex],
                SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
            };

            objPagination.Search = param.sSearch ?? "";

            var allBoardCertification = _doctor.GetDoctorBoardCertifications(objPagination, id).Where(x => !x.IsDeleted && x.DoctorId == id).Select(x => new DoctorBoardCertificationViewModel()
            {
                BoardCertificationId = x.BoardCertificationId,
                Name = x.BoardCertificationName,
                Description = x.Description,
                IsActive = x.IsActive,
                DoctorName = x.DoctorName,
                Id = x.DoctorBoardCertificationId,
                TotalRows = x.TotalRows
            }).ToList();


            var total = allBoardCertification.Count > 0 ? allBoardCertification.FirstOrDefault().TotalRows : 0;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allBoardCertification
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("RemoveDoctorBoardCertification/{id?}")]
        public JsonResult RemoveDoctorBoardCertification(int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorBoardCertification @Activity ,@Id  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Delete" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                         ;
            // var BoardCertification = _repo.All<DoctorBoardCertification>().ToList().Where(x=>x.DoctorBoardCertificationId==id).FirstOrDefault();
            //BoardCertification.IsDeleted = true;
            //_repo.Update<DoctorBoardCertification>(BoardCertification,true);

            return Json(new JsonResponse() { Status = 1, Message = " Board Certification remove successfully" });
        }

        #endregion

        #region DoctorAffiliation
        [HttpPost, Route("ActiveDeAffiliation/{flag}/{id}")]
        public JsonResult ActiveDeAffiliation(bool flag, int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorAffiliation @Activity ,@Id,@IsActive  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Activate" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id },
                          new SqlParameter("IsActive", System.Data.SqlDbType.Int) { Value = !flag })
                         ;
            //var doctor = _doctor.GetById(id);
            //doctor.IsActive = !flag;
            //doctor.DoctorUser.IsActive = !flag;
            //_doctor.UpdateData(doctor);
            //_doctor.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor Affiliation has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }

        [HttpPost, Route("GetDoctorAffiliationAutocomplete/{Prefix}")]
        public JsonResult GetDoctorAffiliationAutocomplete(string Prefix)
        {
            var list = _repo.ExecWithStoreProcedure<DropDownModel>("spGetDoctorAffiliationAutocomplete @Organization",
                new SqlParameter("@Organization", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("AddEditDoctorAffiliation"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorAffiliation(DoctorAffiliationViewModel model)
        {
            model.IsActive = Convert.ToString(Request["IsActive"]) == "on" ? true : false;
            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDoctorAffiliation " +
                            "@Activity," +
                             "@Id," +
                          "@OrganisationId," +
                           "@DoctorId," +
                           "@AddressId," +
                           "@IsActive," +
                          "@CreatedBy"

                            , new SqlParameter("Activity", System.Data.SqlDbType.VarChar) { Value = "Insert" },
                              new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = model.DoctorAffiliationId },
                              new SqlParameter("OrganisationId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId },
                                  new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() }
                         );
            return Json(new JsonResponse() { Status = 1, Message = "Affiliation save successfully" });
        }
        [HttpGet, Route("AddEditDoctorAffiliation/{id?}")]
        public ActionResult AddEditDoctorAffiliation(int? id)
        {
            //int docAffiliationid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorAffiliationViewModel>("spDoctorAffiliation @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            // var result = _repo.All<DoctorAffiliation>().Where(x => x.DoctorAffiliationId == id).FirstOrDefault();
            var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
            var addresses = _repo.ExecWithStoreProcedure<DoctorAffiliationViewModel>("GetAddressByAffiliationName @DoctorId, @OrganisationName  ",
                         new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = DoctorId },
                         new SqlParameter("OrganisationName", System.Data.SqlDbType.NVarChar) { Value = result != null ? result.OrganisationName : "" },
                         new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id });
            ViewBag.AddressList = addresses.Select(x =>
            new SelectListItem
            {
                Text = x.FullAddress,
                Value = x.AddressId.ToString()
            });

            var list = new List<DropDownModel>();
            if (result != null)
            {
                //ViewBag.AffiliationList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AffiliationId.ToString(),
                //    Selected = (x.AffiliationId == result.AffiliationId)
                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorAffiliation @Activity, @OrganisationId  ",
                    new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "OrganisationList" },
                    new SqlParameter("OrganisationId", System.Data.SqlDbType.Int) { Value = result.OrganisationId }
                 ).ToList();

                //  list = _repo.All<Affiliations>().ToList().Where(x => x.IsActive && !x.IsDeleted).ToList();
            }
            else
            {
                //ViewBag.AffiliationList = _Affiliation.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AffiliationId.ToString(),

                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorAffiliation @Activity  ",
                     new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "OrganisationList" }
                  ).ToList();

            }
            if (id != 0 && id != null)
            {
                var Affiliation = result;

                //var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Affiliation.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Affiliation.OrganizationList = list;
                Affiliation.DoctorId = Convert.ToInt32(DoctorId);
                return View(Affiliation);
            }
            else
            {
                var Affiliation = new DoctorAffiliationViewModel();
                //var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Affiliation.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Affiliation.OrganizationList = list;
                Affiliation.DoctorId = Convert.ToInt32(DoctorId);
                return View(Affiliation);
            }

        }

        [HttpPost, Route("GetAddressByAffiliationName/{id?}")]
        public ActionResult GetAddressByAffiliationName(int? id, string organisationName)
        {
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorAffiliationViewModel>("GetAddressByAffiliationName @DoctorId, @OrganisationName  ",
                         new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = id },
                         new SqlParameter("OrganisationName", System.Data.SqlDbType.NVarChar) { Value = organisationName },
                         new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                         .ToList();
            ViewBag.AddressList = result.Select(x =>
            new DoctorAffiliationViewModel
            {
                FullAddress = x.FullAddress,
                AddressId = x.AddressId
            });

            return Json(new JsonResponse() { Data = ViewBag.AddressList });
        }


        [HttpGet, Route("AddEditViewDoctorAffiliation/{id?}")]
        public ActionResult AddEditViewDoctorAffiliation(int? id)
        {
            //int docAffiliationid = Convert.ToInt32(id);
            id = id ?? 0;
            var result = _repo.ExecWithStoreProcedure<DoctorAffiliationViewModel>("spDoctorAffiliation @Activity ,@Id  ",
                       new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Find" },
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                    .FirstOrDefault();
            var list = new List<DropDownModel>();
            if (result != null)
            {
                //ViewBag.AffiliationList = .Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AffiliationId.ToString(),
                //    Selected = (x.AffiliationId == result.AffiliationId)
                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorAffiliation @Activity  ",
                 new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "OrganisationList" }
              ).ToList();

            }
            else
            {
                //ViewBag.AffiliationList = _Affiliation.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.AffiliationId.ToString(),

                //}).ToList();
                list = _repo.ExecWithStoreProcedure<DropDownModel>("spDoctorAffiliation @Activity  ",
                 new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "OrganisationList" }
              ).ToList();


            }
            if (id != 0 && id != null)
            {



                var DoctorId = Convert.ToString(Session["DoctorSearch"]);

                result.OrganizationList = list;
                result.IsViewMode = true;
                // result.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorAffiliation", result);
            }
            else
            {
                var Affiliation = new DoctorAffiliationViewModel();
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Affiliation.DoctorName = doctor == null ? "" : doctor.FirstName + " " + doctor.LastName;
                Affiliation.OrganizationList = list;
                Affiliation.IsViewMode = true;
                Affiliation.DoctorId = Convert.ToInt32(DoctorId);
                return View("AddEditDoctorAffiliation", Affiliation);
            }

        }


        [Route("DoctorAffiliations/{id}")]
        public ActionResult DoctorAffiliations(string id)
        {
            Session["DoctorSearch"] = id;
            DoctorAffiliationViewModel doc = new DoctorAffiliationViewModel()
            {
                DoctorId = Convert.ToInt32(id)
            };
            return View(doc);
        }

        [Route("GetDoctorAffiliationList")]
        public ActionResult GetDoctorAffiliationList(JQueryDataTableParamModel param)
        {
            var id = Convert.ToInt32(Session["DoctorSearch"]);
            Dictionary<int, string> SortColumns = new Dictionary<int, string>();
            SortColumns.Add(0, "OrganisationName");
            SortColumns.Add(1, "IsActive");

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            Pagination objPagination = new Pagination()
            {
                StartIndex = param.iDisplayStart,
                PageSize = param.iDisplayLength,
                SortColumnName = SortColumns[sortColumnIndex],
                SortDirection = HttpContext.Request.QueryString["sSortDir_0"] // asc or desc
            };

            objPagination.Search = param.sSearch ?? "";

            var allAffiliation = _doctor.GetDoctorAffiliations(objPagination, id).Where(x => !x.IsDeleted && x.DoctorId == id).Select(x => new DoctorAffiliationViewModel()
            {
                DoctorAffiliationId = x.DoctorAffiliationId,
                OrganisationName = x.OrganisationName,
                Description = x.Description,
                IsActive = x.IsActive,
                DoctorName = x.DoctorName,
                // Id = x.DoctorAffiliationId,
                FullAddress = x.Address1 + " " + x.Address2 + " " + x.City + " " + x.State + " " + x.ZipCode,
                TotalRows = x.TotalRows
            }).ToList();


            var total = allAffiliation.Count > 0 ? allAffiliation.FirstOrDefault().TotalRows : 0;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allAffiliation
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("RemoveDoctorAffiliation/{id?}")]
        public JsonResult RemoveDoctorAffiliation(int id)
        {
            int count = _repo.ExecuteSQLQuery("spDoctorAffiliation @Activity ,@Id  ",
                             new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "Delete" },
                          new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id })
                         ;
            // var Affiliation = _repo.All<DoctorAffiliation>().ToList().Where(x=>x.DoctorAffiliationId==id).FirstOrDefault();
            //Affiliation.IsDeleted = true;
            //_repo.Update<DoctorAffiliation>(Affiliation,true);

            return Json(new JsonResponse() { Status = 1, Message = " Affiliation remove successfully" });
        }

        #endregion

        //#region DoctorOrgTaxonomy



        //[HttpPost, Route("AddEditDoctorOrgTaxonomy"), ValidateAntiForgeryToken]
        //public JsonResult AddEditDoctorOrgTaxonomy(OrgTaxonomyViewModel model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {

        //            if (model.Id == 0)
        //            {

        //                var bResult = _repo.Find<DoctorOrgTaxonomy>(x => x.OrgTaxonomy.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
        //                if (bResult != null)
        //                {

        //                    txscope.Dispose();
        //                    return Json(new JsonResponse { Status = 0, Message = "Org Taxonomy already exists. Could not add the data!" });
        //                }

        //                var OrgTaxonomy = new Doctyme.Model.DoctorOrgTaxonomy()
        //                {
        //                    DoctorOrgTaxonomyId = model.Id,
        //                    OrgTaxonomyId = model.OrgTaxonomyId,
        //                    DoctorId = model.DoctorId,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    CreatedDate = DateTime.Now
        //                };


        //                _repo.Insert<DoctorOrgTaxonomy>(OrgTaxonomy, true);
        //                // _OrgTaxonomy.SaveData();
        //                txscope.Complete();
        //                return Json(new JsonResponse { Status = 1, Message = "Org Taxonomy save successfully" });
        //            }
        //            else
        //            {

        //                var bResultdata = _repo.Find<DoctorOrgTaxonomy>(x => x.OrgTaxonomyId == model.OrgTaxonomyId && x.DoctorId == model.DoctorId && x.IsActive && !x.IsDeleted);
        //                if (bResultdata != null)
        //                {
        //                    txscope.Dispose();
        //                    return Json(new JsonResponse { Status = 0, Message = "Org Taxonomy already exists. Could not add the data!" });
        //                }

        //                var age = _repo.All<DoctorOrgTaxonomy>().ToList().Where(x => x.DoctorOrgTaxonomyId == model.Id).FirstOrDefault();
        //                age.OrgTaxonomyId = model.OrgTaxonomyId;

        //                _repo.Update<DoctorOrgTaxonomy>(age, true);
        //                // _OrgTaxonomy.SaveData();
        //                txscope.Complete();
        //                return Json(new JsonResponse { Status = 1, Message = "Org Taxonomy update successfully" });
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            Common.LogError(ex, "AddEditDoctorOrgTaxonomy-Post");
        //            return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
        //        }
        //    }
        //}
        //[HttpGet, Route("AddEditDoctorOrgTaxonomy/{id?}")]
        //public ActionResult AddEditDoctorOrgTaxonomy(int? id)
        //{
        //    //int docOrgTaxonomyid = Convert.ToInt32(id);
        //    var result = _repo.All<DoctorOrgTaxonomy>().Where(x => x.DoctorOrgTaxonomyId == id).FirstOrDefault();
        //    var list = new List<Doctyme.Model.OrgTaxonomy>();
        //    if (result != null)
        //    {
        //        //ViewBag.OrgTaxonomyList = .Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.OrgTaxonomyId.ToString(),
        //        //    Selected = (x.OrgTaxonomyId == result.OrgTaxonomyId)
        //        //}).ToList();
        //        list = _OrgTaxonomy.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
        //    }
        //    else
        //    {
        //        //ViewBag.OrgTaxonomyList = _OrgTaxonomy.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.OrgTaxonomyId.ToString(),

        //        //}).ToList();
        //        list = _OrgTaxonomy.GetAll(x => x.IsActive && !x.IsDeleted).ToList();

        //    }
        //    if (id != 0 && id != null)
        //    {
        //        var OrgTaxonomy = new OrgTaxonomyViewModel()
        //        {
        //            OrgTaxonomyId = result.OrgTaxonomyId,
        //            Name = result.OrgTaxonomy?.Name,
        //            Description = result.OrgTaxonomy?.Description,
        //            DoctorName = result.Doctor?.Name,
        //            Id = result.DoctorOrgTaxonomyId
        //        };

        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);
        //        var doctor = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
        //        OrgTaxonomy.DoctorName =doctor==null?"":doctor.FirstName + " " + doctor.LastName;
        //        OrgTaxonomy.OrgTaxonomysList = list;
        //        OrgTaxonomy.DoctorId = Convert.ToInt32(DoctorId);
        //        return View(OrgTaxonomy);
        //    }
        //    else
        //    {
        //        var OrgTaxonomy = new OrgTaxonomyViewModel();
        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);
        //        var doctor = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
        //        OrgTaxonomy.DoctorName =doctor==null?"":doctor.FirstName + " " + doctor.LastName;
        //        OrgTaxonomy.OrgTaxonomysList = list;
        //        OrgTaxonomy.DoctorId = Convert.ToInt32(DoctorId);
        //        return View(OrgTaxonomy);
        //    }

        //}

        //[HttpGet, Route("AddEditViewDoctorOrgTaxonomy/{id?}")]
        //public ActionResult AddEditViewDoctorOrgTaxonomy(int? id)
        //{
        //    //int docOrgTaxonomyid = Convert.ToInt32(id);
        //    var result = _repo.All<DoctorOrgTaxonomy>().Where(x => x.DoctorOrgTaxonomyId == id).FirstOrDefault();
        //    var list = new List<Doctyme.Model.OrgTaxonomy>();
        //    if (result != null)
        //    {
        //        //ViewBag.OrgTaxonomyList = .Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.OrgTaxonomyId.ToString(),
        //        //    Selected = (x.OrgTaxonomyId == result.OrgTaxonomyId)
        //        //}).ToList();
        //        list = _OrgTaxonomy.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
        //    }
        //    else
        //    {
        //        //ViewBag.OrgTaxonomyList = _OrgTaxonomy.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
        //        //{
        //        //    Text = x.Name,
        //        //    Value = x.OrgTaxonomyId.ToString(),

        //        //}).ToList();
        //        list = _OrgTaxonomy.GetAll(x => x.IsActive && !x.IsDeleted).ToList();

        //    }
        //    if (id != 0 && id != null)
        //    {
        //        var OrgTaxonomy = new OrgTaxonomyViewModel()
        //        {
        //            OrgTaxonomyId = result.OrgTaxonomyId,
        //            Name = result.OrgTaxonomy?.Name,
        //            Description = result.OrgTaxonomy?.Description,
        //            DoctorName = result.Doctor?.Name,
        //            Id = result.DoctorOrgTaxonomyId,
        //            IsViewMode = true
        //        };

        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);
        //        //int docid =(int) DoctorId;
        //        var doctor = _repo.All<Doctor>().ToList().Where(x => x.DoctorId == Convert.ToInt32(DoctorId)).FirstOrDefault();
        //        OrgTaxonomy.DoctorName =doctor==null?"":doctor.FirstName + " " + doctor.LastName;
        //        OrgTaxonomy.OrgTaxonomysList = list;
        //        OrgTaxonomy.DoctorId = Convert.ToInt32(DoctorId);
        //        return View("AddEditDoctorOrgTaxonomy", OrgTaxonomy);
        //    }
        //    else
        //    {
        //        var OrgTaxonomy = new OrgTaxonomyViewModel();
        //        var DoctorId = Convert.ToString(Session["DoctorSearch"]);
        //        var doctor = _repo.All<Doctor>().ToList().Where(x => Convert.ToInt32(x.DoctorId) == Convert.ToInt32(DoctorId)).FirstOrDefault();
        //        OrgTaxonomy.DoctorName =doctor==null?"":doctor.FirstName + " " + doctor.LastName;
        //        OrgTaxonomy.OrgTaxonomysList = list;

        //        OrgTaxonomy.DoctorId = Convert.ToInt32(DoctorId);
        //        return View("AddEditDoctorOrgTaxonomy", OrgTaxonomy);
        //    }

        //}
        //[HttpGet, Route("_DoctorOrgTaxonomy/{id?}")]
        //public PartialViewResult _DoctorOrgTaxonomy(int id)
        //{

        //    if (id == 0) return PartialView(@"Partial/_DoctorOrgTaxonomy", new OrgTaxonomyViewModel());
        //    var result = _repo.Find<DoctorOrgTaxonomy>(id);
        //    ViewBag.OrgTaxonomyList = _OrgTaxonomy.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
        //    {
        //        Text = x.Name,
        //        Value = x.OrgTaxonomyId.ToString(),
        //        Selected = (x.OrgTaxonomyId == result.OrgTaxonomyId)
        //    }).ToList();

        //    var OrgTaxonomy = new OrgTaxonomyViewModel()
        //    {
        //        OrgTaxonomyId = result.OrgTaxonomyId,
        //        Name = result.OrgTaxonomy.Name,
        //        Description = result.OrgTaxonomy.Description,
        //        DoctorName = result.Doctor.Name,
        //        Id = result.DoctorOrgTaxonomyId
        //    };
        //    if (result == null) return PartialView(@"Partial/_DoctorOrgTaxonomy", new OrgTaxonomyViewModel());

        //    return PartialView(@"Partial/_DoctorOrgTaxonomy", OrgTaxonomy);
        //}

        //[Route("DoctorOrgTaxonomy/{id}")]
        //public ActionResult DoctorOrgTaxonomy(string id)
        //{
        //    Session["DoctorSearch"] = id;
        //    DoctorBasicInformation doc = new DoctorBasicInformation()
        //    {
        //        DoctorId = Convert.ToInt32(id)
        //    };
        //    return View(doc);
        //}

        //[Route("GetDoctorOrgTaxonomyList")]
        //public ActionResult GetDoctorOrgTaxonomyList(JQueryDataTableParamModel param)
        //{
        //    var id = Convert.ToInt32(Session["DoctorSearch"]);
        //    var allOrgTaxonomy = _repo.All<DocOrgTaxonomy>().Where(x => x.IsActive && !x.IsDeleted && x.DoctorId == id).Select(x => new OrgTaxonomyViewModel()
        //    {
        //        DocOrgTaxonomyID = x.OrgTaxonomyId,
        //        Taxonomy = x.OrgTaxonomy,
        //        Description = x.OrgTaxonomy.Description,
        //        IsActive = x.IsActive,
        //        DoctorName = x.Doctor.FirstName,
        //        Id = x.DoctorOrgTaxonomyId
        //    }).ToList();

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        allOrgTaxonomy = allOrgTaxonomy.Where(x => x.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
        //    }

        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

        //    Func<OrgTaxonomyViewModel, string> orderingFunction =
        //        c => sortColumnIndex == 1 ? c.Name
        //            : sortColumnIndex == 2 ? c.CreatedDate
        //                : sortColumnIndex == 3 ? c.UpdatedDate
        //                    : c.Name;
        //    allOrgTaxonomy = sortDirection == "asc" ? allOrgTaxonomy.OrderBy(orderingFunction).ToList() : allOrgTaxonomy.OrderByDescending(orderingFunction).ToList();

        //    var display = allOrgTaxonomy.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var total = allOrgTaxonomy.Count;

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost, Route("RemoveDoctorOrgTaxonomy/{id?}")]
        //public JsonResult RemoveDoctorOrgTaxonomy(int id)
        //{
        //    var OrgTaxonomy = _repo.All<DocOrgTaxonomy>().ToList().Where(x=>x.DocOrgTaxonomyID==id).FirstOrDefault();
        //    OrgTaxonomy.IsDeleted = true;
        //    _repo.Update<DocOrgTaxonomy>(OrgTaxonomy,true);
        //    //_OrgTaxonomy.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = "Org Taxonomy remove successfully" });
        //}

        //#endregion

        #region Social Media

        // GET: Social Media 
        [Route("SocialMedia/{id?}")]
        public ActionResult SocialMedia(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetDoctorSocialMediaList/{flag}/{id}")]
        public ActionResult GetDoctorSocialMediaList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var socialMediaInfo = _repo.ExecWithStoreProcedure<DocSocialMediaUpdateViewModel>("[spSocialMediaDoctor_Get] @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = socialMediaInfo
                .Select(x => new DocSocialMediaUpdateViewModel()
                {
                    SocialMediaId = x.SocialMediaId,
                    DoctorId = (int)x.ReferenceId,
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


        //---- Get Social Media Details
        [Route("DoctorSocialMedia/{id?}")]
        public PartialViewResult DoctorSocialMedia(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var docSocialMediaInfo = _repo.ExecWithStoreProcedure<DocSocialMediaUpdateViewModel>("spSocialMediaDoctor_GetById @SocialMediaId",
                new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = id }
                ).Select(x => new DocSocialMediaUpdateViewModel
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
                return PartialView(@"/Views/Doctor/Partial/_DoctorSocialMedia.cshtml", docSocialMediaInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgSocialMediaInfo = _repo.ExecWithStoreProcedure<DocSocialMediaUpdateViewModel>("spSocialMediaDoctor_GetByDoctorId @DoctorId",
                new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID }
                ).Select(x => new DocSocialMediaUpdateViewModel
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
                if (orgSocialMediaInfo == null)
                    orgSocialMediaInfo = new DocSocialMediaUpdateViewModel();
                return PartialView(@"/Views/Doctor/Partial/_DoctorSocialMedia.cshtml", orgSocialMediaInfo);
            }
        }


        [HttpPost, Route("AddEditDoctorSocialMedia"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorSocialMedia(DocSocialMediaUpdateViewModel model)
        {
            try
            {
                if (model.DoctorId > 0)
                {
                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSocialMedia_CreateDoctor " +
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
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Doctor social media info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Doctor Info Not Found! Should be select Doctor Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Doctor social media info saving Error.." });
            }

        }


        //-- Active DeActive Doctor Social Media
        [Route("ActiveDeActiveDoctorSocialMedia/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActiveDoctorSocialMedia(bool flag, int id)
        {
            try
            {
                bool DelFlag = false;
                if (flag == false)
                    DelFlag = true;


                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSocialMedia_ActiveDeActive @SocialMediaId, @IsActive, @IsDelete,@ModifiedBy",
                    new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = flag },
                    new SqlParameter("IsDelete", System.Data.SqlDbType.Bit) { Value = DelFlag },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor social media info has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Doctor social media info {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }

        #endregion
        #region Opening Hours
        [Route("OpeningHours/{id?}")]
        public ActionResult OpeningHours(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = id;
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }
            return View();
        }

        [Route("GetDoctorOpeningHoursList/{flag}/{id}")]
        public ActionResult GetDoctorOpeningHoursList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allDoctor = _repo.ExecWithStoreProcedure<DocOpeningHourViewModel>("[spOpeningHourDoctor_Get] @Search, @DoctorID, @UserTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("DoctorID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allDoctor
                .Select(x => new DocOpeningHoursListViewModel()
                {
                    OpeningHourID = x.OpeningHourID,
                    DoctorID = x.DoctorID,
                    WeekDay = x.WeekDay,
                    CalendarDate = x.CalendarDate,
                    StartDateTime = x.StartDateTime,
                    EndDateTime = x.EndDateTime,
                    SlotDuration = x.SlotDuration,
                    WeekDayName = getWeekDayName((int)x.WeekDay),
                    IsActive = x.IsActive,
                    IsHoliday = x.IsHoliday,
                    IsDeleted = x.IsDeleted,
                    TotalRecordCount = x.TotalRecordCount,
                    DoctorName = x.DoctorName,
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate()
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


        //---- Get Opening Hours Details
        [Route("DoctorOpeningHours/{id?}")]
        public PartialViewResult DoctorOpeningHours(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

            if (id > 0)
            {
                ViewBag.ID = id;

                // ViewBag.OrgName = GetOrganisationNameById((int)id);

                var orgInfo = _repo.ExecWithStoreProcedure<DoctorOpeningHoursUpdateViewModel>("spDoctorOpeningHour_GetByDoctorID @DoctorId",
                    new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = id }
                    ).FirstOrDefault();

                var info2 = _repo.ExecWithStoreProcedure<DocOpeningHoursViewModel>("spDoctorOpeningHour_GetByDoctorID @DoctorId",
                    new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = id }
                    ).ToList();


                ViewBag.Items = info2;
                ViewBag.ItemCount = info2.Count;

                orgInfo.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);

                return PartialView(@"/Views/Doctor/Partial/_DoctorOpeningHours.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = Convert.ToInt32(Session["DoctorSearch"]);
                var orgInfo = new DoctorOpeningHoursUpdateViewModel();
                ViewBag.OrgName = null;
                orgInfo.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);
                orgInfo = _repo.ExecWithStoreProcedure<DoctorOpeningHoursUpdateViewModel>("spDoctorOpeningHour_GetByDoctorID @DoctorId",
                 new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = orgInfo.DoctorID }
                 ).FirstOrDefault();
                if (orgInfo == null)
                {
                    orgInfo = new DoctorOpeningHoursUpdateViewModel();
                    orgInfo.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);
                }
                var info2 = _repo.ExecWithStoreProcedure<DocOpeningHoursViewModel>("spDoctorOpeningHour_GetByDoctorID @DoctorId",
                    new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = orgInfo.DoctorID }
                    ).ToList();


                ViewBag.Items = info2;
                ViewBag.ItemCount = info2.Count;

                return PartialView(@"/Views/Doctor/Partial/_DoctorOpeningHours.cshtml", orgInfo);
            }
        }


        //--- Update Doctor Opening Hours

        [HttpPost, Route("AddEditDoctorOpeningHours"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorOpeningHours(DoctorOpeningHoursUpdateViewModel model)
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
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                              new SqlParameter("Comments", System.Data.SqlDbType.NVarChar) { Value = (object)model.Comments[i] ?? DBNull.Value }
                                          );

                        continue;
                    }

                    return Json(new JsonResponse() { Status = 1, Message = "Doctor Opening Hours info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Doctor Info Not Found! Should be select Doctor Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Doctor OpeningHours info saving Error.." });
            }

        }


        //-- Delete Doctor Featured
        [Route("DeleteDoctorOpeningHours/{id}")]
        [HttpPost]
        public JsonResult DeleteDoctorOpeningHours(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrgOpeningHour_Delete @OpeningHourID, @ModifiedBy",
                    new SqlParameter("OpeningHourID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor opening hours info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Doctor opening hours info deleted Error.." });
            }
        }


        #endregion

        #region Doctor Images

        // GET: Images 
        [Route("Images/{id?}")]
        public ActionResult Images(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                //var DoctorId = Convert.ToString(Session["DoctorSearch"]);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["DoctorSearch"])))
                {
                    var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                    var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                    Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
                }
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }
            return View();

        }

        [Route("GetDoctorImageList/{flag}/{id}")]
        public ActionResult GetDoctorImageList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            ViewBag.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);
            var result = _repo.ExecWithStoreProcedure<OrgSiteImageViewModel>("spDoctorSiteImage_Get @Search, @ReferenceId, @UserTypeID, @PageIndex, @PageSize, @Sort",
                   new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                   new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID },
                   new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },

                   new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                   new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                   new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                   ).Select(x => new DocSiteImageListViewModel
                   {
                       SiteImageId = x.SiteImageId,
                       DoctorName = x.ReferenceName,
                       DoctorId = x.ReferenceId,
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

        //---- Get Site Image Details

        public PartialViewResult DoctorSiteImages(int? id)
        {
            ViewBag.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgSiteImageViewModel>("spDoctorSiteImage_GetById @SiteImageId",
                    new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new DocSiteImageUpdateViewModel
                    {
                        SiteImageId = x.SiteImageId,
                        DoctorId = x.ReferenceId,
                        Name = x.Name,
                        ImagePath = x.ImagePath,
                        IsProfile = x.IsProfile,
                        IsActive = x.IsActive
                    }).First();

                return PartialView(@"/Views/Doctor/Partial/_DoctorImages.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new DocSiteImageUpdateViewModel();
                return PartialView(@"/Views/Doctor/Partial/_DoctorImages.cshtml", orgInfo);
            }
        }
        public PartialViewResult DoctorViewSiteImages(int? id)
        {
            ViewBag.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgSiteImageViewModel>("spDoctorSiteImage_GetById @SiteImageId",
                    new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new DocSiteImageUpdateViewModel
                    {
                        SiteImageId = x.SiteImageId,
                        DoctorId = x.ReferenceId,
                        Name = x.Name,
                        ImagePath = x.ImagePath,
                        IsProfile = x.IsProfile,
                        IsActive = x.IsActive
                    }).First();
                orgInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorImages.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new DocSiteImageUpdateViewModel();
                orgInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorImages.cshtml", orgInfo);
            }
        }

        //---- Add Edit Doctor Site Images

        [HttpPost, Route("AddEditDoctorSiteImage"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorSiteImage(DocSiteImageUpdateViewModel model, HttpPostedFileBase Image1)
        {
            ViewBag.DoctorID = Convert.ToInt32(Session["DoctorSearch"]);
            model.DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
            try
            {
                if (model.DoctorId > 0)
                {
                    string imagePath = "";

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/DoctorSiteImages/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/DoctorSiteImages"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "Doctor-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/DoctorSiteImages"), newImageName);
                        Image1.SaveAs(path);

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
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
                                          new SqlParameter("ImagePath", System.Data.SqlDbType.VarChar) { Value = (object)(imagePath) ?? DBNull.Value },
                                          new SqlParameter("IsProfile", System.Data.SqlDbType.Bit) { Value = (object)model.IsProfile ?? DBNull.Value },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeID },
                                          new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = model.Name }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Doctor site image info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Doctor Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Doctor site image info saving Error.." });
            }

        }

        //-- Delete Doctor Site Image
        [Route("DeleteDoctorSiteImage/{id}/{ImgName}")]
        [HttpPost]
        public JsonResult DeleteDoctorSiteImage(int id, string ImgName)
        {
            try
            {
                if (ImgName != null)
                {
                    var path = Server.MapPath("~/Uploads/DoctorSiteImages/" + ImgName);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizatonSiteImage_Remove @SiteImageId, @ModifiedBy",
                    new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor site image has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Doctor site image deleted Error.." });
            }
        }

        #endregion

        #region Doctor Address

        // GET: StateLicense 
        [Route("DoctorAddress/{id}")]
        public ActionResult DoctorAddress(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }
        [HttpPost]
        public JsonResult GetCityByZipCode(string Prefix, string ZipCode)
        {
            var cityList = _repo.ExecWithStoreProcedure<CityStateInfoByZipCodeViewModel>("spGetCityStateCityByZipCodeAutoComplete @ZipCode, @City",
                new SqlParameter("ZipCode", System.Data.SqlDbType.VarChar) { Value = ZipCode },
                new SqlParameter("City", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(cityList.ToList(), JsonRequestBehavior.AllowGet);
        }


        //--Get Cities By ZipCode
        [HttpPost]
        public JsonResult GetCityStateZipCodeInfo(string ZipCode, string City, string State, string Country)
        {
            var Info = _repo.ExecWithStoreProcedure<CityStateInfoByZipCodeViewModel>("spGetCityStateZip_All");

            Info = Info.Where(x => x.ZipCode == ZipCode && x.City == City && x.State == State && x.Country == Country);


            return Json(Info.ToList(), JsonRequestBehavior.AllowGet);
        }


        //--Get Reference Address By ReferenceId AND UserTypeID
        [HttpPost]
        public JsonResult GetReferenceAddress(int ReferenceId, int UserTypeId)
        {
            var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
                new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ReferenceId },
                new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypeId }
                );


            return Json(addressList.ToList(), JsonRequestBehavior.AllowGet);
        }


        //--Get Insurance Plan By Type Id
        [HttpPost]
        public JsonResult GetInsurancePlanByTypeId(int TypeId)
        {
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownListByType_Get @InsuranceTypeId",
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        //--Get Organization Insurance Plan By Type Id
        [HttpPost]
        public JsonResult GetOrgInsurancePlanByTypeId(int ReferenceId, int TypeId)
        {
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spOrgInsurancePlanDropDownListDoctor_Get @DoctorId, @InsuranceTypeId",
                new SqlParameter("DoctorID", System.Data.SqlDbType.Int) { Value = ReferenceId },
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInsurancePlanBySlotID(int ReferenceId, int TypeId)
        {
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownListBySlot_Get @DoctorId, @InsuranceTypeId",
                new SqlParameter("DoctorID", System.Data.SqlDbType.Int) { Value = ReferenceId },
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetOrgAddressById(int ReferenceId, int TypeId)
        {
            var planList = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModel>("sprGetOrganiationAddresses @OrganizationId",
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ReferenceId }

                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }


        //--Organisation Organisation Name AutoComplete
        [HttpPost]
        public JsonResult GetOrganisationName(string Prefix, string type)
        {
            if (string.IsNullOrEmpty(type))
                type = "";

            var orgList = _repo.ExecWithStoreProcedure<OrganisationAutoCompleteViewModel>("spGetOrganisationInfoAutoComplete @strType, @OrganisationName, @OrganisationTypeID",
                new SqlParameter("strType", System.Data.SqlDbType.VarChar) { Value = type },
                new SqlParameter("OrganisationName", System.Data.SqlDbType.VarChar) { Value = Prefix },
                new SqlParameter("OrganisationTypeID", System.Data.SqlDbType.Int) { Value = 1005 }
                );


            return Json(orgList.ToList(), JsonRequestBehavior.AllowGet);
        }

        //--- State Auto Complete
        [HttpPost]
        public JsonResult GetStatesAutocomplete(string Prefix)
        {
            var stateList = _repo.ExecWithStoreProcedure<StateCompleteViewModel>("spCityStateZip_AllState_AutoComplete @Prefix",
                new SqlParameter("Prefix", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(stateList.ToList(), JsonRequestBehavior.AllowGet);
        }

        //--- Return CityStateInfoById
        public string GetCityStateInfoById(int id, string type)
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

        [Route("GetDoctorOfficeLocationList/{flag}/{id}")]
        public ActionResult GetDoctorOfficeLocationList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allDoctorAddress = _repo.ExecWithStoreProcedure<OrganisationAddressViewModel>("spAddressDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allDoctorAddress
                .Select(x => new DoctorAddressListViewModel()
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
        [HttpPost]
        public JsonResult GetZipCode(string Prefix)
        {
            var zipcodes = _repo.ExecWithStoreProcedure<CityStateZipCodeViewModel>("spGetCityStateZipCodesAutoComplete @ZipCode",
                new SqlParameter("ZipCode", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(zipcodes.ToList(), JsonRequestBehavior.AllowGet);
        }
        [Route("ViewAddress/{id?}")]
        public PartialViewResult ViewAddress(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

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
                    ).Select(x => new DoctorAddressViewModel
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
                orgAddressInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorAddress.cshtml", orgAddressInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new DoctorAddressViewModel();
                orgAddressInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorAddress.cshtml", orgAddressInfo);
            }
        }
        //---- Get Address Details
        [Route("Address/{id?}")]
        public PartialViewResult Address(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

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
                    ).Select(x => new DoctorAddressViewModel
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
                return PartialView(@"/Views/Doctor/Partial/_DoctorAddress.cshtml", orgAddressInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new DoctorAddressViewModel();
                return PartialView(@"/Views/Doctor/Partial/_DoctorAddress.cshtml", orgAddressInfo);
            }
        }




        [HttpPost, Route("AddEditDoctorAddress"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorAddress(DoctorAddressViewModel model)
        {
            try
            {
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
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                      new SqlParameter("Phone", System.Data.SqlDbType.NVarChar) { Value = (object)model.Phone ?? DBNull.Value },
                                      new SqlParameter("Fax", System.Data.SqlDbType.NVarChar) { Value = (object)model.Fax ?? DBNull.Value },
                                      new SqlParameter("Email", System.Data.SqlDbType.NVarChar) { Value = (object)model.Email ?? DBNull.Value },
                                      new SqlParameter("WebSite", System.Data.SqlDbType.VarChar) { Value = (object)model.WebSite ?? DBNull.Value },
                                      new SqlParameter("Lat", System.Data.SqlDbType.Decimal) { Value = (object)model.Lat ?? DBNull.Value },
                                      new SqlParameter("Lon", System.Data.SqlDbType.Decimal) { Value = (object)model.Lon ?? DBNull.Value },
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeID },
                                      new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = model.AddressId > 0 ? User.Identity.GetUserId<int>() : (object)DBNull.Value }
                                  );

                return Json(new JsonResponse() { Status = 1, Message = "Doctor address info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Doctor address info saving Error.." });
            }

        }

        [Route("DeleteDoctorAddress/{id}")]
        [HttpPost]
        public JsonResult DeleteDoctorAddress(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_Delete @AddressId, @ModifiedBy",
                    new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor Address has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Doctor Address deleted Error.." });
            }


        }


        #endregion
        #region Doctor State License
        // GET: StateLicense 
        [Route("DoctorStateLicense/{id?}")]
        public ActionResult DoctorStateLicense(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctor = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctor != null ? doctor.FirstName + " " + doctor.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }
        [HttpPost]
        public JsonResult GetTaxonomy(string Prefix)
        {
            var taxonomyList = _repo.ExecWithStoreProcedure<TaxonomyAutoCompleteDropDownViewModel>("spGetTaxonomyCodesAutoComplete @Taxonomy_Code",
                new SqlParameter("Taxonomy_Code", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(taxonomyList.ToList(), JsonRequestBehavior.AllowGet);
        }

        //--Taxonomy Parent Specialization
        [HttpPost]
        public JsonResult GetTaxonomyParentSpecialization(string Prefix)
        {
            var taxonomyList = _repo.ExecWithStoreProcedure<TaxonomyAutoCompleteDropDownViewModel>("spGetTaxonomyHierarchy @Search",
                new SqlParameter("Search", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(taxonomyList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [Route("GetDoctorStateLicenseList/{flag}/{id}")]
        public ActionResult GetDoctorStateLicenseList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var result = _repo.ExecWithStoreProcedure<DoctorStateLicenseViewModel>("spDocOrgStateLicensesDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = Convert.ToString(Session["DoctorSearch"]) },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    ).Select(x => new DoctorStateLicenseListViewModel
                    {
                        DocOrgStateLicenseId = x.DocOrgStateLicense,
                        ReferenceName = x.ReferenceName,
                        HealthCareProviderTaxonomyCode = x.HealthCareProviderTaxonomyCode,
                        ProviderLicenseNumber = x.ProviderLicenseNumber,
                        ProviderLicenseNumberStateCode = x.ProviderLicenseNumberStateCode,
                        HealthcareProviderPrimaryTaxonomySwitch = x.HealthcareProviderPrimaryTaxonomySwitch,
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

        //---- Get State License Details
        [Route("StateLicense/{id?}")]
        public PartialViewResult StateLicense(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

            var stateList = _repo.ExecWithStoreProcedure<StateDropDownViewModel>("spCityStateZip_US_AllState");
            ViewBag.stateList = new SelectList(stateList.OrderBy(o => o.State), "State", "State");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgLicenseInfo = _repo.ExecWithStoreProcedure<DoctorStateLicenseViewModel>("spDocOrgStateLicensesDoctor_GetById @DocOrgStateLicense",
                    new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new DoctorStateLicenseUpdateViewModel
                    {
                        DocOrgStateLicenseId = x.DocOrgStateLicense,
                        DoctorId = x.ReferenceId,
                        UserTypeID = x.UserTypeID,
                        HealthCareProviderTaxonomyCode = x.HealthCareProviderTaxonomyCode,
                        ProviderLicenseNumber = x.ProviderLicenseNumber,
                        ProviderLicenseNumberStateCode = x.ProviderLicenseNumberStateCode,
                        HealthcareProviderPrimaryTaxonomySwitch = x.HealthcareProviderPrimaryTaxonomySwitch,
                        IsDeleted = x.IsDeleted,
                        IsActive = x.IsActive
                    }).First();

                ViewBag.stateList = new SelectList(stateList.OrderBy(o => o.State), "State", "State", orgLicenseInfo.ProviderLicenseNumberStateCode);
                return PartialView(@"/Views/Doctor/Partial/_DoctorStateLicense.cshtml", orgLicenseInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgLicenseInfo = new DoctorStateLicenseUpdateViewModel();
                return PartialView(@"/Views/Doctor/Partial/_DoctorStateLicense.cshtml", orgLicenseInfo);
            }
        }
        [Route("StateViewLicense/{id?}")]
        public PartialViewResult StateViewLicense(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

            var stateList = _repo.ExecWithStoreProcedure<StateDropDownViewModel>("spCityStateZip_US_AllState");
            ViewBag.stateList = new SelectList(stateList.OrderBy(o => o.State), "State", "State");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgLicenseInfo = _repo.ExecWithStoreProcedure<DoctorStateLicenseViewModel>("spDocOrgStateLicensesDoctor_GetById @DocOrgStateLicense",
                    new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new DoctorStateLicenseUpdateViewModel
                    {
                        DocOrgStateLicenseId = x.DocOrgStateLicense,
                        DoctorId = x.ReferenceId,
                        UserTypeID = x.UserTypeID,
                        HealthCareProviderTaxonomyCode = x.HealthCareProviderTaxonomyCode,
                        ProviderLicenseNumber = x.ProviderLicenseNumber,
                        ProviderLicenseNumberStateCode = x.ProviderLicenseNumberStateCode,
                        HealthcareProviderPrimaryTaxonomySwitch = x.HealthcareProviderPrimaryTaxonomySwitch,
                        IsDeleted = x.IsDeleted,
                        IsActive = x.IsActive
                    }).First();
                orgLicenseInfo.IsViewMode = true;

                ViewBag.stateList = new SelectList(stateList.OrderBy(o => o.State), "State", "State", orgLicenseInfo.ProviderLicenseNumberStateCode);
                return PartialView(@"/Views/Doctor/Partial/_DoctorStateLicense.cshtml", orgLicenseInfo);
            }
            else
            {
                ViewBag.ID = 0;

                var orgLicenseInfo = new DoctorStateLicenseUpdateViewModel();
                orgLicenseInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorStateLicense.cshtml", orgLicenseInfo);
            }
        }

        //-- Active DeActive Doctor State License
        [Route("ActiveDeActiveDoctorStateLicense/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActiveDoctorStateLicense(bool flag, int id)
        {
            try
            {
                bool DelFlag = false;
                if (flag == false)
                    DelFlag = true;


                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgStateLicenses_ActiveDeActive @DocOrgStateLicense, @IsActive, @IsDelete,@ModifiedBy",
                    new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = flag },
                    new SqlParameter("IsDelete", System.Data.SqlDbType.Bit) { Value = DelFlag },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor State License has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Doctor State License {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }

        //--Update State License

        [HttpPost, Route("AddEditDoctorStateLicense"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorStateLicense(DoctorStateLicenseUpdateViewModel model)
        {

            try
            {
                model.DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
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
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive ?? false },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Doctor State License info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Doctor State License info saving Error.." });
            }

        }

        #endregion
        #region DocOrgTaxonomy

        [Route("DoctorTaxonomy/{id}")]
        // GET: Taxonomy 
        public ActionResult DoctorTaxonomy(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetDoctorTaxonomyList/{flag}/{id}")]
        public ActionResult GetDoctorTaxonomyList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var Info = _repo.ExecWithStoreProcedure<DoctorTaxonomyViewModel>("spDocOrgTaxonomyDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },

                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = Info
                .Select(x => new DoctorTaxonomyListViewModel()
                {
                    DocOrgTaxonomyID = x.DocOrgTaxonomyID,
                    TaxonomyID = x.TaxonomyID,
                    ReferenceID = x.ReferenceID,
                    DoctorId = x.ReferenceID,
                    Taxonomy_Code = x.Taxonomy_Code,
                    Specialization = x.Specialization,

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

        [Route("ViewTaxonomy/{id?}")]
        public PartialViewResult ViewTaxonomy(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgTaxonomyInfo = _repo.ExecWithStoreProcedure<DoctorTaxonomyViewModel>("spDocOrgTaxonomyDoctor_GetById @DocOrgTaxonomyID",
                    new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new DoctorTaxonomyUpdateViewModel
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
                orgTaxonomyInfo.IsViewMode = true;

                return PartialView(@"/Views/Doctor/Partial/_DoctorTaxonomy.cshtml", orgTaxonomyInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgTaxonomyInfo = new DoctorTaxonomyUpdateViewModel();
                orgTaxonomyInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorTaxonomy.cshtml", orgTaxonomyInfo);
            }
        }
        //---- Get Taxonomy Details
        [Route("Taxonomy/{id?}")]
        public PartialViewResult Taxonomy(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgTaxonomyInfo = _repo.ExecWithStoreProcedure<DoctorTaxonomyViewModel>("spDocOrgTaxonomyDoctor_GetById @DocOrgTaxonomyID",
                    new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new DoctorTaxonomyUpdateViewModel
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


                return PartialView(@"/Views/Doctor/Partial/_DoctorTaxonomy.cshtml", orgTaxonomyInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgTaxonomyInfo = new DoctorTaxonomyUpdateViewModel();
                return PartialView(@"/Views/Doctor/Partial/_DoctorTaxonomy.cshtml", orgTaxonomyInfo);
            }
        }


        //-- Add/Edit Taxonomy

        [HttpPost, Route("AddEditDoctorTaxonomy"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorTaxonomy(DoctorTaxonomyUpdateViewModel model)
        {
            try
            {
                model.DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
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
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                      new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 2 }
                                  );

                return Json(new JsonResponse() { Status = 1, Message = "Doctor Taxonomy info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Doctor Taxonomy info saving Error.." });
            }

        }


        //-- Active DeActive Doctor Taxonomy
        [Route("ActiveDeActiveDoctorTaxonomy/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActiveDoctorTaxonomy(bool flag, int id)
        {
            try
            {
                bool DelFlag = false;
                if (flag == false)
                    DelFlag = true;


                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgTaxonomy_ActiveDeActive @DocOrgTaxonomyID, @IsActive, @IsDelete, @ModifiedBy",
                    new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = flag },
                    new SqlParameter("IsDelete", System.Data.SqlDbType.Bit) { Value = DelFlag },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor Taxonomy info has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Doctor Taxonomy info {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }

        #endregion

        #region Booking
        public JsonResult ValidateBookingDate(string SlotDate)
        {
            bool status = true;

            if (!string.IsNullOrEmpty(SlotDate))
            {

                //                string[] strArr = SlotDate.Split('-');

                //string strFormattedDate = strArr[2] + "-" + strArr[1] + "-" + strArr[0];
                DateTime dt1 = DateTimeOffset.ParseExact(SlotDate, "MM/dd/yyyy", null).UtcDateTime.Date;//DateTimeOffset.Parse(strFormattedDate).UtcDateTime;
                if (dt1 < DateTime.UtcNow.Date)
                    status = false;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBookingTime(string SlotTime, string SlotDate, int? SlotId)
        {
            bool status = true;
            int countX = 0;
            int DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
            if (!string.IsNullOrEmpty(SlotTime) && !string.IsNullOrEmpty(SlotDate))
            {
                if (SlotId > 0)
                {
                    var result = _repo.All<Slot>().Where(x => x.ReferenceId == DoctorId && x.SlotDate == SlotDate && x.SlotTime == SlotTime && x.SlotId != SlotId && x.IsDeleted == false);
                    countX = result.Count();
                }
                else
                {
                    var result = _repo.All<Slot>().Where(x => x.ReferenceId == DoctorId && x.SlotDate == SlotDate && x.SlotTime == SlotTime && x.IsDeleted == false);
                    countX = result.Count();
                }

                if (countX > 0)
                    status = false;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
        [Route("Booking/{id?}")]
        // GET: Booking
        public ActionResult Booking(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }


            return View();
        }


        [Route("GetDoctorBookingList/{id}")]
        public ActionResult GetDoctorBookingList(JQueryDataTableParamModel param, int? id)
        {
                                                                            
            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spDocOrgBookingDoctor_Get @Search, @UserTypeID, @ReferenceId, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },

                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = orgInfo
                .Select(x => new DoctorBookingListViewModel() 
                {
                    SlotId = x.SlotId,
                    DoctorId = x.DoctorId,
                    DoctorName = x.DoctorName + " [" + x.Credential + "]",
                    OrganisationName =x.OrganisationName,
                    OrganisationAddress = x.OrganisationAddress,
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

        [HttpPost]
        public JsonResult GetPatientName(string Prefix)
        {

            var patientList = _repo.ExecWithStoreProcedure<PatientNameAutoCompleteViewModel>("spGetPatientInfoAutoComplete @Email, @PhoneNumber, @FirstName, @LastName",
                new SqlParameter("Email", System.Data.SqlDbType.VarChar) { Value = Prefix },
                new SqlParameter("PhoneNumber", System.Data.SqlDbType.VarChar) { Value = Prefix },
                new SqlParameter("FirstName", System.Data.SqlDbType.VarChar) { Value = Prefix },
                new SqlParameter("LastName", System.Data.SqlDbType.VarChar) { Value = Prefix }
                ).Select(x => new PatientNameAutoCompleteSummaryViewModel()
                {
                    PatientId = x.PatientId,
                    UserId = x.UserId,
                    FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber
                });


            return Json(patientList.ToList(), JsonRequestBehavior.AllowGet);
        }

        //---- Get Booking Details
        [Route("DoctorBooking/{id?}")]
        public PartialViewResult DoctorBooking(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];


            //var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
            //    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID },
            //    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 }
            //    ).Select(x => new OrgAddressDropdownViewModel()
            //    {
            //        AddressId = x.AddressId,
            //        FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country
            //    });
            var addressList = new List<OrgAddressDropdownViewModel>();
            ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress");

            var OrganizationList = _repo.ExecWithStoreProcedure<OrgDoctorsDropDownViewModel>("GetOrganizationByDoctor @DoctorId",
               new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID }
               ).Select(x => new OrgDoctorsDropDownViewModel()
               {
                   OrganizationId = x.OrganizationId,
                   DisplayName = x.DisplayName
               });

            ViewBag.OrganizationList = new SelectList(OrganizationList.OrderBy(o => o.DisplayName), "OrganizationId", "DisplayName");

            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get");
            ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name");

            //  var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get");
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get").ToList();
            //var planList = new List<InsurancePlanDropDownViewModel>();
            ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spDocOrgBookingDoctor_GetById @SlotId, @DoctorId",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID }
                    ).Select(x => new DoctorBookingUpdateViewModel
                    {
                        SlotId = x.SlotId,
                        OrganisationId = x.OrganisationId.HasValue ? x.OrganisationId.Value : 0,
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
                        InsurancePlanId = x.InsurancePlanId.HasValue ? x.InsurancePlanId.Value : 0,
                        InsuranceTypeId = x.InsuranceTypeId.HasValue ? x.InsuranceTypeId.Value : 0
                    }).First();
                var addresses = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModel>("sprGetOrganiationAddresses @OrganizationId",
               new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = orgInfo.OrganisationId }

               ).ToList();

                ViewBag.addressList = new SelectList(addresses.OrderBy(o => o.AddressId), "AddressId", "OrganizationAddress", orgInfo.AddressId);
                ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name", orgInfo.InsurancePlanId);
                return PartialView(@"/Views/Doctor/Partial/_DoctorBooking.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new DoctorBookingUpdateViewModel();
                return PartialView(@"/Views/Doctor/Partial/_DoctorBooking.cshtml", orgInfo);
            }
        }

        //---- Get Booking Details
        [Route("DoctorViewBooking/{id?}")]
        public PartialViewResult DoctorViewBooking(int? id)
        {
            ViewBag.DoctorID = Session["DoctorSearch"];


            //var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
            //    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID },
            //    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 }
            //    ).Select(x => new OrgAddressDropdownViewModel()
            //    {
            //        AddressId = x.AddressId,
            //        FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country
            //    });
            var addressList = new List<OrgAddressDropdownViewModel>();
            ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress");

            var OrganizationList = _repo.ExecWithStoreProcedure<OrgDoctorsDropDownViewModel>("GetOrganizationByDoctor @DoctorId",
               new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID }
               ).Select(x => new OrgDoctorsDropDownViewModel()
               {
                   OrganizationId = x.OrganizationId,
                   DisplayName = x.DisplayName
               });

            ViewBag.OrganizationList = new SelectList(OrganizationList.OrderBy(o => o.DisplayName), "OrganizationId", "DisplayName");

            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get");
            ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name");

            //  var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get");
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get").ToList();
            //var planList = new List<InsurancePlanDropDownViewModel>();
            ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spDocOrgBookingDoctor_GetById @SlotId, @DoctorId",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("DoctorId", System.Data.SqlDbType.Int) { Value = ViewBag.DoctorID }
                    ).Select(x => new DoctorBookingUpdateViewModel
                    {
                        SlotId = x.SlotId,
                        OrganisationId = x.OrganisationId.HasValue ? x.OrganisationId.Value : 0,
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
                        InsurancePlanId = x.InsurancePlanId.HasValue ? x.InsurancePlanId.Value : 0,
                        InsuranceTypeId = x.InsuranceTypeId.HasValue ? x.InsuranceTypeId.Value : 0
                    }).First();
                var addresses = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModel>("sprGetOrganiationAddresses @OrganizationId",
               new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = orgInfo.OrganisationId }

               );

                ViewBag.addressList = new SelectList(addresses.OrderBy(o => o.AddressId), "AddressId", "OrganizationAddress", orgInfo.AddressId);
                ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name", orgInfo.InsurancePlanId);
                orgInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorBooking.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new DoctorBookingUpdateViewModel();
                orgInfo.IsViewMode = true;
                return PartialView(@"/Views/Doctor/Partial/_DoctorBooking.cshtml", orgInfo);
            }
        }


        //--- Add Update Doctor Booking Info

        [HttpPost, Route("AddEditDoctorBooking"), ValidateAntiForgeryToken]
        public JsonResult AddEditDoctorBooking(DoctorBookingUpdateViewModel model)
        {
            try
            {
                if (model.OrganisationId > 0)
                {
                    model.DoctorId = Convert.ToInt32(Session["DoctorSearch"]);
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
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = !string.IsNullOrEmpty(model.Description) ? model.Description : "" },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 2 }

                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Doctor booking info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Doctor Info Not Found! Should be select Doctor Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Doctor booking info saving Error.." });
            }

        }

        //-- Delete Doctor Booking
        [Route("DeleteDoctorBooking/{id}")]
        [HttpPost]
        public JsonResult DeleteDoctorBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Doctor Booking info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Doctor Booking info deleted Error.." });
            }
        }

        #endregion


        #region Review

        [Route("Reviews/{id?}/{flag?}")]
        public ActionResult Reviews(int id = 0)
        {
            if (User.IsInRole("Admin"))
            {
                Session["DoctorSearch"] = id;
                ViewBag.DoctorID = Session["DoctorSearch"];
                var DoctorId = id;
                var doctors = _repo.Find<Doctor>(x => x.DoctorId == DoctorId);
                Session["DoctorName"] = doctors != null ? doctors.FirstName + " " + doctors.LastName : "";
            }
            else
            {
                if (User.IsInRole("Doctor"))
                {
                    Session["DoctorSearch"] = GetDoctorId();
                    ViewBag.DoctorID = GetDoctorId();

                }
                else
                {
                    return View("Error");
                }
            }
            ViewBag.DocId = id;
            return View();
        }

        public ActionResult GetReviewList(JQueryDataTableParamModel param, int id = 0, bool flag = false)
        {
            var model = new SearchParamModel();
            //model.PageIndex = pageIndex;
            //model.SearchBox = param.sSearch;
            //model.PageSize = param.iDisplayLength;
            var data = GetReviewSearchResult(model, id);

            return Json(new
            {
                param.sEcho,
                iTotalRecords = data.TotalRecord,
                iTotalDisplayRecords = data.TotalRecord,
                aaData = data.ReviewProviderList
            }, JsonRequestBehavior.AllowGet);
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
                    new SqlParameter("@UserTypeID",SqlDbType.Int) {Value = UserTypes.Doctor},
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
            parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createReview.DoctorId });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = createReview.Description });
            parameters.Add(new SqlParameter("Rating", SqlDbType.Int) { Value = createReview.Rating });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createReview.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = null });
            parameters.Add(new SqlParameter("Doctor_DoctorId", System.Data.SqlDbType.Int) { Value = createReview.DoctorId });
            parameters.Add(new SqlParameter("SeniorCare_SeniorCareId", System.Data.SqlDbType.Int) { Value = null });
            parameters.Add(new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = UserTypes.Doctor });
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

        [Route("DeleteDoctorReview/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyReview(int id)
        {
            try
            {

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spReview_Remove @ReviewId, @ModifiedBy",
                    new SqlParameter("ReviewId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy review has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy review deleted Error.." });
            }
        }


        #endregion
    }
}
