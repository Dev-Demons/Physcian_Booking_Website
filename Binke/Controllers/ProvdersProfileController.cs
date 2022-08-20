using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Binke.App_Helpers;
using Binke.Models;
using Binke.Utility;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Configuration;
using Binke.ViewModels;
using System.Globalization;

namespace Binke.Controllers
{
    public class ProvdersProfileController : Controller
    {
        private readonly IDoctorService _doctor;
        private ApplicationUserManager _userManager;
        private IPatientService _patient;
        private readonly IFacilityService _facility;
        private readonly ICityService _city;
        private readonly IStateService _state;
        private readonly IInsuranceTypeRepository _insuranceType;
        private readonly ISlotService _slot;
        private readonly ICityStateZipService _cityStateZip;

        public ProvdersProfileController(IDoctorService doctor, ApplicationUserManager userManager, IPatientService patient,
            IFacilityService facility, ICityService city, IStateService state, IInsuranceTypeRepository insuranceType, ISlotService slot
            , ICityStateZipService cityStateZip
            )
        {
            _doctor = doctor;
            _userManager = userManager;
            _patient = patient;
            _city = city;
            _state = state;
            _insuranceType = insuranceType;
            _slot = slot;
            _cityStateZip = cityStateZip;
            _facility = facility;
        }
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }
        [Route("Profile/Facility/{OrganisationName?}-{npi?}")]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Facility(string npi)
        {
            try
            {
                var orgIds = _facility.SQLQuery<int>("select OrganisationId from Organisation where npi = @npi", new SqlParameter("@npi", System.Data.SqlDbType.VarChar) { Value = npi }).ToList();
                var orgId = 0;
                if (orgIds.Count > 0)
                    orgId = orgIds.First();
                Doctyme.Model.ViewModels.ProfileViewModel _model = new Doctyme.Model.ViewModels.ProfileViewModel();
                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetFacilityProfileDetails + " " + orgId);
                var orgProfile = Common.ConvertDataTable<Doctyme.Model.ViewModels.ProfileViewModel>(ds.Tables[0]).FirstOrDefault();
                if (orgProfile == null) orgProfile = new ProfileViewModel();
                orgProfile.lstReviews = Common.ConvertDataTable<Reviews>(ds.Tables[1]).ToList();
                orgProfile.lstOrgAmenOpt = Common.ConvertDataTable<OrganizationAmenityOptions>(ds.Tables[2]).ToList();
                orgProfile.lstSiteImages = Common.ConvertDataTable<SiteImages>(ds.Tables[3]).ToList();
                orgProfile.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[4]).ToList();
                orgProfile.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[5]).ToList();
                orgProfile.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[6]).ToList();
                orgProfile.SocialMediaLinks = Common.ConvertDataTable<SocialMedia>(ds.Tables[7]).ToList();
                orgProfile.lstCost = Common.ConvertDataTable<Cost>(ds.Tables[8]).ToList();

                orgProfile.Maxslots = orgProfile.lstslotTimes.Count > 0 ? Convert.ToInt32(orgProfile.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
                orgProfile.MaxDays = 1;
                orgProfile.CalenderDatesCount = orgProfile.lstslotsDates.Count > 0 ? orgProfile.lstslotsDates.FirstOrDefault().MaxCount : 0;
                //orgProfile.CalenderDatesCount = 7;
                var lat1 = GetLocationbyIP().Item1;//Added by Reena
                var long1 = GetLocationbyIP().Item2;//Added by Reena
                                                    //var organisationIds = _doctor.GetOrganisationIdsFromTypeId(1006, lat1, long1);//Added by Reena
                                                    //orgProfile.lstProviderAdvertisements = _doctor.GetAdvertisementsFromSearchText(organisationIds, 1006);//Added by Reena
                                                    ////orgProfile.lstProviderAdvertisements = Common.ConvertDataTable<ProviderAdvertisements>(ds.Tables[9]).ToList();//Added by Reena
                var advList = _patient.ExecWithStoreProcedure<ProviderAdvertisements>("spGetAdvertisements_ByOrgTypeId @userTypeId",
                   new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = 1006 }).ToList();
                orgProfile.lstProviderAdvertisements = _doctor.GetDistanceMilebyOrgIds(advList, lat1, long1);
                if (ConfigurationManager.AppSettings["AgoraKey"] == null)
                {
                    ViewBag.AgoraKey = "0f8f282b530c4232b289867cda582737";
                }
                else
                {
                    ViewBag.AgoraKey = ConfigurationManager.AppSettings["AgoraKey"].ToString();
                }
                var logoPath = _facility.GetById(orgProfile.OrganisationId);
                orgProfile.ImagePath = logoPath != null ? "/Uploads/FacilitySiteImages/" + logoPath.LogoFilePath : "/Uploads/ProfilePic/no_picture.png";
                orgProfile.NPI = logoPath.NPI;
                return View(orgProfile);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Facility-GET");
                return View(new ProfileViewModel());
            }
        }
        [Route("Profile/Pharmacy/{OrganisationName?}-{npi?}")]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Pharmacy(string OrganisationName, string npi)
        {
            try
            {
                var orgIds = _facility.SQLQuery<int>("select OrganisationId from Organisation where npi = @npi", new SqlParameter("@npi", System.Data.SqlDbType.VarChar) { Value = npi }).ToList();
                var orgId = 0;
                if (orgIds.Count > 0)
                {
                    orgId = orgIds.First();
                }
                else
                {
                    if (!string.IsNullOrEmpty(npi))
                    {
                        orgId = Convert.ToInt32(npi);
                    }
                }
                Doctyme.Model.ViewModels.ProfileViewModel _model = new Doctyme.Model.ViewModels.ProfileViewModel();
                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetPharmacyProfileDetails + " " + orgId);
                Doctyme.Model.ViewModels.ProfileViewModel orgProfile = Common.ConvertDataTable<Doctyme.Model.ViewModels.ProfileViewModel>(ds.Tables[0]).FirstOrDefault();
                if(orgProfile == null)
                {
                    orgProfile = new Doctyme.Model.ViewModels.ProfileViewModel();
                }
                orgProfile.lstReviews = Common.ConvertDataTable<Reviews>(ds.Tables[1]).ToList();
                orgProfile.lstOrgAmenOpt = Common.ConvertDataTable<OrganizationAmenityOptions>(ds.Tables[2]).ToList();
                orgProfile.lstSiteImages = Common.ConvertDataTable<SiteImages>(ds.Tables[3]).ToList();
                orgProfile.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[4]).ToList();
                orgProfile.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[5]).ToList();
                orgProfile.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[6]).ToList();
                orgProfile.SocialMediaLinks = Common.ConvertDataTable<SocialMedia>(ds.Tables[7]).ToList();
                orgProfile.lstCost = Common.ConvertDataTable<Cost>(ds.Tables[8]).ToList();
                orgProfile.Maxslots = orgProfile.lstslotTimes.Count > 0 ? Convert.ToInt32(orgProfile.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
                orgProfile.MaxDays = 1;
                orgProfile.CalenderDatesCount = orgProfile.lstslotsDates.Count > 0 ? orgProfile.lstslotsDates.FirstOrDefault().MaxCount : 0;
                //orgProfile.CalenderDatesCount = 7;
                var lat1 = GetLocationbyIP().Item1;//Added by Reena
                var long1 = GetLocationbyIP().Item2;//Added by Reena
                //var organisationIds = _doctor.GetOrganisationIdsFromTypeId(1005, lat1, long1);//Added by Reena
                //orgProfile.lstProviderAdvertisements = _doctor.GetAdvertisementsFromSearchText(organisationIds, 1005);//Added by Reena
                //orgProfile.lstProviderAdvertisements = Common.ConvertDataTable<ProviderAdvertisements>(ds.Tables[9]).ToList();//Added by Reena

                //orgProfile.lstProviderAdvertisements = _patient.ExecWithStoreProcedure<ProviderAdvertisements>("spGetAdvertisements_ByOrgTypeId @organisationId, @userTypeId",
                //               new SqlParameter("organisationId", System.Data.SqlDbType.VarChar) { Value = string.Join(",", organisationIds) },
                //               new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = 1005 }).ToList();
                var advList = _patient.ExecWithStoreProcedure<ProviderAdvertisements>("spGetAdvertisements_ByOrgTypeId @userTypeId",
                   new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = 1005 }).ToList();
                orgProfile.lstProviderAdvertisements = _doctor.GetDistanceMilebyOrgIds(advList, lat1, long1);
                if (ConfigurationManager.AppSettings["AgoraKey"] == null)
                {
                    ViewBag.AgoraKey = "0f8f282b530c4232b289867cda582737";
                }
                else
                {
                    ViewBag.AgoraKey = ConfigurationManager.AppSettings["AgoraKey"].ToString();
                }

                SetStoreAddress(orgProfile);


                return View(orgProfile);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return View(new ProfileViewModel());
            }
        }

        private void SetStoreAddress(ProfileViewModel orgProfile)
        {
            PharmacyAddress pharmacyAddress = new PharmacyAddress()
            {
                OrgId = orgProfile.OrganisationId,
                OrgUserTypeId = orgProfile.OrganizationTypeID,
                OrgName = orgProfile.OrganisationName,
                NPI = orgProfile.NPI,
                Address1 = orgProfile.Address1,
                Address2 = orgProfile.Address2,
                AddressId = orgProfile.AddressId,
                City = orgProfile.City + ", ",
                State = orgProfile.State + ", ",
                Country = orgProfile.Country + ", ",
                Zipcode = orgProfile.ZipCode,
                PhoneNumber = orgProfile.PhoneNumber,
            };
            Session["PharmacyDetails"] = pharmacyAddress;
        }

        [Route("Profile/SeniorCare/{OrganisationName?}-{npi?}")]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult SeniorCare(string OrganisationName, string npi)
        {
            try
            {
                ViewBag.Npi = npi;
                var orgIds = _facility.SQLQuery<int>("select OrganisationId from Organisation where npi = @npi", new SqlParameter("@npi", System.Data.SqlDbType.VarChar) { Value = npi }).ToList();
                var orgId = 0;
                if (orgIds.Count > 0)
                    orgId = orgIds.First();
                Doctyme.Model.ViewModels.ProfileViewModel _model = new Doctyme.Model.ViewModels.ProfileViewModel();
                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetSeiorCareProfileDetails + " " + orgId);
                var orgProfileList = Common.ConvertDataTable<Doctyme.Model.ViewModels.ProfileViewModel>(ds.Tables[0]).ToList();
                if (orgProfileList != null && orgProfileList.Count > 0)
                {
                    var orgProfile = Common.ConvertDataTable<Doctyme.Model.ViewModels.ProfileViewModel>(ds.Tables[0]).FirstOrDefault();
                    orgProfile.lstReviews = Common.ConvertDataTable<Reviews>(ds.Tables[1]).ToList();
                    orgProfile.lstOrgAmenOpt = Common.ConvertDataTable<OrganizationAmenityOptions>(ds.Tables[2]).ToList();
                    orgProfile.lstSiteImages = Common.ConvertDataTable<SiteImages>(ds.Tables[3]).ToList();
                    ViewBag.SiteImages = new List<SiteImages>(orgProfile.lstSiteImages);
                    if (orgProfile.lstSiteImages != null)
                        ViewBag.SiteImages.Remove(orgProfile.lstSiteImages.Find(x => x.ImagePath == orgProfile.ImagePath));
                    orgProfile.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[4]).ToList();
                    orgProfile.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[5]).ToList();
                    orgProfile.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[6]).ToList();
                    orgProfile.SocialMediaLinks = Common.ConvertDataTable<SocialMedia>(ds.Tables[7]).ToList();
                    orgProfile.lstCost = Common.ConvertDataTable<Cost>(ds.Tables[8]).ToList();
                    orgProfile.Maxslots = orgProfile.lstslotTimes.Count > 0 ? Convert.ToInt32(orgProfile.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
                    orgProfile.MaxDays = 1;
                    orgProfile.CalenderDatesCount = orgProfile.lstslotsDates.Count > 0 ? orgProfile.lstslotsDates.FirstOrDefault().MaxCount : 0;
                    //orgProfile.CalenderDatesCount = 7;
                    var lat1 = GetLocationbyIP().Item1;//Added by Reena
                    var long1 = GetLocationbyIP().Item2;//Added by Reena
                    //var organisationIds = _doctor.GetOrganisationIdsFromTypeId(1007, lat1, long1);//Added by Reena
                    //orgProfile.lstProviderAdvertisements = _doctor.GetAdvertisementsFromSearchText(organisationIds, 1007);//Added by Reena
                    // orgProfile.lstProviderAdvertisements = Common.ConvertDataTable<ProviderAdvertisements>(ds.Tables[9]).ToList();//Added by Reena
                    var advList = _patient.ExecWithStoreProcedure<ProviderAdvertisements>("spGetAdvertisements_ByOrgTypeId @userTypeId",
                    new SqlParameter("userTypeId", System.Data.SqlDbType.VarChar) { Value = 1007 }).ToList();
                    orgProfile.lstProviderAdvertisements = _doctor.GetDistanceMilebyOrgIds(advList, lat1, long1);
                    if (ConfigurationManager.AppSettings["AgoraKey"] == null)
                    {
                        ViewBag.AgoraKey = "0f8f282b530c4232b289867cda582737";
                    }
                    else
                    {
                        ViewBag.AgoraKey = ConfigurationManager.AppSettings["AgoraKey"].ToString();
                    }
                    return View(orgProfile);
                }
                else
                {
                    return RedirectToAction("SeniorCare", "Filter");
                }
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/SeniorCare-GET");
                return View(new ProfileViewModel());
            }
        }
        [HttpGet]
        [Route("ProvdersProfile/GetSlotsByOrg")]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public JsonResult GetSlots(int MaxDays, int OrganisationId)
        {
            OrganizationViewModel result = new OrganizationViewModel();
            try
            {

                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetTimeSlotsByOrgID + " " + MaxDays + "," + OrganisationId);
                result.lstslotsDates = Common.ConvertDataTable<SlotsDates>(ds.Tables[0]).ToList();
                result.lstslotTimes = Common.ConvertDataTable<SlotTimes>(ds.Tables[1]).ToList();
                result.OpeningHours = Common.ConvertDataTable<OpeningHour>(ds.Tables[2]).ToList();
                result.Maxslots = result.lstslotTimes.Count > 0 ? Convert.ToInt32(result.lstslotTimes.Max(i => i.slotTImesRno)) : 0;
                result.MaxDays = MaxDays;
                result.CalenderDatesCount = result.lstslotsDates.Count > 0 ? result.lstslotsDates.FirstOrDefault().MaxCount : 0;
                string opeHoursFial = "";
                if (result.OpeningHours.Count > 0)
                {
                    DateTime dateValue = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                    int weekNumber = (int)dateValue.DayOfWeek;
                    var resultweek = result.OpeningHours.Where(i => i.WeekDay == weekNumber + 1).FirstOrDefault();
                    result.OpeningDay = dateValue.ToString("ddd");
                    if (resultweek != null)
                    {
                        DateTime startDateTIme = DateTime.Parse(resultweek.StartDateTime);
                        DateTime endDateTIme = DateTime.Parse(resultweek.EndDateTime);
                        string strMinFormat = startDateTIme.ToString("hh:mm tt");
                        string endMinFormat = endDateTIme.ToString("hh:mm tt");

                        result.OpeningTime = strMinFormat + " -" + endMinFormat;
                        opeHoursFial = dateValue.ToString("ddd") + "  " + resultweek.StartDateTime + " -" + resultweek.EndDateTime;
                    }
                }

                return Json(new JsonResponse() { Status = 200, Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ProvdersProfile/GetSlotsByOrg");
                return Json(new JsonResponse() { Status = 0, Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult SlotsConfirmation()
        {
            try
            {
                //Doctyme.Model.ViewModels.SlotConfirmation _model = new Doctyme.Model.ViewModels.SlotConfirmation();

                //_model.DoctorId = 1;

                //    TempData["SlotConfirmation"] = _model;
                var sotInfo = Session["SlotConfirmation"] as Doctyme.Model.ViewModels.SlotConfirmation;

                if (sotInfo.SlotFor != "Doctor")
                {
                    sotInfo.DoctorId = sotInfo.OrganisationId;
                }
                string usrId = !string.IsNullOrEmpty(User.Identity.GetUserId()) ? User.Identity.GetUserId().ToString() : "0";
                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetUerDetailsByUserID + " " + usrId + " ," + sotInfo.DoctorId + "," + sotInfo.OrganisationId);
                var UserProfile = Common.ConvertDataTable<Doctyme.Model.ViewModels.SlotConfirmation>(ds.Tables[0]).FirstOrDefault();
                //var InsuranceProvider = Common.ConvertDataTable<SelectListItem>(ds.Tables[1]);
                var InsuranceType = InsuranceTypeList(); //Common.ConvertDataTable<SelectListItem>(ds.Tables[2]);
                ViewBag.InsuranceProvider = new List<SelectListItem>(); //InsuranceProvider;
                sotInfo.OrganisationName = ds.Tables[3].Rows.Count > 0 ? ds.Tables[3].Rows[0]["OrganisationName"].ToString() : "";
                sotInfo.FullOrgAddress = ds.Tables[3].Rows.Count > 0 ? ds.Tables[3].Rows[0]["FullOrgAddress"].ToString() : "";
                sotInfo.FullOrgName = ds.Tables[3].Rows.Count > 0 ? ds.Tables[3].Rows[0]["FullOrgName"].ToString() : "";
                sotInfo.DoctorProfileImage = ds.Tables[3].Rows.Count > 0 ? ds.Tables[3].Rows[0]["DoctorProfileImage"].ToString() : "";
                sotInfo.AddressId = ds.Tables[3].Rows.Count > 0 ? Convert.ToInt32(ds.Tables[3].Rows[0]["AddressId"]) : 0;

                if (!string.IsNullOrEmpty(User.Identity.GetUserId()))
                {
                    sotInfo.FirstName = UserProfile.FirstName;
                    sotInfo.LastName = UserProfile.LastName;
                    sotInfo.UserEmail = UserProfile.UserEmail;
                    sotInfo.UserName = UserProfile.UserName;
                    sotInfo.Address1 = UserProfile.Address1;
                    sotInfo.DOB = UserProfile.DOB;
                    //sotInfo.InsurancePlanId = UserProfile.InsurancePlanId;
                    //sotInfo.InsuranceProviderId = UserProfile.InsuranceProviderId;
                    sotInfo.ZipCode = UserProfile.ZipCode;
                    sotInfo.City = UserProfile.City;
                    sotInfo.State = UserProfile.State;
                    sotInfo.PhoneNumber = UserProfile.PhoneNumber;
                    sotInfo.FullName = sotInfo.FullName + "" + sotInfo.LastName;
                    //sotInfo.InsuranceTypeId = UserProfile.InsuranceTypeId;

                    ViewBag.InsurancePlan = InsuranceType;

                    ViewBag.StateList = new List<SelectListItem>();
                    ViewBag.CityList = new List<SelectListItem>();
                    ViewBag.ZipList = new List<SelectListItem>();
                    if (InsuranceType.Count() > 0)
                    {
                        ViewBag.InsurancePlan = InsuranceType;
                    }
                    else
                    {
                        ViewBag.InsurancePlan = new List<SelectListItem>();
                    }
                    sotInfo.UserId = Convert.ToInt32(usrId);

                }
                else
                {

                    //ViewBag.StateList = _state.GetAll().Select(x => new SelectListItem
                    //{
                    //    Text = x.StateName,
                    //    Value = x.StateId.ToString()
                    //}).ToList();
                    DataSet state = _doctor.GetQueryResult(StoredProcedureList.GetStatesByCountry + " US");
                    var Statelist = Common.ConvertDataTable<SelectListItem>(state.Tables[0]);
                    Statelist = Statelist.OrderBy(m => m.Text).ToList();
                    ViewBag.StateList = Statelist;
                    ViewBag.ZipList = new List<SelectListItem>();

                    ViewBag.InsurancePlan = InsuranceType; //new List<SelectListItem>();

                    ViewBag.CityList = new List<SelectListItem>();


                }
                return View(sotInfo);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "SlotsConfirmation");
                return View(new SlotConfirmation());
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public async Task<JsonResult> SlotsConfirmation(Doctyme.Model.ViewModels.SlotConfirmation _model)
        {

            try
            {
                _model.SlotDate = !string.IsNullOrEmpty(_model.SlotDate) ? Convert.ToDateTime(_model.SlotDate).ToString("MM/dd/yyyy") : DateTime.Now.ToString("MM/dd/yyyy");
                _model.SlotDate = _model.SlotDate.Replace('-', '/');
                if (_model.UserId != 0)
                {
                    _model.IsTextReminder = !string.IsNullOrEmpty(_model.IsTextReminder.ToString()) ? _model.IsTextReminder : (short)0;
                    _model.IsEmailReminder = !string.IsNullOrEmpty(_model.IsEmailReminder.ToString()) ? _model.IsEmailReminder : (short)0;
                    _model.IsWhantCallUs = !string.IsNullOrEmpty(_model.IsWhantCallUs.ToString()) ? _model.IsWhantCallUs : (short)0;
                    _model.InsuranceTypeId = !string.IsNullOrEmpty(_model.InsuranceTypeId) ? _model.InsuranceTypeId : "0";
                    var strintResult = ConvertObjectToXMLString(_model);

                    DataSet ds = _doctor.GetQueryResult(StoredProcedureList.InsertSlot + " '" + strintResult + "'");
                    if (ds.Tables[0].Rows[0][0].ToString().Contains("Fail"))
                    {
                        return Json(new JsonResponse { Status = 0, Message = "Something Wrong while booking please try later.." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (_model.IsEmailReminder == 1)
                        {
                            string OrganisationName = ds.Tables[0].Rows[0]["OrganisationName"].ToString();
                            string OrganisationId = ds.Tables[0].Rows[0]["OrganisationId"].ToString();
                            string FullOrgName = ds.Tables[0].Rows[0]["FullOrgName"].ToString();
                            string FullOrgAddress = ds.Tables[0].Rows[0]["FullOrgAddress"].ToString();
                            string phone = ds.Tables[0].Rows[0]["phone"].ToString();
                            string UserID = ds.Tables[0].Rows[0]["UserID"].ToString();
                            string SlotDate = ds.Tables[0].Rows[0]["SlotDate"].ToString();
                            string SlotTime = ds.Tables[0].Rows[0]["SlotTime"].ToString();
                            string UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
                            string SlotEndTime = ds.Tables[0].Rows[0]["SlotEndTime"].ToString();
                            string body = Common.ReadEmailTemplate(EmailTemplate.ConfirmSlot);

                            DateTime StartDate = DateTime.Parse(SlotDate + " " + SlotTime, System.Globalization.CultureInfo.InvariantCulture);
                            DateTime endate = DateTime.Parse(SlotDate + " " + SlotEndTime, System.Globalization.CultureInfo.InvariantCulture);
                            body = body.Replace("{UserName}", UserName)
                                .Replace("{OrganizationName}", OrganisationName)
                                .Replace("{StartDate}", StartDate.ToString("yyyyMMddTHHmmss"))
                                .Replace("{EndDate}", endate.ToString("yyyyMMddTHHmmss"))
                                .Replace("{Address}", FullOrgAddress)
                                .Replace("{Location}", FullOrgAddress)
                                .Replace("{AppointMentDate}", SlotDate)
                                .Replace("{OrgPhone}", phone)
                                .Replace("{OrgMapAddress}", FullOrgAddress)
                                 .Replace("{SlotTime}", SlotTime)
                                .Replace("{EmailSentDate}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));

                            SendMail.SendEmail(_model.UserEmail, "", "", "", body, "Booking Confirmation.");
                        }
                       
                        return Json(new JsonResponse { Status = 1, Message = "Please Check your email account for account confirmation.", Data = new { } }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    _model.IsTextReminder = !string.IsNullOrEmpty(_model.IsTextReminder.ToString()) ? _model.IsTextReminder : (short)0;
                    _model.IsEmailReminder = !string.IsNullOrEmpty(_model.IsEmailReminder.ToString()) ? _model.IsEmailReminder : (short)0;
                    _model.IsWhantCallUs = !string.IsNullOrEmpty(_model.IsWhantCallUs.ToString()) ? _model.IsWhantCallUs : (short)0;
                    _model.InsuranceTypeId = !string.IsNullOrEmpty(_model.InsuranceTypeId) ? _model.InsuranceTypeId : "0";
                    var strintResult = ConvertObjectToXMLString(_model);
                    DataSet ds = _doctor.GetQueryResult(StoredProcedureList.InsertSlot + " '" + strintResult + "'");
                    if (ds.Tables[0].Rows[0][0].ToString() == "Email")
                    {
                        return Json(new JsonResponse { Status = 0, Message = "Email Already exists.." }, JsonRequestBehavior.AllowGet);
                    }
                    if (!ds.Tables[0].Rows[0][0].ToString().Contains("Fail"))
                    {
                        var result = _userManager.AddPassword(Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"].ToString()), _model.Password);
                        if (_model.IsEmailReminder == 1)
                        {
                            string OrganisationName = ds.Tables[0].Rows[0]["OrganisationName"].ToString();
                            string OrganisationId = ds.Tables[0].Rows[0]["OrganisationId"].ToString();
                            string FullOrgName = ds.Tables[0].Rows[0]["FullOrgName"].ToString();
                            string FullOrgAddress = ds.Tables[0].Rows[0]["FullOrgAddress"].ToString();
                            string phone = ds.Tables[0].Rows[0]["phone"].ToString();
                            string UserID = ds.Tables[0].Rows[0]["UserID"].ToString();
                            string SlotDate = ds.Tables[0].Rows[0]["SlotDate"].ToString();
                            string SlotTime = ds.Tables[0].Rows[0]["SlotTime"].ToString();
                            string UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
                            string SlotEndTime = ds.Tables[0].Rows[0]["SlotEndTime"].ToString();
                            string body = Common.ReadEmailTemplate(EmailTemplate.ConfirmSlot);

                            DateTime StartDate = DateTime.Parse(SlotDate + " " + SlotTime, System.Globalization.CultureInfo.InvariantCulture);
                            DateTime endate = DateTime.Parse(SlotDate + " " + SlotEndTime, System.Globalization.CultureInfo.InvariantCulture);
                            body = body.Replace("{UserName}", UserName)
                                .Replace("{OrganizationName}", OrganisationName)
                                .Replace("{StartDate}", StartDate.ToString("MMddyyyyTHHmmss"))
                                .Replace("{EndDate}", endate.ToString("MMddyyyyTHHmmss"))
                                .Replace("{Address}", FullOrgAddress)
                                .Replace("{Location}", FullOrgAddress)
                                .Replace("{AppointMentDate}", SlotDate)
                                .Replace("{OrgPhone}", phone)
                                .Replace("{OrgMapAddress}", FullOrgAddress)
                                .Replace("{SlotTime}", SlotTime)
                                .Replace("{EmailSentDate}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));

                            SendMail.SendEmail(_model.UserEmail, "", "", "", body, "Booking Confirmation.");
                        }
                        //Against Issue #28
                        string code = await _userManager.GenerateEmailConfirmationTokenAsync(Convert.ToInt16(ds.Tables[0].Rows[0]["UserID"].ToString()));
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = Convert.ToInt16(ds.Tables[0].Rows[0]["UserID"].ToString()), code = code, Password = _model.Password }, protocol: Request.Url.Scheme);
                        string sbody = Common.ReadEmailTemplate(EmailTemplate.ConfirmEmail);
                        sbody = sbody.Replace("{UserName}", "")
                            .Replace("{action_url}", callbackUrl)
                            .Replace("{action_url_settings}", "")
                            .Replace("{action_url_help}", "")
                            .Replace("{action_url_notmyaccount}", "");

                        SendMail.SendEmail(_model.UserEmail, "", "", "", sbody, "Confirm your account");
                        //
                    }
                    else
                    {
                        return Json(new JsonResponse { Status = 0, Message = "Something Wrong while booking please try later.." }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new JsonResponse { Status = 1, Message = "Please Check your email account for account confirmation.", Data = new { } }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Common.LogError(ex, "SlotsConfirmation-POST");
                return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
            }


        }

        private IEnumerable<SelectListItem> InsuranceTypeList()
        {
            DataSet ds = _doctor.GetQueryResult("spInsuranceTypeDropDownList_Get");
            DataColumn dcId = ds.Tables[0].Columns["InsuranceTypeId"];
            DataColumn dcName = ds.Tables[0].Columns["Name"];
            var list = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => new SelectListItem { Text = dr.Field<string>(dcName), Value = dr.Field<int>(dcId).ToString() }).ToList();

            return list;
        }
        //--Drug Info AutoComplete
        [HttpGet]
        public JsonResult GetInsProvider(string Prefix)
        {
            var ProviderList = _patient.ExecWithStoreProcedure<DrpInsuranceProvider>("spGetInsProviderInfoAutoComplete @ProviderName",
                new SqlParameter("ProviderName", System.Data.SqlDbType.VarChar) { Value = Prefix }).ToList();
            return Json(ProviderList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCityList(string id)
        {
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetCitiesByStates + " '" + id + "'");
            var lstCities = Common.ConvertDataTable<SelectListItem>(ds.Tables[0]);

            //var result = _cityStateZip.GetAll(i => i.State == id).Select(x => new SelectListItem
            //{
            //    Text = x.City,
            //    Value = x.City.ToString()
            //}).Distinct().ToList();
            return Json(new JsonResponse { Status = 1, Data = lstCities }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInsurancePlanByTypeId(int typeId)
        {
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetInsurancePlanByTypeId + " " + typeId);
            var lstInsurancePlan = Common.ConvertDataTable<SelectListItem>(ds.Tables[0]);

            return Json(new JsonResponse { Status = 1, Data = lstInsurancePlan }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInsurancePlan(int id)
        {
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetInsurancePlanById + " " + id);
            var lstInsurancePlan = Common.ConvertDataTable<SelectListItem>(ds.Tables[0]);

            return Json(new JsonResponse { Status = 1, Data = lstInsurancePlan }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetZipCode(string State, string City)
        {
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetZipCodeaByCities + " '" + City + "' ," + "'" + State + "'");
            var lstZipCodes = Common.ConvertDataTable<SelectListItem>(ds.Tables[0]);

            //var result = _cityStateZip.GetAll(i => i.State == State && i.City == City).Select(x => new SelectListItem
            //{
            //    Text = x.ZipCode,
            //    Value = x.ZipCode.ToString()
            //}).Distinct().ToList();
            return Json(new JsonResponse { Status = 1, Data = lstZipCodes }, JsonRequestBehavior.AllowGet);
        }

        static string ConvertObjectToXMLString(object classObject)
        {
            string xmlString = null;
            XmlSerializer xmlSerializer = new XmlSerializer(classObject.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, classObject);
                memoryStream.Position = 0;
                xmlString = new StreamReader(memoryStream).ReadToEnd();
            }
            return xmlString;
        }
        [HttpPost]
        public JsonResult ClaimPractice(ClaimPractice _claimPractice)
        {
            try
            {
                string msg = "Login";
                if (string.IsNullOrEmpty(User.Identity.GetUserId()) || User.Identity.GetUserId() == "0")
                {
                    msg = "NoLogin";
                }
                else
                {
                    TempData["claimInfo"] = _claimPractice;
                }
                return Json(new JsonResponse { Status = 1, Data = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ClaimPractice-POST");
                return Json(new JsonResponse { Status = 1, Data = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ClaimPracticeBusiness()
        {
            try
            {
                var sotInfo = TempData["claimInfo"] as ClaimPractice;

                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetOrganizationAddressByOrgId + " " + sotInfo.OrganizationId);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    sotInfo.Address1 = ds.Tables[0].Rows[0]["Address1"].ToString();
                    sotInfo.Address2 = ds.Tables[0].Rows[0]["Address2"].ToString();
                    sotInfo.State = ds.Tables[0].Rows[0]["State"].ToString();
                    sotInfo.City = ds.Tables[0].Rows[0]["City"].ToString();
                    sotInfo.Country = ds.Tables[0].Rows[0]["Country"].ToString();
                    sotInfo.ZipCode = ds.Tables[0].Rows[0]["ZipCode"].ToString();
                    sotInfo.OrganizationName = ds.Tables[0].Rows[0]["OrganisationName"].ToString();
                    sotInfo.PhoneNumber = ds.Tables[0].Rows[0]["Phone"].ToString();
                    sotInfo.Email = ds.Tables[0].Rows[0]["Email"].ToString();
                }
                return View(sotInfo);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ClaimPracticeBusiness");
                return View(new ClaimPractice());
            }
        }
        [HttpPost]
        public ActionResult ClaimPracticeBusiness(HttpPostedFileBase claimDoc, ClaimPractice _claimPractice)
        {
            string imagePath = "", newImageName = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (_claimPractice.PostedFile != null && _claimPractice.PostedFile.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(_claimPractice.PostedFile.FileName);
                        newImageName = "ClaimDoc-" + DateTime.Now.Ticks.ToString() + extension;


                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~" + FilePathList.ClaimDocument));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~" + FilePathList.ClaimDocument));
                        }
                        var path = Path.Combine(Server.MapPath("~" + FilePathList.ClaimDocument), newImageName);


                        // var path = Path.Combine(Server.MapPath("~" + FilePathList.ClaimDocument), newImageName);
                        //var path= Path.Combine(Server.MapPath(FilePathList.ClaimDocument), newImageName);
                        _claimPractice.PostedFile.SaveAs(path);

                        imagePath = "~" + FilePathList.ClaimDocument + newImageName;
                    }

                    DataSet ds = _doctor.GetQueryResult(StoredProcedureList.InsertBusinessClaim + " '" + FilePathList.ClaimDocument + "'" + "," + "'" + newImageName + "'" + "," + User.Identity.GetUserId() + "," + _claimPractice.ReferenceId + ",'" + _claimPractice.ClaimType + "'");

                    if (!ds.Tables[0].Rows[0][0].ToString().Contains("Fail"))
                    {
                        var user = _userManager.FindById(Convert.ToInt32(User.Identity.GetUserId()));

                        var callbackUrl = Url.Action("ConfirmClaimEmail", "Profile", new { userId = user.Id, BusinessClaimID = ds.Tables[0].Rows[0][0].ToString() }, protocol: Request.Url.Scheme);
                        //var settingUrl = Url.Action("", "Home", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        string body = Common.ReadEmailTemplate(EmailTemplate.ClaimVerify);
                        body = body.Replace("{LastName}", user.LastName)
                            .Replace("{FirstName}", user.FirstName)
                            .Replace("{OrganizationName}", _claimPractice.OrganizationName)
                            .Replace("{ReturnUrl}", callbackUrl);


                        SendMail.SendEmail(user.Email, "", "", "", body, "Claim Practice Verification Required");
                        switch (_claimPractice.ClaimType)
                        {
                            case "Doctor":
                                return RedirectToAction("Organization", "Search", new { doctorName = _claimPractice.DoctorName, npi = _claimPractice.Npi });
                            //return RedirectToAction("/Profile/Doctor/" + _claimPractice.DoctorName + "-" + _claimPractice.Npi);
                            case "Senior Care":
                                return RedirectToAction("SeniorCare", "ProvdersProfile", new { OrganisationName = _claimPractice.OrganizationName.Replace("&", ""), orgId = _claimPractice.OrganizationId });
                            // return RedirectToRoute("/Profile/SeniorCare/" + _claimPractice.OrganizationName + "-" + _claimPractice.OrganizationId);
                            case "Pharmacy":
                                return RedirectToAction("Pharmacy", "ProvdersProfile", new { OrganisationName = _claimPractice.OrganizationName.Replace("&", ""), orgId = _claimPractice.OrganizationId });
                            //return RedirectToRoute("/Profile/Pharmacy/" + _claimPractice.OrganizationName + "-" + _claimPractice.OrganizationId);
                            case "Facility":
                                return RedirectToAction("Facility", "ProvdersProfile", new { OrganisationName = _claimPractice.OrganizationName.Replace("&", ""), orgId = _claimPractice.OrganizationId });
                            // return RedirectToRoute("/Profile/Facility/" + _claimPractice.OrganizationName + "-" + _claimPractice.OrganizationId);
                            default:
                                return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        return View(_claimPractice);
                    }
                }
                else
                {
                    return View(_claimPractice);
                }
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ClaimPracticeBusiness-POST");
                return View(new ClaimPractice());
            }
        }
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmClaimEmail(int userId, int BusinessClaimID)
        {
            try
            {
                if (userId == 0 && BusinessClaimID == 0)
                {
                    return View("Error");
                }

                var result = _doctor.GetQueryResult(StoredProcedureList.UpdateBusinessClaim + " " + BusinessClaimID + "," + userId);

                var jsonModels = new List<Common.JsonModel> { new Common.JsonModel(Common.JsonType.Success, @"Your claim process hass been confirm successfully.") };
                this.AddJsons(jsonModels);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ConfirmClaimEmail");
            }

            return RedirectToAction("Login", "Account");
        }

        public ActionResult ClaimProcess()
        {
            return View();
        }

        private Tuple<decimal, decimal> GetLocationbyIP()//Added by Reena
        {
            string ipString = GetIPString();
            var ipInfo = new Doctyme.Model.ViewModels.IpInfo();
            var latOfUser = string.Empty;
            var longOfUser = string.Empty;

            if (!string.IsNullOrEmpty(ipString))
            {
                try
                {
                    ipInfo = JsonConvert.DeserializeObject<Doctyme.Model.ViewModels.IpInfo>(ipString);
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
            decimal.TryParse(latOfUser, out lat2);
            decimal long2 = 0;
            decimal.TryParse(longOfUser, out long2);
            var tupleData = new Tuple<decimal, decimal>(lat2, long2);
            return tupleData;
        }

        private string GetIPString()//Added by Reena
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
                    string info = new System.Net.WebClient().DownloadString("http://ipinfo.io/" + VisitorsIPAddr);
                    return info;
                }
                catch (Exception ex)
                {
                    Common.LogError(ex, "ConfirmClaimEmail");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("ProvdersProfile/CheckIfPatientLoggedIn")]
        public int CheckIfPatientLoggedIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(UserRoles.Patient))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return -1;
            }
        }

        #region Patient Prescription Info 

        //Worked this Module by Venkat

        #region New Prescription

        [HttpGet]
        [Authorize(Roles = "Patient")]
        [Route("ProvdersProfile/NewPrescription")]
        public ActionResult NewPrescription(string pharmacy)
        {
            PatientPrescription result = null;
            try
            {

                if (!User.Identity.IsAuthenticated) return View(new ViewModels.PatientBasicInformation());
                ViewBag.PharmacyID = pharmacy;

                int userId = User.Identity.GetUserId<int>();
                int _patientId = _patient.GetSingle(x => x.UserId == userId).PatientId;

                result = GetPatientProfileInfo(userId);
                result.PrescriptionType = PrescriptionType.New;
                result.OrgId = Convert.ToInt32(pharmacy);

                var DrugStrenthList = _patient.ExecWithStoreProcedure<DrugStrengthByDrugId>("spGetDrugStrengthByDrugID @DrugId",
                new SqlParameter("DrugId", System.Data.SqlDbType.Int) { Value = 0 }
                ).ToList();
                ViewBag.DrugStrenthList = new SelectList(DrugStrenthList, "DrugStrengthId", "Name");

                var newPreTableData = Session["NewPresTableData"] as PatientPrescription;

                if (result.lstNewPrescriptionInfo == null && newPreTableData == null)
                    ClearPrescriptionSession();

                Session["NewPatientPrescription"] = newPreTableData != null ? newPreTableData : result;

                if (newPreTableData != null)
                {
                    result = newPreTableData;
                }
                return View(result);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return RedirectToAction("Pharmacy", "ProvdersProfile", new { OrganisationName = "", orgId = 0 });
            }
        }

        [HttpPost]
        public JsonResult NewPrescriptionInfo(List<NewPrescription> newPrescription, string deliver)
        {
            try
            {
                var prescription = Session["NewPatientPrescription"] as PatientPrescription;
                newPrescription.RemoveAt(0);
                prescription.lstNewPrescriptionInfo = newPrescription;
                prescription.IsDeliveryType = deliver.Equals("storePickup") ? DeliveryType.Store : DeliveryType.Home;

                Session["NewPresTableData"] = prescription;

                //Session["NewPatientPrescription"] = prescription;
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public ActionResult InsuranceDetails()
        {
            try
            {
                PatientPrescription model = null;
                if (Session["NewPatientPrescription"] as PatientPrescription != null)
                {
                    var newPresEditMode = Session["NewPresTableData"] as PatientPrescription;
                    model = newPresEditMode != null ? newPresEditMode : Session["NewPatientPrescription"] as PatientPrescription;
                }

                if (Session["TransferPatientPrescription"] as PatientPrescription != null)
                {
                    var transPresEditMode = Session["TransPresTableData"] as PatientPrescription;
                    model = transPresEditMode != null ? transPresEditMode : Session["TransferPatientPrescription"] as PatientPrescription;
                }

                if (!User.Identity.IsAuthenticated) return View(new ViewModels.PatientBasicInformation());
                var typeDataList = _patient.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get").ToList();
                ViewBag.typeList = new SelectList(typeDataList, "InsuranceTypeId", "Name");

                var insPlanList = _patient.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spGetInsurancePlanInsTypeAndInsProvider_Get");
                ViewBag.PlanList = new SelectList(insPlanList, "InsurancePlanId", "Name");


                model.InsuranceTypeId = 0;

                if (model.PrescriptionType == PrescriptionType.New)
                {
                    Session["NewPatientPrescription"] = model;
                }
                else
                {
                    Session["TransferPatientPrescription"] = model;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return View(new ProfileViewModel());
            }
        }

        [HttpPost]
        public JsonResult SubmitConfirm(PatientPrescription patientInsuranceDetails)
        {
            try
            {
                if (patientInsuranceDetails.PrescriptionType == PrescriptionType.New)
                    Session["NewPatientPrescription"] = BindPrescriptionInsuranceDetails(patientInsuranceDetails);

                if (patientInsuranceDetails.PrescriptionType == PrescriptionType.Transfer)
                    Session["TransferPatientPrescription"] = BindPrescriptionInsuranceDetails(patientInsuranceDetails);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        private PatientPrescription BindPrescriptionInsuranceDetails(PatientPrescription patientPrescription)
        {
            try
            {
                var result = patientPrescription.PrescriptionType == PrescriptionType.New ? Session["NewPatientPrescription"] as PatientPrescription
                   : Session["TransferPatientPrescription"] as PatientPrescription;

                result.IsPrimary = patientPrescription.IsPrimary;
                result.RelationShipWithCardHolder = patientPrescription.RelationShipWithCardHolder;
                result.CardHolderDateOfBirth = patientPrescription.CardHolderDateOfBirth;
                result.InsuranceProviderID = patientPrescription.InsuranceProviderID;
                result.InsuranceTypeId = patientPrescription.InsuranceTypeId;
                result.InsurancePlanId = patientPrescription.InsurancePlanId;
                result.InsuranceType = patientPrescription.InsuranceType;
                result.InsurancePlan = patientPrescription.InsurancePlan;
                result.HealthInsurance = patientPrescription.HealthInsurance;//check in DB and make it to InsuranceProvider Name
                result.PrimaryCardHolderName = patientPrescription.PrimaryCardHolderName;
                result.PhoneNumber = patientPrescription.PhoneNumber;
                result.MemberNumber = patientPrescription.MemberNumber;
                result.BinNumber = patientPrescription.BinNumber;
                result.GroupNumber = patientPrescription.GroupNumber;
                result.PcnNumber = patientPrescription.PcnNumber;
                result.InsuranceProvider = patientPrescription.InsuranceProvider;

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Refill Prescription

        [HttpGet]
        [Authorize(Roles = "Patient")]
        [Route("ProvdersProfile/RefillPrescription")]
        public ActionResult RefillPrescription(string pharmacy)
        {
            try
            {
                //ClearPrescriptionSession();
                if (!User.Identity.IsAuthenticated) return View(new ViewModels.PatientBasicInformation());
                ViewBag.PharmacyID = pharmacy;

                int userId = User.Identity.GetUserId<int>();
                int _patientId = _patient.GetSingle(x => x.UserId == userId).PatientId;

                var result = GetPatientProfileInfo(userId);
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@PatientId", SqlDbType.BigInt) { Value = _patientId });
                parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

                List<ExistingPrescription> existingPrescriptions =
                    _patient.GetDataList<ExistingPrescription>(StoredProcedureList.spGetExistingPrescription, parameters).ToList();
                if (existingPrescriptions != null && existingPrescriptions.Count > 0)
                {
                    result.lstExistingPrescriptionInfo = existingPrescriptions;
                    result.InsuranceProvider = existingPrescriptions[0].InsuranceProvider;
                    result.InsuranceType = existingPrescriptions[0].InsuranceType;
                    result.InsurancePlan = existingPrescriptions[0].InsurancePlan;
                }
                result.PrescriptionType = PrescriptionType.Refill;
                var refillData = Session["RefillPrescriptionData"] as PatientPrescription;
                if (refillData != null)
                {
                    ViewBag.Refillitems = refillData.lstExistingPrescriptionInfo;
                    result.IsDeliveryType = refillData.IsDeliveryType;
                }
                if (result.lstNewPrescriptionInfo == null && refillData == null)
                    ClearPrescriptionSession();
                Session["RefillPrescription"] = result;
                return View(result);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return RedirectToAction("Pharmacy", "ProvdersProfile", new { OrganisationName = "", orgId = 0 });
            }
        }

        [HttpPost]
        public JsonResult RefillPrescriptionInfo(List<ExistingPrescription> refillPrescription, string deliver)
        {
            try
            {
                var refillPres = Session["RefillPrescription"] as PatientPrescription;
                refillPrescription.RemoveAt(0);
                refillPres.lstExistingPrescriptionInfo = refillPrescription;
                refillPres.IsDeliveryType = deliver.Equals("storePickup") ? DeliveryType.Store : DeliveryType.Home;
                Session["RefillPrescription"] = refillPres;
                Session["RefillPrescriptionData"] = refillPres;
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
                // return Redirect("/ProvdersProfile/RefillPrescription");
            }
        }

        #endregion

        #region Transfer Prescription

        [HttpGet]
        [Authorize(Roles = "Patient")]
        [Route("ProvdersProfile/TransferPrescription")]
        public ActionResult TransferPrescription(string pharmacy)
        {
            try
            {
                if (!User.Identity.IsAuthenticated) return View(new ViewModels.PatientBasicInformation());
                ViewBag.PharmacyID = pharmacy;

                int userId = User.Identity.GetUserId<int>();
                int _patientId = _patient.GetSingle(x => x.UserId == userId).PatientId;

                var result = GetPatientProfileInfo(userId);
                result.OrgId = Convert.ToInt32(pharmacy);
                result.PrescriptionType = PrescriptionType.Transfer;
                var DrugStrenthList = _patient.ExecWithStoreProcedure<DrugStrengthByDrugId>("spGetDrugStrengthByDrugID @DrugId",
                new SqlParameter("DrugId", System.Data.SqlDbType.Int) { Value = 0 }
                ).ToList();
                ViewBag.DrugStrenthList = new SelectList(DrugStrenthList, "DrugStrengthId", "Name");

                var transPreTableData = Session["TransPresTableData"] as PatientPrescription;

                if (result.lstTransferPrescription == null & transPreTableData == null)
                    ClearPrescriptionSession();

                Session["TransferPatientPrescription"] = transPreTableData != null ? transPreTableData : result;

                if (transPreTableData != null)
                {
                    var backtofrompharmacy = result.Pharmacy;
                    var backtoTransferPharmacy = transPreTableData.Pharmacy;
                    result = transPreTableData;
                    result.Pharmacy = backtofrompharmacy;
                    result.TransferPharmacy = backtoTransferPharmacy;
                }
                return View(result);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return View(new ProfileViewModel());
            }
        }

        [HttpPost]
        public JsonResult TransferPrescriptionInfo(List<TransferPrescription> transferPrescriptions, string deliver)
        {
            try
            {
                var transPrescription = Session["TransferPatientPrescription"] as PatientPrescription;
                transferPrescriptions.RemoveAt(0);

                transPrescription.lstTransferPrescription = transferPrescriptions;
                transPrescription.PrescriptionType = PrescriptionType.Transfer;
                transPrescription.IsDeliveryType = deliver.Equals("storePickup") ? DeliveryType.Store : DeliveryType.Home;

                transPrescription = GetTransferPharmacyDetail(transPrescription);

                //Session["TransferPatientPrescription"] = transPrescription;
                Session["TransPresTableData"] = transPrescription;
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
                // return Redirect("/ProvdersProfile/RefillPrescription");
            }
        }

        #endregion

        [HttpGet]
        public ActionResult ConfirmRefillPrescription()
        {
            try
            {
                PatientPrescription result = null;

                if (Session["NewPatientPrescription"] as PatientPrescription != null)
                {
                    result = Session["NewPatientPrescription"] as PatientPrescription;
                    result.confirmPrescription = result.lstNewPrescriptionInfo.Select(x => new ConfirmPrescription
                    {
                        RXNumber = x.RXNumber,
                        DrugName = x.DrugName,
                        DrugStrengthName = x.DrugStrengthName,
                        Quantity = x.Quantity,
                        InsuranceProvider = result.InsuranceProvider
                    }).ToList();
                }

                if (Session["RefillPrescription"] as PatientPrescription != null)
                {
                    result = Session["RefillPrescription"] as PatientPrescription;
                    result.confirmPrescription = result.lstExistingPrescriptionInfo.Select(x => new ConfirmPrescription
                    {
                        RXNumber = x.RXNumber,
                        DrugName = x.DrugName,
                        DrugStrengthName = x.DrugStrengthName,
                        Quantity = x.Quantity,
                        InsuranceProvider = result.InsuranceProvider
                    }).ToList();
                }

                if (Session["TransferPatientPrescription"] as PatientPrescription != null)
                {
                    result = Session["TransferPatientPrescription"] as PatientPrescription;
                    result.confirmPrescription = result.lstTransferPrescription.Select(x => new ConfirmPrescription
                    {
                        RXNumber = x.TransferPrescriptionNumber,
                        DrugName = x.DrugStrengthName,
                        DrugStrengthName = x.DrugStrengthName,
                        Quantity = x.Quantity,
                        InsuranceProvider = result.InsuranceProvider
                    }).ToList();
                }

                return View(result);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SavePrescription(PrescriptionType presType)
        {
            string orgName = string.Empty;
            string npiValue = string.Empty;
            try
            {
                switch (presType)
                {
                    case PrescriptionType.New:
                        SaveNewPrescription(ref orgName, ref npiValue);
                        break;
                    case PrescriptionType.Refill:
                        SaveRefillPrescription(ref orgName, ref npiValue);
                        break;
                    case PrescriptionType.Transfer:
                        SaveTransferPrescription(ref orgName, ref npiValue);
                        break;
                    default:
                        break;
                }
                ClearPrescriptionSession();
                return Json(new { orgName, npiValue, result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Profile/Pharmacy-GET");
                return RedirectToAction("Pharmacy", "ProvdersProfile", new { OrganisationName = orgName, npi = npiValue });
            }
        }

        [HttpGet]
        public JsonResult GetInsurancePlanByInsProviderandInsTypeId(int insTypeId, int providerId)
        {
            var planList = _patient.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spGetInsurancePlanInsTypeAndInsProvider_Get @InsuranceTypeId, @InsuranceProviderId",
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = insTypeId }, new SqlParameter("InsuranceProviderId", System.Data.SqlDbType.Int) { Value = providerId }
                ).ToList();
            ViewBag.PlanList = planList;
            return Json(planList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDrugStrengthByDrugId(int DrugId)
        {
            var DrugStrenthList = _patient.ExecWithStoreProcedure<DrugStrengthByDrugId>("spGetDrugStrengthByDrugID @DrugId",
                new SqlParameter("DrugId", System.Data.SqlDbType.Int) { Value = DrugId }
                ).ToList();
            ViewBag.DrugStrenthList = DrugStrenthList;
            return Json(DrugStrenthList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPharmacyInfo(int OrgId, string searchLocationKey, string searchprefix)
        {
            var splitarr = searchLocationKey.Split(',').Select(p => p.Trim()).ToList();
            searchLocationKey = string.Join("|", splitarr.ToArray());

            var PharmacyGroupList = _patient.ExecWithStoreProcedure<PharmacyDetailsById>("spGetPharmacyByZipcode @OrganizationTypeId, @userTypeId, @SearchLocationKey, @SearchPrefix",
                new SqlParameter("@OrganizationTypeId", System.Data.SqlDbType.Int) { Value = OrgId },
                new SqlParameter("@userTypeId", System.Data.SqlDbType.Int) { Value = UserTypes.Pharmacy },
                new SqlParameter("@SearchLocationKey", System.Data.SqlDbType.VarChar) { Value = searchLocationKey },
                new SqlParameter("@SearchPrefix", System.Data.SqlDbType.VarChar) { Value = searchprefix }
                ).ToList();

            Session["PharmacyDetailsByZipCode"] = PharmacyGroupList;
            return Json(PharmacyGroupList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDoctorsByUserId(string prefix, int userId)
        {
            var DoctorList = _patient.ExecWithStoreProcedure<DoctorViewModel>("spGetDoctorsByUserID @UserId, @DoctorName",
                  new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userId },
                  new SqlParameter("DoctorName", System.Data.SqlDbType.VarChar) { Value = prefix }
                  ).ToList();

            return Json(DoctorList, JsonRequestBehavior.AllowGet);
        }

        public string GetCityStateInfoById(int id, string type)
        {
            string result = "";

            var info = _patient.SQLQuery<CityStateInfoByZipCodeViewModel>("spGetCityStateZipInfoByID @ID", new SqlParameter("ID", System.Data.SqlDbType.Int) { Value = id }).ToList();

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

        private PatientPrescription GetPatientProfileInfo(int userId)
        {
            try
            {
                var patientInfo = _doctor.GetQueryResult("spBasicPatientProfile_Get " + userId);

                var result = patientInfo.Tables[0].Rows.OfType<DataRow>().Select(dr => new PatientPrescription()
                {
                    UserId = dr.Field<int>("UserId"),
                    UserTypeID = dr.Field<int>("UserTypeId"),
                    FirstName = dr.Field<string>("FirstName"),
                    LastName = dr.Field<string>("LastName"),
                    MiddleName = dr.Field<string>("MiddleName"),
                    Gender = dr.Field<string>("Gender"),
                    Address = dr.Field<string>("Address"),
                    City = dr.Field<string>("City"),
                    State = dr.Field<string>("State"),
                    Country = dr.Field<string>("Country"),
                    ZipCode = dr.Field<string>("ZipCode"),
                    DateOfBirth = String.Format("{0:MM/dd/yyyy}", dr.Field<DateTime?>("DateOfBirth")),
                    PhoneNumber = dr.Field<string>("PhoneNumber"),
                    Email = dr.Field<string>("Email"),
                    HealthInsurance = dr.Field<string>("HealthInsurance"),
                    PatientId = dr.Field<int?>("PatientId"),
                    InsurancePlanId = dr.Field<int?>("InsurancePlanID"),
                    InsuranceTypeId = dr.Field<int?>("InsuranceTypeId"),
                    CityStateZipCodeID = dr.Field<int?>("CityStateZipCodeID"),
                    AddressId = dr.Field<int?>("AddressId"),
                    BinNumber = dr.Field<string>("Bin_Number"),
                    MemberNumber = dr.Field<string>("Member_number"),
                    GroupNumber = dr.Field<string>("Group_number"),
                    PcnNumber = dr.Field<string>("PCN_number"),
                    IsPrimary = dr.Field<bool>("IsPrimary"),
                    RelationShipWithCardHolder = dr.Field<string>("RelationshipToPrimaryCardHolder"),
                    PrimaryCardHolderName = dr.Field<string>("PrimaryCardHolderName")

                }).FirstOrDefault();
                result.TransferPharmacy = new PharmacyAddress();
                result.Pharmacy = Session["PharmacyDetails"] as PharmacyAddress;

                return result;
            }
            catch
            {
                return null;
            }
        }

        private PatientPrescription GetTransferPharmacyDetail(PatientPrescription patientPrescription)
        {
            var transferPharamcy = Session["PharmacyDetailsByZipCode"] as List<PharmacyDetailsById>;
            var pharmacyAddress = transferPharamcy.Where(p => p.OrganisationId == patientPrescription.lstTransferPrescription[patientPrescription.lstTransferPrescription.Count - 1].TransFromPharmacyId).ToList();
            PharmacyAddress transfromPharmacy = pharmacyAddress.Select(x => new PharmacyAddress
            {
                OrgId = x.OrganisationId,
                OrgName = x.OrganisationName,
                OrgUserTypeId = x.OrganizationTypeID,
                Address1 = x.Address1,
                Address2 = x.Address2,
                AddressId = x.AddressId,
                AddressTypeID = x.AddressTypeID,
                City = x.CityName,
                State = x.StateName,
                Country = x.CountryName,
                Zipcode = x.ZipCode,
                PhoneNumber = x.PhoneNumber,

            }).FirstOrDefault();

            patientPrescription.TransferPharmacy = patientPrescription.Pharmacy;
            patientPrescription.Pharmacy = transfromPharmacy != null ? transfromPharmacy : patientPrescription.Pharmacy;
            return patientPrescription;
        }

        private void ClearPrescriptionSession()
        {
            Session.Remove("PharmacyDetailsByZipCode");
            Session.Remove("NewPatientPrescription");
            Session.Remove("RefillPrescription");
            Session.Remove("TransferPatientPrescription");
            Session.Remove("NewPresTableData");
            Session.Remove("TransPresTableData");
            Session.Remove("RefillPrescriptionData");
        }

        private void SendPrescriptionMail(PrescriptionEmail email)
        {
            string body = Common.ReadEmailTemplate(EmailTemplate.PrescriptionEmail);

            if (email.PrescriptionType == PrescriptionType.New || email.PrescriptionType == PrescriptionType.Refill)
            {
                var rxNumbers = email.RxNumber.Split(',').ToList().Distinct();
                foreach (var rx in rxNumbers)
                {
                    body = Common.ReadEmailTemplate(EmailTemplate.PrescriptionEmail);
                    body = body.Replace("{PatientName}", email.FirstName)
                            .Replace("{PharmacyName}", email.PharmacyName)
                            .Replace("{RXNumber}", rx)
                            .Replace("{DeliveryDate}", email.DeliveryDate)
                            .Replace("{displayTranfer}", "'display:none'");
                    SendMail.SendEmail(email.PatientEmail, "", "", "", body, email.Subject);
                }
            }
            else
            {
                body = body.Replace("{PatientName}", email.FirstName)
                               .Replace("{PharmacyName}", email.PharmacyName)
                               .Replace("{PharmacyFromName}", email.TransferFromPharmacyName)
                               .Replace("{RXNumber}", email.RxNumber)
                               .Replace("{Address}", email.Address)
                               .Replace("{DeliveryDate}", email.DeliveryDate)
                               .Replace("{display}", "'display:none'");
                SendMail.SendEmail(email.PatientEmail, "", "", "", body, email.Subject);
            }
        }

        private void SaveNewPrescription(ref string orgName, ref string npiValue)
        {
            string rxNumber = string.Empty;
            var newSave = Session["NewPatientPrescription"] as PatientPrescription;
            orgName = newSave.Pharmacy.OrgName;
            npiValue = newSave.Pharmacy.NPI;
            if (newSave != null && newSave.lstNewPrescriptionInfo != null)
            {
                foreach (var item in newSave.lstNewPrescriptionInfo)
                {
                    var saveData = _patient.ExecuteSQLQuery("spPatientNewPrescription_Create " +
                      "@UserId," +
                            "@PatientID," +
                            "@DoctorID," +
                            "@PharmacyID," +
                            "@OrganizationTypeId," +
                            "@UserTypeID," +
                            "@AddressId," +
                            "@PhoneNumber," +
                            "@DrugID," +
                            "@DrugStrengthID," +
                            "@RXNumber," +
                            "@PrescriptionNumber," +
                            "@RefillDate," +
                            "@Quantity," +
                            "@IsPrimary," +
                            "@RelationshipToPrimaryCardHolder," +
                            "@PrimaryCardHolderName," +
                            "@InsurancePlanID," +
                            "@Member_Number," +
                            "@Group_Number," +
                            "@BIN_Number," +
                            "@PCN_Number," +
                            "@DeliveryType," +
                            "@OrderStatus," +
                            "@CreatedBy," +
                            "@IsActive",
                            //    +  "@Success OUTPUT",
                            new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = newSave.UserId },
                                         new SqlParameter("PatientID", System.Data.SqlDbType.Int) { Value = newSave.PatientId },
                                         new SqlParameter("DoctorID", System.Data.SqlDbType.BigInt) { Value = item.DoctorId },
                                         new SqlParameter("PharmacyID", System.Data.SqlDbType.Int) { Value = newSave.OrgId },
                                         new SqlParameter("OrganizationTypeId", System.Data.SqlDbType.Int) { Value = newSave.Pharmacy.OrgUserTypeId },
                                         new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = newSave.UserTypeID },// (object)model.InsurancePlanId ?? DBNull.Value },
                                         new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = newSave.AddressId },
                                         new SqlParameter("PhoneNumber", System.Data.SqlDbType.NVarChar) { Value = (object)newSave.PhoneNumber ?? DBNull.Value },
                                         new SqlParameter("DrugID", System.Data.SqlDbType.Int) { Value = item.DrugId },
                                         new SqlParameter("DrugStrengthID", System.Data.SqlDbType.Int) { Value = item.DrugStrengthId },
                                         new SqlParameter("RXNumber", System.Data.SqlDbType.VarChar) { Value = item.RXNumber },
                                         new SqlParameter("PrescriptionNumber", System.Data.SqlDbType.VarChar) { Value = item.RXNumber }, ////doubt check with them
                                         new SqlParameter("RefillDate", System.Data.SqlDbType.DateTime) { Value = DateTime.ParseExact(item.RefillDate, "MM/dd/yyyy", null) },
                                         new SqlParameter("Quantity", System.Data.SqlDbType.Int) { Value = item.Quantity },
                                         new SqlParameter("IsPrimary", System.Data.SqlDbType.Bit) { Value = newSave.IsPrimary },
                                         new SqlParameter("RelationshipToPrimaryCardHolder", System.Data.SqlDbType.NVarChar) { Value = (object)newSave.RelationShipWithCardHolder ?? DBNull.Value },
                                         new SqlParameter("PrimaryCardHolderName", System.Data.SqlDbType.NVarChar) { Value = (object)newSave.PrimaryCardHolderName ?? DBNull.Value },
                                         new SqlParameter("InsurancePlanID", System.Data.SqlDbType.Int) { Value = newSave.InsurancePlanId },
                                         new SqlParameter("Member_Number", System.Data.SqlDbType.NVarChar) { Value = (object)newSave.MemberNumber ?? DBNull.Value },
                                         new SqlParameter("Group_Number", System.Data.SqlDbType.NVarChar) { Value = (object)newSave.GroupNumber ?? DBNull.Value },
                                         new SqlParameter("BIN_Number", System.Data.SqlDbType.NVarChar) { Value = (object)newSave.BinNumber ?? DBNull.Value },
                                         new SqlParameter("PCN_Number", System.Data.SqlDbType.NVarChar) { Value = (object)newSave.PcnNumber ?? DBNull.Value },
                                         new SqlParameter("DeliveryType", System.Data.SqlDbType.Int) { Value = newSave.IsDeliveryType },
                                         new SqlParameter("OrderStatus", System.Data.SqlDbType.VarChar) { Value = "Ordered" }, //// check with them
                                         new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = newSave.UserId },
                                         new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = 1 });

                    rxNumber += item.RXNumber + ",";
                    if (newSave.RefillDate == null)
                    {
                        newSave.RefillDate = item.RefillDate;
                    }
                }
                var email = new PrescriptionEmail
                {
                    FirstName = newSave.FirstName,
                    Subject = "Prescription New",
                    PatientEmail = newSave.Email,
                    PharmacyName = newSave.Pharmacy.OrgName,
                    RxNumber = rxNumber.Substring(0, rxNumber.Length - 1),
                    Address = newSave.Pharmacy.FullAddress,
                    DeliveryDate = newSave.RefillDate.ToString(),
                    PrescriptionType = PrescriptionType.New

                };
                SendPrescriptionMail(email);
            }
        }
        private void SaveRefillPrescription(ref string orgName, ref string npiValue)
        {
            string rxNumber = string.Empty;
            var refillSave = Session["RefillPrescription"] as PatientPrescription;
            orgName = refillSave.Pharmacy.OrgName;
            npiValue = refillSave.Pharmacy.NPI;
            if (refillSave != null && refillSave.lstExistingPrescriptionInfo != null)
            {
                foreach (var itemRefill in refillSave.lstExistingPrescriptionInfo)
                {
                    var saveData = _patient.ExecuteSQLQuery("spPatientRefillPrescription_Create " +
                            "@PatientPrescriptionID," +
                            "@PatientRefillId," +
                            "@PatientId," +
                            "@DrugID," +
                            "@AddressId," +
                            "@Quantity," +
                            "@OrderStatus," +
                            //"@OrderDate," +
                            "@DeliveryType," +
                            "@CreatedBy," +
                            "@IsActive",

                            new SqlParameter("PatientPrescriptionID", System.Data.SqlDbType.BigInt) { Value = itemRefill.PatientPrescriptionID },
                                         new SqlParameter("PatientRefillId", System.Data.SqlDbType.Int) { Value = itemRefill.PatientRefillId },
                                         new SqlParameter("PatientId", System.Data.SqlDbType.Int) { Value = itemRefill.PatientID },
                                         new SqlParameter("DrugID", System.Data.SqlDbType.Int) { Value = itemRefill.DrugId },// (object)model.InsurancePlanId ?? DBNull.Value },
                                         new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = refillSave.AddressId },
                                         new SqlParameter("Quantity", System.Data.SqlDbType.Int) { Value = itemRefill.Quantity },
                                         new SqlParameter("OrderStatus", System.Data.SqlDbType.VarChar) { Value = "Ordered" },
                                         //new SqlParameter("OrderDate", System.Data.SqlDbType.DateTime) { Value = DateTime.ParseExact(item.OrderDate, "MM/dd/yyyy", null) }
                                         new SqlParameter("DeliveryType", System.Data.SqlDbType.Int) { Value = refillSave.IsDeliveryType },
                                         new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = refillSave.UserId },
                                         new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = 1 });

                    rxNumber += itemRefill.RXNumber + ",";
                    if (refillSave.RefillDate == null)
                    {
                        refillSave.RefillDate = itemRefill.RefillDate.ToString();
                    }
                }
                var email = new PrescriptionEmail
                {
                    FirstName = refillSave.FirstName,
                    Subject = "Prescription Refill",
                    PatientEmail = refillSave.Email,
                    PharmacyName = refillSave.Pharmacy.OrgName,
                    RxNumber = rxNumber.Substring(0, rxNumber.Length - 1),
                    Address = refillSave.Pharmacy.FullAddress,
                    DeliveryDate = refillSave.RefillDate.ToString(),
                    PrescriptionType = PrescriptionType.Refill

                };
                SendPrescriptionMail(email);
            }
        }
        private void SaveTransferPrescription(ref string orgName, ref string npi)
        {
            var transferSave = Session["TransferPatientPrescription"] as PatientPrescription;
            orgName = transferSave.TransferPharmacy.OrgName;
            npi = transferSave.TransferPharmacy.NPI;
            if (transferSave != null && transferSave.lstTransferPrescription != null)
            {
                foreach (var itemTransfer in transferSave.lstTransferPrescription)
                {
                    var saveData = _patient.ExecuteSQLQuery("spPatientTransferPrescription_Create " +
                      "@UserId," +
                            "@PatientID," +
                            "@PharmacyID," +
                            "@UserTypeID," +
                            "@AddressId," +
                            "@PhoneNumber," +
                            "@DrugID," +
                            "@DrugStrengthID," +
                            "@TransferPharmacyID," +
                            "@Transfer_PrescriptionNumber," +
                            "@Transfer_Status," +
                            "@RefillDate," +
                            "@Quantity," +
                            "@IsPrimary," +
                            "@RelationshipToPrimaryCardHolder," +
                            "@PrimaryCardHolderName," +
                            "@InsurancePlanID," +
                            "@Member_Number," +
                            "@Group_Number," +
                            "@BIN_Number," +
                            "@PCN_Number," +
                            "@DeliveryType," +
                            "@OrderStatus," +
                            "@CreatedBy," +
                            "@IsActive",
                            //    +  "@Success OUTPUT",
                            new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = transferSave.UserId },
                                         new SqlParameter("PatientID", System.Data.SqlDbType.Int) { Value = transferSave.PatientId },
                                         new SqlParameter("PharmacyID", System.Data.SqlDbType.Int) { Value = transferSave.OrgId },
                                         new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = transferSave.UserTypeID },
                                         new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = transferSave.AddressId },
                                         new SqlParameter("PhoneNumber", System.Data.SqlDbType.NVarChar) { Value = (object)transferSave.PhoneNumber ?? DBNull.Value },
                                         new SqlParameter("DrugID", System.Data.SqlDbType.Int) { Value = itemTransfer.DrugId },
                                         new SqlParameter("DrugStrengthID", System.Data.SqlDbType.Int) { Value = itemTransfer.DrugStrengthID },
                                         new SqlParameter("TransferPharmacyID", System.Data.SqlDbType.Int) { Value = itemTransfer.TransFromPharmacyId },
                                         new SqlParameter("Transfer_PrescriptionNumber", System.Data.SqlDbType.VarChar) { Value = itemTransfer.TransferPrescriptionNumber },
                                         new SqlParameter("Transfer_Status", System.Data.SqlDbType.VarChar) { Value = "Requested" },
                                         new SqlParameter("RefillDate", System.Data.SqlDbType.DateTime) { Value = DateTime.ParseExact(itemTransfer.RefillDate, "MM/dd/yyyy", null) },
                                         new SqlParameter("Quantity", System.Data.SqlDbType.Int) { Value = itemTransfer.Quantity },
                                         new SqlParameter("IsPrimary", System.Data.SqlDbType.Bit) { Value = transferSave.IsPrimary },
                                         new SqlParameter("RelationshipToPrimaryCardHolder", System.Data.SqlDbType.NVarChar) { Value = (object)transferSave.RelationShipWithCardHolder ?? DBNull.Value },
                                         new SqlParameter("PrimaryCardHolderName", System.Data.SqlDbType.NVarChar) { Value = (object)transferSave.PrimaryCardHolderName ?? DBNull.Value },
                                         new SqlParameter("InsurancePlanID", System.Data.SqlDbType.Int) { Value = transferSave.InsurancePlanId },
                                         new SqlParameter("Member_Number", System.Data.SqlDbType.NVarChar) { Value = (object)transferSave.MemberNumber ?? DBNull.Value },
                                         new SqlParameter("Group_Number", System.Data.SqlDbType.NVarChar) { Value = (object)transferSave.GroupNumber ?? DBNull.Value },
                                         new SqlParameter("BIN_Number", System.Data.SqlDbType.NVarChar) { Value = (object)transferSave.BinNumber ?? DBNull.Value },
                                         new SqlParameter("PCN_Number", System.Data.SqlDbType.NVarChar) { Value = (object)transferSave.PcnNumber ?? DBNull.Value },
                                         new SqlParameter("DeliveryType", System.Data.SqlDbType.Int) { Value = transferSave.IsDeliveryType },
                                         new SqlParameter("OrderStatus", System.Data.SqlDbType.VarChar) { Value = "Ordered" }, //// check with them
                                         new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = transferSave.UserId },
                                         new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = 1 });

                    var email = new PrescriptionEmail
                    {
                        FirstName = transferSave.FirstName,
                        Subject = "Prescription Transfer",
                        PatientEmail = transferSave.Email,
                        PharmacyName = itemTransfer.TransToPharmacyName,
                        TransferFromPharmacyName = itemTransfer.TransFromPharmacyName,
                        RxNumber = itemTransfer.TransferPrescriptionNumber,
                        Address = transferSave.TransferPharmacy.FullAddress,
                        DeliveryDate = itemTransfer.RefillDate,
                        PrescriptionType = PrescriptionType.Transfer
                    };
                    SendPrescriptionMail(email);
                }
            }
        }

        #endregion
    }
}

