using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doctyme.Repository.Interface;
using Binke.Models;
using Binke.ViewModels;
using Binke.Utility;
using System.Transactions;
using Doctyme.Model;
using Binke.App_Helpers;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using Doctyme.Repository.Enumerable;
using System.Data;

namespace Binke.Controllers
{
    [Authorize]
    public class PharmacyController : Controller
    {
        private readonly IAddressService _address;
        private readonly IDoctorInsuranceService _insurance;
        private readonly IDocOrgStateLicensesService _stateLicense;
        private readonly IRepository _repo;
        private readonly IPharmacyService _pharmacy;
        private readonly IStateService _state;
        private readonly IUserService _appUser;
        private readonly IDrugService _drugService;
        private readonly ICityStateZipService _cityState;
        private ApplicationUserManager _userManager;
        private static string _PharmacySection = "Yes";

        private int _PharmacyId;
        public PharmacyController(IAddressService address, IPharmacyService pharmacy, IDoctorInsuranceService insurance, IDocOrgStateLicensesService stateLicense, IRepository repo, IStateService state, IUserService appUser, IDrugService drugService, ICityStateZipService cityState, ApplicationUserManager userManager)
        {
            _address = address;
            _pharmacy = pharmacy;
            _insurance = insurance;
            _stateLicense = stateLicense;
            _repo = repo;
            _state = state;
            _drugService = drugService;
            _appUser = appUser;
            _cityState = cityState;
            _userManager = userManager;
        }


        #region Admin Section
        //// GET: Pharmacy
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                return View();
            }
            else
            {
                return View("Error");
            }

        }

        #region Controller Common

        public JsonResult ValidateNPI(string NPI, int? OrganisationId, int? DoctorId)
        {
            bool status = true;
            int countX = 0;
            if (!string.IsNullOrEmpty(NPI))
            {
                if (OrganisationId > 0)
                {
                    //var result = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.OrganisationId != OrganisationId && x.IsDeleted == false);
                    var result = _repo.Find<Organisation>(x => x.NPI == NPI && x.OrganisationId != OrganisationId && x.IsDeleted == false);
                    countX = result != null ? 1 : 0;
                }
                else if (OrganisationId == 0 && DoctorId == null)
                {
                    //var result = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false);
                    var result = _repo.Find<Organisation>(x => x.NPI == NPI && x.IsDeleted == false);
                    countX = result != null ? 1 : 0;
                }

                if (DoctorId > 0)
                {
                    //var result = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false);
                    var result = _repo.Find<Doctor>(x => x.NPI == NPI && x.DoctorId != DoctorId && x.IsDeleted == false);
                    countX = result != null ? 1 : 0;
                }
                else if (OrganisationId == null && DoctorId == 0)
                {
                    //var result = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false);
                    var result = _repo.Find<Doctor>(x => x.NPI == NPI && x.IsDeleted == false);
                    countX = result != null ? 1 : 0;
                }

                if (countX > 0)
                    status = false;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //--CityState ZipCode AutoComplete
        [HttpPost]
        public JsonResult GetZipCode(string Prefix)
        {
            var zipcodes = _repo.ExecWithStoreProcedure<CityStateZipCodeViewModel>("spGetCityStateZipCodesAutoComplete @ZipCode",
                new SqlParameter("ZipCode", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(zipcodes.ToList(), JsonRequestBehavior.AllowGet);
        }

        //--CityState City AutoComplete
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
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spOrgInsurancePlanDropDownList_Get @OrganizationId, @InsuranceTypeId",
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ReferenceId },
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
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


        //--- Return CityStateInfoById
        public string GetOrganisationNameById(int id)
        {
            string result = "";

            var info = _repo.SQLQuery<OrganisationProfileViewModel>("spGetOrganizationInfoByID @orgnizationID", new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = id }).ToList();

            result = info[0].OrganisationName;

            return result;
        }


        //--Taxonomy Code AutoComplete
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

        //--- Return Taxonomy Code BY ID
        public string GetTaxonomyCodeById(int id, string type)
        {
            string result = "";

            var info = _repo.SQLQuery<TaxonomyCodeViewModel>("spGET_TaxonomyCode_ById @TaxonomyID", new SqlParameter("TaxonomyID", System.Data.SqlDbType.Int) { Value = id }).ToList();

            if (type == "code")
                result = info[0].Taxonomy_Code;

            if (type == "special")
                result = info[0].Specialization;

            return result;
        }

        //--- Return Taxonomy Codes BY IDS
        public string GetTaxonomyCodeByIds(string strIds)
        {
            string result = "";
            string[] IDArray = strIds.Split(',');

            foreach (var item in IDArray)
            {
                int ID = int.Parse(item.ToString());

                if (result == "")
                {
                    result = GetTaxonomyCodeById(ID, "code");
                }
                else
                {
                    result += ", " + GetTaxonomyCodeById(ID, "code");
                }
            }


            return result;
        }

        public JsonResult ValidateTaxonomyCode(string Taxonomy_Code, int? TaxonomyID)
        {
            bool status = true;
            int countX = 0;
            if (!string.IsNullOrEmpty(Taxonomy_Code))
            {
                if (TaxonomyID > 0)
                {
                    var result = _repo.All<Taxonomy>().Where(x => x.TaxonomyID == TaxonomyID && x.Taxonomy_Code == Taxonomy_Code && x.TaxonomyID != TaxonomyID && x.IsDeleted == false);
                    countX = result.Count();
                }
                else
                {
                    var result = _repo.All<Taxonomy>().Where(x => x.Taxonomy_Code == Taxonomy_Code && x.IsDeleted == false);
                    countX = result.Count();
                }

                if (countX > 0)
                    status = false;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateProcedureName(string Name, int OrganisationId, int? OrgProcedureId)
        {
            bool status = true;
            int ID = 0;

            if (OrgProcedureId > 0)
                ID = (int)OrgProcedureId;

            var result = _repo.ExecWithStoreProcedure<OrgProcedureUpdateViewModel>("spOrgProcedure_ValidateName @OrgProcedureId, @ReferenceID, @Name",
                new SqlParameter("OrgProcedureId", System.Data.SqlDbType.Int) { Value = ID },
                new SqlParameter("ReferenceID", System.Data.SqlDbType.Int) { Value = OrganisationId },
                new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = Name }
                );

            status = result.First().status;

            return Json(status, JsonRequestBehavior.AllowGet);
        }


        //--Patient Name AutoComplete
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


        //public JsonResult ValidateBookingDate(string SlotDate, string SlotTime, int OrganisationId, int? SlotId)
        //{
        //    bool status = true;
        //    int countX = 0;
        //    if (!string.IsNullOrEmpty(SlotDate) && !string.IsNullOrEmpty(SlotTime))
        //    {
        //        if (SlotId > 0)
        //        {
        //            var result = _repo.All<Slot>().Where(x => x.ReferenceId == OrganisationId && x.SlotDate == SlotDate && x.SlotTime == SlotTime &&  x.SlotId != SlotId && x.IsDeleted == false);
        //            countX = result.Count();
        //        }
        //        else
        //        {
        //            var result = _repo.All<Slot>().Where(x => x.ReferenceId == OrganisationId && x.SlotDate == SlotDate && x.SlotTime == SlotTime && x.IsDeleted == false);
        //            countX = result.Count();
        //        }

        //        if (countX > 0)
        //            status = false;
        //    }

        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ValidateBookingTime(string SlotTime, string SlotDate, int OrganisationId, int? SlotId)
        {
            bool status = true;
            int countX = 0;
            if (!string.IsNullOrEmpty(SlotTime) && !string.IsNullOrEmpty(SlotDate))
            {
                if (SlotId > 0)
                {
                    var result = _repo.All<Slot>().Where(x => x.ReferenceId == OrganisationId && x.SlotDate == SlotDate && x.SlotTime == SlotTime && x.SlotId != SlotId && x.IsDeleted == false);
                    countX = result.Count();
                }
                else
                {
                    var result = _repo.All<Slot>().Where(x => x.ReferenceId == OrganisationId && x.SlotDate == SlotDate && x.SlotTime == SlotTime && x.IsDeleted == false);
                    countX = result.Count();
                }

                if (countX > 0)
                    status = false;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBookingDate(string SlotDate)
        {
            bool status = true;

            //if (!string.IsNullOrEmpty(SlotDate))
            //{
            //    string[] strArr = SlotDate.Split('-');
            //    string strFormattedDate = strArr[2] + "-" + strArr[1] + "-" + strArr[0];
            //    DateTime dt1 = DateTimeOffset.Parse(strFormattedDate).UtcDateTime;
            //    if(dt1<DateTime.UtcNow)
            //        status = false;
            //}

            return Json(status, JsonRequestBehavior.AllowGet);
        }


        //--Drug Info AutoComplete
        [HttpPost]
        public JsonResult GetDrugInfo(string Prefix)
        {
            var drugList = _repo.ExecWithStoreProcedure<OrgPatientOrderDrugAutocompleteViewModel>("spGetDrugInfoAutoComplete @DrugName",
                new SqlParameter("DrugName", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(drugList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Pharmacy Profile

        public ActionResult Profile(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.NPI != null && x.IsDeleted == false && x.UserId == userId);
                    if (pharmacyInfo != null)
                    {
                        TempData["PharmacyData"] = "Yes";
                        ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                        Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                    }
                    else
                    {
                        ViewBag.NPI = NPI;
                        TempData["PharmacyData"] = "Yes";
                        ViewBag.PharmacyID = 0;
                        Session["PharmacyID"] = 0;
                    }
                }
                else
                {
                    return View("Error");
                }
            }
            return View();
        }

        [Route("GetPharmacyList/{flag}")]
        public ActionResult GetPharmacyList(bool flag, JQueryDataTableParamModel param)
        {
            //var outParam = new SqlParameter();
            //outParam.ParameterName = "TotalRecordCount";
            //outParam.SqlDbType = System.Data.SqlDbType.Int;
            //outParam.Direction = System.Data.ParameterDirection.Output;


            var allPharmacys = _repo.ExecWithStoreProcedure<OrganisationProfileViewModel>("spOrganisation_GetByAddressTypeID @Search, @UserTypeID, @OrganizationTypeID, @AddressTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("AddressTypeID", System.Data.SqlDbType.Int) { Value = 12 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allPharmacys
                .Select(x => new OrganisationProfileListViewModel()
                {
                    Id = x.OrganisationId,
                    PharmacyName = x.OrganisationName,
                    FullName = (x.AuthorizedOfficialFirstName != null ? x.AuthorizedOfficialFirstName : "") + ((x.AuthorizedOfficialFirstName != null && x.AuthorizedOfficialLastName != null) ? (", " + x.AuthorizedOfficialLastName) : " " + x.AuthorizedOfficialLastName),
                    NPI = x.NPI,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    EnabledBooking = x.EnabledBooking,
                    IsDeleted = x.IsDeleted,
                    totRows = x.TotalRecordCount
                }).ToList();

            int TotalRecordCount = 0;

            if (result.Count > 0)
                TotalRecordCount = result[0].totRows;


            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public PartialViewResult PharmacyProfile(int? id, bool flag = false)
        {

            if (User.IsInRole("Pharmacy"))
            {
                int currentUserID = User.Identity.GetUserId<int>();

                var userInfo = _appUser.GetById(currentUserID);
                string NPI = userInfo.Uniquekey;
                ViewBag.NPI = NPI;

                var info = _repo.ExecWithStoreProcedure<GetOrgIDViewModel>("spGetOrganisationId_ByUserId @UserID, @UserTypeID, @OrganizationTypeID",
                       new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = currentUserID },
                       new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                       new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 }
                       ).ToList();

                if (info.Count() > 0)
                {
                    int ORGID = info[0].OrganisationId;
                    id = ORGID;
                }
            }


            if (id > 0)
            {
                ViewBag.PharmacyID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrganisationProfileViewModel>("spGetOrganizationInfoByID @orgnizationID",
                    new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = id }
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

                if (orgInfo == null)
                {
                    orgInfo = new OrganisationProfileViewModel();
                    orgInfo.UserTypeID = 3;
                    orgInfo.OrganizationTypeID = 1005;
                }
                Session["PharmacyName"] = orgInfo.OrganisationName;
                Session["PharmacyProfile"] = orgInfo.LogoFilePath;

                if (flag == true)
                    orgInfo.IsViewMode = true;

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyProfile.cshtml", orgInfo);

            }
            else
            {
                ViewBag.PharmacyID = 0;
                var orgInfo = new OrganisationProfileViewModel();
                orgInfo.UserTypeID = 3;
                orgInfo.OrganizationTypeID = 1005;
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyProfile.cshtml", orgInfo);
            }
        }

        [HttpPost, Route("AddEditPharmacyProfile"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyProfile(OrganisationProfileViewModel model, HttpPostedFileBase Image1)
        {
            try
            {
                string imagePath = "";


                string strShortDescription = "";
                string strLongDescription = "";
                if (!string.IsNullOrEmpty(model.ShortDescription))
                {
                    strShortDescription = System.Uri.UnescapeDataString(model.ShortDescription);
                    model.ShortDescription = strShortDescription;
                }
                if (!string.IsNullOrEmpty(model.LongDescription))
                {
                    strLongDescription = System.Uri.UnescapeDataString(model.LongDescription);
                    model.LongDescription = strLongDescription;
                }

                int curUserId = 0;

                if (!User.IsInRole("Admin"))
                {
                    curUserId = User.Identity.GetUserId<int>();
                }
                else
                {
                    if (model.UserId > 0)
                    {
                        curUserId = (int)model.UserId;
                    }
                }
                if (Image1 != null && Image1.ContentLength > 0)
                {
                    DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/PharmacySiteImages/"));
                    if (!dir.Exists)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/PharmacySiteImages"));
                    }

                    string extension = Path.GetExtension(Image1.FileName);
                    string newImageName = "Doctor-" + DateTime.Now.Ticks.ToString() + extension;

                    var path = Path.Combine(Server.MapPath("~/Uploads/PharmacySiteImages"), newImageName);
                    Image1.SaveAs(path);

                    imagePath = newImageName;
                    model.LogoFilePath = imagePath;
                }

                if (model.OrganisationId > 0 && imagePath == "")
                {
                    imagePath = model.LogoFilePath;
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
                                      new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = curUserId },
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
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
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
                Session["PharmacyProfile"] = model.LogoFilePath;
                return Json(new JsonResponse() { Status = 1, Message = "Pharmacy profile save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy profile saving Error.." });
            }

        }

        [Route("ActiveDeActivePharmacy/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActivePharmacy(bool flag, int id)
        {
            try
            {
                bool DelFlag = false;
                if (flag == false)
                    DelFlag = true;


                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganisation_ActiveDeActive @OrganisationId, @IsActive, @IsDelete, @ModifiedBy",
                    new SqlParameter("OrganisationId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = flag },
                    new SqlParameter("IsDelete", System.Data.SqlDbType.Bit) { Value = DelFlag },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy profile {(flag ? "reactivated" : "deleted")} Error.." });
            }


        }
        [HttpPost]
        public ActionResult UpdateSwitch(SwitchUpdateViewModel switchUpdateViewModel)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter(switchUpdateViewModel.FieldToUpdateName, SqlDbType.VarChar) { Value = switchUpdateViewModel.FieldToUpdateValue });
            try
            {
                _pharmacy.ExecuteSqlCommandForUpdate(switchUpdateViewModel.TableName, switchUpdateViewModel.PrimaryKeyName, switchUpdateViewModel.PrimaryKeyValue, parameters);
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

        #endregion

        #region Pharmacy Insurance

        public ActionResult Insurance(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetPharmacyInsuranceList/{flag}/{id}")]
        public ActionResult GetPharmacyInsuranceList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var result = _repo.ExecWithStoreProcedure<OrgInsuranceViewModel>("spOrganisationInsurances_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    ).Select(x => new OrgInsuranceListViewModel
                    {
                        DocOrgInsuranceId = x.DocOrgInsuranceId,
                        OrganisationName = x.ReferenceName,
                        Name = x.Name,
                        InsuranceTypeId = x.InsuranceTypeId,
                        TypeName = x.TypeName,
                        StateId = x.StateId,
                        InsuranceIdentifierId = x.InsuranceIdentifierId,
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


        //---- Get Insurance Details

        public PartialViewResult PharmacyInsurance(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get");
            ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name");

            var planList = new List<InsurancePlanDropDownViewModel>();
            ViewBag.planList = new SelectList(planList, "InsuranceTypeId", "Name");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgLicenseInfo = _repo.ExecWithStoreProcedure<OrgInsuranceViewModel>("spOrganisationInsurances_GetById @DocOrgInsuranceId",
                    new SqlParameter("DocOrgInsuranceId", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrgInsuranceUpdateViewModel
                    {
                        DocOrgInsuranceId = x.DocOrgInsuranceId,
                        OrganisationId = x.ReferenceId,
                        InsurancePlanId = x.InsurancePlanId,
                        OrganizatonTypeID = 1005,
                        OrganisationName = x.ReferenceName,
                        UserTypeID = x.UserTypeID,
                        Name = x.Name,
                        StateId = x.StateId,
                        InsuranceIdentifierId = x.InsuranceIdentifierId,
                        UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                        InsuranceTypeId = x.InsuranceTypeId,
                        IsDeleted = x.IsDeleted,
                        IsActive = x.IsActive
                    }).First();

                ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name", orgLicenseInfo.InsuranceTypeId);
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyInsurance.cshtml", orgLicenseInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgLicenseInfo = new OrgInsuranceUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyInsurance.cshtml", orgLicenseInfo);
            }
        }


        //-- Delete Pharmacy Insurance
        [Route("DeletePharmacyInsurance/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyInsurance(int id)
        {
            try
            {

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizatonInsurance_Remove @DocOrgInsuranceId, @ModifiedBy",
                    new SqlParameter("DocOrgInsuranceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Insurance Info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Insurance Info deleted Error.." });
            }
        }


        //--- Add/Edit Insurance
        [HttpPost, Route("AddEditPharmacyInsurance"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyInsurance(OrgInsuranceUpdateViewModel model)
        {

            try
            {
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
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }
                                      );





                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy Insurance info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy Insurance info saving Error.." });
            }

        }

        #endregion

        #region Pharmacy Address

        // GET: StateLicense 
        public ActionResult Address(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetPharmacyOfficeLocationList/{flag}/{id}")]
        public ActionResult GetPharmacyOfficeLocationList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allPharmacyAddress = _repo.ExecWithStoreProcedure<OrganisationAddressViewModel>("spAddress_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
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


        //---- Get Address Details

        public PartialViewResult PharmacyAddress(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

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
                    }).First();

                ViewBag.AddressTypes = new SelectList(addressTypes.OrderBy(o => o.AddressTypeId), "AddressTypeId", "Name", orgAddressInfo.AddressTypeId);
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAddress.cshtml", orgAddressInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganisationAddressViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAddress.cshtml", orgAddressInfo);
            }
        }




        [HttpPost, Route("AddEditPharmacyAddress"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyAddress(OrganisationAddressViewModel model)
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
                                      new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeID },
                                      new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = model.AddressId > 0 ? User.Identity.GetUserId<int>() : (object)DBNull.Value }
                                  );

                return Json(new JsonResponse() { Status = 1, Message = "Pharmacy address info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy address info saving Error.." });
            }

        }

        [Route("DeletePharmacyAddress/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyAddress(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_Delete @AddressId, @ModifiedBy",
                    new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Address has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Address deleted Error.." });
            }


        }


        #endregion

        #region Pharmacy State License
        // GET: StateLicense 
        public ActionResult StateLicense(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetPharmacyStateLicenseList/{flag}/{id}")]
        public ActionResult GetPharmacyStateLicenseList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var result = _repo.ExecWithStoreProcedure<OrgStateLicenseViewModel>("spDocOrgStateLicenses_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    ).Select(x => new OrgStateLicenseListViewModel
                    {
                        DocOrgStateLicenseId = x.DocOrgStateLicense,
                        OrganisationName = x.ReferenceName,
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

        public PartialViewResult PharmacyStateLicense(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            var stateList = _repo.ExecWithStoreProcedure<StateDropDownViewModel>("spCityStateZip_US_AllState");
            ViewBag.stateList = new SelectList(stateList.OrderBy(o => o.State), "State", "State");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgLicenseInfo = _repo.ExecWithStoreProcedure<OrgStateLicenseViewModel>("spDocOrgStateLicenses_GetById @DocOrgStateLicense",
                    new SqlParameter("DocOrgStateLicense", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrgStateLicenseUpdateViewModel
                    {
                        DocOrgStateLicenseId = x.DocOrgStateLicense,
                        OrganisationId = x.ReferenceId,
                        OrganizatonTypeID = 1005,
                        OrganisationName = x.ReferenceName,
                        UserTypeID = x.UserTypeID,
                        HealthCareProviderTaxonomyCode = x.HealthCareProviderTaxonomyCode,
                        ProviderLicenseNumber = x.ProviderLicenseNumber,
                        ProviderLicenseNumberStateCode = x.ProviderLicenseNumberStateCode,
                        HealthcareProviderPrimaryTaxonomySwitch = x.HealthcareProviderPrimaryTaxonomySwitch,
                        IsDeleted = x.IsDeleted,
                        IsActive = x.IsActive
                    }).First();

                ViewBag.stateList = new SelectList(stateList.OrderBy(o => o.State), "State", "State", orgLicenseInfo.ProviderLicenseNumberStateCode);
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyStateLicense.cshtml", orgLicenseInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgLicenseInfo = new OrgStateLicenseUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyStateLicense.cshtml", orgLicenseInfo);
            }
        }

        //-- Active DeActive Pharmacy State License
        [Route("ActiveDeActivePharmacyStateLicense/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActivePharmacyStateLicense(bool flag, int id)
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

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy State License has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy State License {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }

        //--Update State License

        [HttpPost, Route("AddEditPharmacyStateLicense"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyStateLicense(OrgStateLicenseUpdateViewModel model)
        {

            try
            {
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
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive ?? false },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 3 }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy State License info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy State License info saving Error.." });
            }

        }

        #endregion

        #region Pharmacy Images

        // GET: Images 
        public ActionResult Images(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }
            return View();
        }

        [Route("GetPharmacyImageList/{flag}/{id}")]
        public ActionResult GetPharmacyImageList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var result = _repo.ExecWithStoreProcedure<OrgSiteImageViewModel>("spOrganizatonSiteImage_Get @Search, @ReferenceId, @UserTypeID, @OrganizatonTypeID, @PageIndex, @PageSize, @Sort",
                   new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                   new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                   new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                   new SqlParameter("OrganizatonTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
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

        //---- Get Site Image Details

        public PartialViewResult PharmacySiteImages(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgSiteImageViewModel>("spOrganizatonSiteImage_GetById @SiteImageId",
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

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyImages.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgSiteImageUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyImages.cshtml", orgInfo);
            }
        }

        //---- Add Edit Pharmacy Site Images

        [HttpPost, Route("AddEditPharmacySiteImage"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacySiteImage(OrgSiteImageUpdateViewModel model, HttpPostedFileBase Image1)
        {

            try
            {
                if (model.OrganisationId > 0)
                {
                    string imagePath = "";

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/PharmacySiteImages/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/PharmacySiteImages"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "Pharmacy-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/PharmacySiteImages"), newImageName);
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
                                          new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                          new SqlParameter("ImagePath", System.Data.SqlDbType.VarChar) { Value = (object)(imagePath) ?? DBNull.Value },
                                          new SqlParameter("IsProfile", System.Data.SqlDbType.Bit) { Value = (object)model.IsProfile ?? DBNull.Value },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = model.UserTypeID },
                                          new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = model.Name }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy site image info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Pharmacy Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy site image info saving Error.." });
            }

        }

        //-- Delete Pharmacy Site Image
        [Route("DeletePharmacySiteImage/{id}/{ImgName}")]
        [HttpPost]
        public JsonResult DeletePharmacySiteImage(int id, string ImgName)
        {
            try
            {
                if (ImgName != null)
                {
                    var path = Server.MapPath("~/Uploads/PharmacySiteImages/" + ImgName);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizatonSiteImage_Remove @SiteImageId, @ModifiedBy",
                    new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy site image has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy site image deleted Error.." });
            }
        }

        #endregion

        #region Pharmacy Reviews

        // GET: Review
        public ActionResult Reviews(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetPharmacyReviewList/{flag}/{id}")]
        public ActionResult GetPharmacyReviewList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var result = _repo.ExecWithStoreProcedure<OrgReviewViewModel>("spReview_Get @Search, @ReferenceId, @UserTypeID, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                   new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                   new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                   new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                   new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                   new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                   new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                   new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                   ).Select(x => new OrgReviewListViewModel
                   {
                       ReviewId = x.ReviewId,
                       OrganisationName = x.RefrenceName,
                       OrganisationId = x.ReferenceId,
                       Description = x.Description,
                       Rating = x.Rating,
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


        //---- Get Review Details

        public PartialViewResult PharmacyReviews(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgReviewViewModel>("spReview_GetById @ReviewId",
                    new SqlParameter("ReviewId", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrgReviewUpdateViewModel
                    {
                        ReviewId = x.ReviewId,
                        OrganisationId = x.ReferenceId,
                        OrganisationName = x.RefrenceName,
                        OrganizatonTypeID = x.OrganizatonTypeID,
                        Description = x.Description,
                        Rating = x.Rating,
                        IsActive = x.IsActive
                    }).First();

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyReviews.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgReviewUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyReviews.cshtml", orgInfo);
            }
        }

        //-- Delete Pharmacy Amenity Option
        [Route("DeletePharmacyReview/{id}")]
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

        //--- Add/Edit Review
        [HttpPost, Route("AddEditPharmacyReview"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyReview(OrgReviewUpdateViewModel model)
        {

            try
            {
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
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                          new SqlParameter("Doctor_DoctorId", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                          new SqlParameter("SeniorCare_SeniorCareId", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }
                                      );





                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy Review info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy Review info saving Error.." });
            }

        }



        #endregion

        #region Amenity Options
        // GET: StateLicense 
        public ActionResult AmenityOptions(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetPharmacyAmenityOptionsList/{flag}/{id}")]
        public ActionResult GetPharmacyAmenityOptionsList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allPharmacy = _repo.ExecWithStoreProcedure<OrganizationAmenityOptionListViewModel>("spAllOrganizationAmenityOption_Get @Search, @UserTypeID, @OrganizationID, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allPharmacy
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

            ViewBag.PharmacyID = id;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        //---- Get AmenityOptions Details
        public PartialViewResult PharmacyAmenityOptions(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];
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

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAmenityOptions.cshtml", orgFeaturedInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganizationAmenityOptionsUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAmenityOptions.cshtml", orgAddressInfo);
            }
        }
        public PartialViewResult PharmacyViewAmenityOptions(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];
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

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAmenityOptions.cshtml", orgFeaturedInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganizationAmenityOptionsUpdateViewModel();
                orgAddressInfo.IsViewMode = true;
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAmenityOptions.cshtml", orgAddressInfo);
            }
        }

        //-- Delete Pharmacy Amenity Option
        [Route("DeletePharmacyAmenityOptions/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyAmenityOptions(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAllOrganizationAmenityOption_Delete @OrganizationAmenityOptionID, @ModifiedBy",
                    new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Amenity Option has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Amenity Option deleted Error.." });
            }
        }

        [HttpPost, Route("AddEditPharmacyAmenityOptions"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyAmenityOptions(OrganizationAmenityOptionsUpdateViewModel model, HttpPostedFileBase Image1)
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
                        string newImageName = "AmenityOpt-Pharmacy-" + DateTime.Now.Ticks.ToString() + extension;

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





                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy Amenity Option info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name, Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy Amenity Option info saving Error.." });
            }

        }

        #endregion

        #region Opening Hours
        public ActionResult OpeningHours(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetPharmacyOpeningHoursList/{flag}/{id}")]
        public ActionResult GetPharmacyOpeningHoursList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allPharmacy = _repo.ExecWithStoreProcedure<OrgOpeningHoursViewModel>("spOpeningHour_Get @Search, @OrganizationID, @OrganizationTypeID, @UserTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

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

        public PartialViewResult PharmacyOpeningHours(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            if (id > 0)
            {
                ViewBag.ID = id;

                ViewBag.OrgName = GetOrganisationNameById((int)id);

                var orgInfo = _repo.ExecWithStoreProcedure<OrgOpeningHoursUpdateViewModel>("spOrgOpeningHour_GetByOrgID @OrganizationID",
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id }
                    ).FirstOrDefault();

                var info2 = _repo.ExecWithStoreProcedure<OrgOpeningHoursViewModel>("spOrgOpeningHour_GetByOrgID @OrganizationID",
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id }
                    ).ToList();


                ViewBag.Items = info2;
                ViewBag.ItemCount = info2.Count;



                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyOpeningHours.cshtml", orgInfo);
            }
            else
            {

                id = id == null ? ViewBag.PharmacyID : ViewBag.PharmacyID;
                ViewBag.OrgName = GetOrganisationNameById((int)id);
                ViewBag.ID = id;
                var orgInfo = _repo.ExecWithStoreProcedure<OrgOpeningHoursUpdateViewModel>("spOrgOpeningHour_GetByOrgID @OrganizationID",
                new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id }
                ).FirstOrDefault();

                var info2 = _repo.ExecWithStoreProcedure<OrgOpeningHoursViewModel>("spOrgOpeningHour_GetByOrgID @OrganizationID",
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id }
                    ).ToList();


                ViewBag.Items = info2;
                ViewBag.ItemCount = info2.Count;
                if (orgInfo == null)
                {
                    ViewBag.ID = 0;
                    orgInfo = new OrgOpeningHoursUpdateViewModel();
                    ViewBag.OrgName = null;
                }


                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyOpeningHours.cshtml", orgInfo);
            }
        }


        //--- Update Pharmacy Opening Hours

        [HttpPost, Route("AddEditPharmacyOpeningHours"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyOpeningHours(OrgOpeningHoursUpdateViewModel model)
        {
            try
            {

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
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                              new SqlParameter("Comments", System.Data.SqlDbType.NVarChar) { Value = (object)model.Comments[i] ?? DBNull.Value }
                                          );

                        continue;
                    }

                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy Opening Hours info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Pharmacy Info Not Found! Should be select Pharmacy Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy OpeningHours info saving Error.." });
            }

        }


        //-- Delete Pharmacy Featured
        [Route("DeletePharmacyOpeningHours/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyOpeningHours(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrgOpeningHour_Delete @OpeningHourID, @ModifiedBy",
                    new SqlParameter("OpeningHourID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy opening hours info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy opening hours info deleted Error.." });
            }
        }


        #endregion

        #region Featured

        // GET: Featuerd 
        public ActionResult Featured(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                    Session["PharmacyName"] = pharmacyInfo.OrganisationName;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetPharmacyFeaturedList/{flag}/{id}")]
        public ActionResult GetPharmacyFeaturedList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allPharmacyFeatured = _repo.ExecWithStoreProcedure<OrganisationFeaturedViewModel>("spFeatured_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allPharmacyFeatured
                .Select(x => new OrganisationFeaturedListViewModel()
                {
                    FeaturedId = x.FeaturedId,
                    OrganisationId = x.ReferenceId,
                    OrganisationName = x.ReferenceName,
                    ProfileImage = x.ProfileImage,
                    Title = x.Title,
                    Description = x.Description,
                    ZipCode = x.ZipCode,
                    City = x.City,
                    strStartDate = x.StartDate.ToDefaultFormate(),
                    strEndDate = x.EndDate != null ? x.EndDate.ToDefaultFormate() : null,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate != null ? x.UpdatedDate.ToDefaultFormate() : null,
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




        //---- Get Featured Details

        public PartialViewResult PharmacyFeatured(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            var advertisementLocations = _repo.ExecWithStoreProcedure<AdvertisementLocationDropDownViewModel>("spAdvertisementLocation @Activity",
                new SqlParameter("Activity", System.Data.SqlDbType.NVarChar) { Value = "List" });

            ViewBag.AdvertisementLocations = new SelectList(advertisementLocations.OrderBy(o => o.AdvertisementLocationId), "AdvertisementLocationId", "Name");

            var lst1 = new List<CityStateInfoByZipCodeViewModel>();

            ViewBag.cityList = new SelectList(lst1, "City", "City");
            ViewBag.stateList = new SelectList(lst1, "State", "State");
            ViewBag.countryList = new SelectList(lst1, "Country", "Country");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgFeaturedInfo = _repo.ExecWithStoreProcedure<OrganisationFeaturedViewModel>("spGetFeaturedInfoByID @FeaturedID",
                    new SqlParameter("FeaturedID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrganisationFeaturedAddEditViewModel
                    {
                        FeaturedId = x.FeaturedId,
                        OrganisationId = x.ReferenceId,
                        OrganizationTypeID = 1005,
                        OrganisationName = GetOrganisationNameById(x.ReferenceId),
                        UserTypeID = x.UserTypeID,
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
                    }).First();

                ViewBag.AdvertisementLocations = new SelectList(advertisementLocations.OrderBy(o => o.AdvertisementLocationId), "AdvertisementLocationId", "Name", orgFeaturedInfo.AdvertisementLocationID);
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyFeatured.cshtml", orgFeaturedInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganisationFeaturedAddEditViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyFeatured.cshtml", orgAddressInfo);
            }
        }


        //-- Delete Pharmacy Featured
        [Route("DeletePharmacyFeatured/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyFeatured(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spFeatured_Delete @FeaturedId, @ModifiedBy",
                    new SqlParameter("FeaturedId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Featured has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Featured deleted Error.." });
            }
        }

        [HttpPost, Route("AddEditPharmacyFeatured"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyFeatured(OrganisationFeaturedAddEditViewModel model, HttpPostedFileBase Image1)
        {

            try
            {
                if (model.OrganisationId > 0 && model.CityStateZipCodeID > 0 && model.FeaturdStartDate != null)
                {
                    string imagePath = "";

                    if (Image1 != null && Image1.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/FeaturedPharmacy/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/FeaturedPharmacy"));
                        }

                        string extension = Path.GetExtension(Image1.FileName);
                        string newImageName = "Featured-Pharmacy-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/FeaturedPharmacy"), newImageName);
                        Image1.SaveAs(path);

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
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("TotalImpressions", System.Data.SqlDbType.Int) { Value = (object)model.TotalImpressions ?? DBNull.Value },
                                          new SqlParameter("PaymentTypeID", System.Data.SqlDbType.Int) { Value = (object)model.PaymentTypeID ?? DBNull.Value },
                                          new SqlParameter("AdvertisementLocationID", System.Data.SqlDbType.Int) { Value = (object)model.AdvertisementLocationID ?? DBNull.Value },
                                          new SqlParameter("Title", System.Data.SqlDbType.NVarChar) { Value = (object)model.Title ?? DBNull.Value },
                                          new SqlParameter("CityStateZipCodeID", System.Data.SqlDbType.Int) { Value = model.CityStateZipCodeID }
                                      );





                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy featured info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name, Start date, Zip code and City" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy featured info saving Error.." });
            }

        }

        #endregion

        #region Social Media

        // GET: Social Media 
        public ActionResult SocialMedia(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetPharmacySocialMediaList/{flag}/{id}")]
        public ActionResult GetPharmacySocialMediaList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var socialMediaInfo = _repo.ExecWithStoreProcedure<OrgSocialMediaViewModel>("spSocialMedia_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
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


        //---- Get Social Media Details

        public PartialViewResult PharmacySocialMedia(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgSocialMediaInfo = _repo.ExecWithStoreProcedure<OrgSocialMediaViewModel>("spSocialMedia_GetById @SocialMediaId",
                    new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrgSocialMediaUpdateViewModel
                    {
                        SocialMediaId = x.SocialMediaId,
                        OrganisationId = (int)x.ReferenceId,
                        OrganizationTypeID = 1005,
                        OrganisationName = GetOrganisationNameById((int)x.ReferenceId),
                        UserTypeID = 3,
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


                return PartialView(@"/Views/Pharmacy/Partial/_PharmacySocialMedia.cshtml", orgSocialMediaInfo);
            }
            else
            {
                ViewBag.ID = ViewBag.PharmacyID;

                var orgSocialMediaInfo = _repo.ExecWithStoreProcedure<OrgSocialMediaViewModel>("spSocialMedia_GetById @SocialMediaId",
                    new SqlParameter("SocialMediaId", System.Data.SqlDbType.Int) { Value = ViewBag.PharmacyID }
                    ).Select(x => new OrgSocialMediaUpdateViewModel
                    {
                        SocialMediaId = x.SocialMediaId,
                        OrganisationId = (int)x.ReferenceId,
                        OrganizationTypeID = 1005,
                        OrganisationName = GetOrganisationNameById((int)x.ReferenceId),
                        UserTypeID = 3,
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
                {
                    orgSocialMediaInfo = new OrgSocialMediaUpdateViewModel();
                }
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacySocialMedia.cshtml", orgSocialMediaInfo);
            }
        }


        [HttpPost, Route("AddEditPharmacySocialMedia"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacySocialMedia(OrgSocialMediaUpdateViewModel model)
        {
            try
            {
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
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive ?? false },
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 3 },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy social media info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Pharmacy Info Not Found! Should be select Pharmacy Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy social media info saving Error.." });
            }

        }


        //-- Active DeActive Pharmacy Social Media
        [Route("ActiveDeActivePharmacySocialMedia/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActivePharmacySocialMedia(bool flag, int id)
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

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy social media info has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy social media info {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }

        #endregion
        #region PharamcyAccount
        public ActionResult UserAccount(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        public PartialViewResult PharamcyAccount(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgAccount = _repo.ExecWithStoreProcedure<RegisterJsonModel>("sprGetUserAccountDetailById @Id",
                    new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId() }
                    ).FirstOrDefault();

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(orgAccount.RegisterViewModels);



                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAccount.cshtml", obj);
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

                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(orgAccount.RegisterViewModels != null ? orgAccount.RegisterViewModels : "");

                }

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyAccount.cshtml", obj);
            }
        }
        #endregion

        [HttpPost, Route("AddEditPharmacyAccount"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyAccount(RegisterViewModel model)
        {
            ViewBag.PharmacyID = Convert.ToInt32(Session["PharmacyID"]);
            if (model.ConfirmPassword != model.Password && !string.IsNullOrEmpty(model.ConfirmPassword))
            {
                return Json(new JsonResponse() { Status = 0, Message = "Password and Confirm Password didn't Matched." });
            }
            else
            {
                var code = _userManager.GeneratePasswordResetToken(User.Identity.GetUserId<int>());
                var result = _userManager.ResetPassword(User.Identity.GetUserId<int>(), code, model.Password);
            }
            try
            {

                model.UserType = "3";
                if (ViewBag.PharmacyID > 0)
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
                        UserTypeId = 3,
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
                                            new SqlParameter("Uniquekey", System.Data.SqlDbType.VarChar) { Value = model.Uniquekey == null ? "" : model.Uniquekey }



                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Account Info saved successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Pharmacy Info Not Found! Should be select Pharmacy Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Account info saving Error.." });
            }

        }
        //#endregion
        #region DocOrgTaxonomy

        // GET: Taxonomy 
        public ActionResult Taxonomy(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetPharmacyTaxonomyList/{flag}/{id}")]
        public ActionResult GetPharmacyTaxonomyList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var Info = _repo.ExecWithStoreProcedure<OrganisationTaxonomyViewModel>("spDocOrgTaxonomy_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = Info
                .Select(x => new OrganisationTaxonomyListViewModel()
                {
                    DocOrgTaxonomyID = x.DocOrgTaxonomyID,
                    TaxonomyID = x.TaxonomyID,
                    ReferenceID = x.ReferenceID,
                    OrganisationId = x.ReferenceID,
                    Taxonomy_Code = x.Taxonomy_Code,
                    Specialization = x.Specialization,
                    OrganisationName = x.OrganisationName,
                    OrganizationTypeID = 1005,
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


        //---- Get Taxonomy Details

        public PartialViewResult PharmacyTaxonomy(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgTaxonomyInfo = _repo.ExecWithStoreProcedure<OrganisationTaxonomyViewModel>("spDocOrgTaxonomy_GetById @DocOrgTaxonomyID",
                    new SqlParameter("DocOrgTaxonomyID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrganisationTaxonomyUpdateViewModel
                    {
                        DocOrgTaxonomyID = x.DocOrgTaxonomyID,
                        TaxonomyID = x.TaxonomyID,
                        OrganisationId = x.ReferenceID,
                        Taxonomy_Code = x.Taxonomy_Code,
                        Specialization = x.Specialization,
                        OrganisationName = x.OrganisationName,
                        OrganizationTypeID = x.OrganizationTypeID,
                        UserTypeID = (int)x.UserTypeId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted
                    }).First();


                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyTaxonomy.cshtml", orgTaxonomyInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgTaxonomyInfo = new OrganisationTaxonomyUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyTaxonomy.cshtml", orgTaxonomyInfo);
            }
        }


        //-- Add/Edit Taxonomy

        [HttpPost, Route("AddEditPharmacyTaxonomy"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyTaxonomy(OrganisationTaxonomyUpdateViewModel model)
        {
            try
            {
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
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                      new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 3 }
                                  );

                return Json(new JsonResponse() { Status = 1, Message = "Pharmacy Taxonomy info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy Taxonomy info saving Error.." });
            }

        }


        //-- Active DeActive Pharmacy Taxonomy
        [Route("ActiveDeActivePharmacyTaxonomy/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActivePharmacyTaxonomy(bool flag, int id)
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

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Taxonomy info has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Taxonomy info {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }

        #endregion

        #region Speciality

        // GET: Speciality 
        public ActionResult Speciality(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetPharmacySpecialityList/{flag}/{id}")]
        public ActionResult GetPharmacySpecialityList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var Info = _repo.ExecWithStoreProcedure<OrganisationSpecialityViewModel>("spOrganizationSpeciality_Get @Search, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = Info
                .Select(x => new OrganisationSpecialityListViewModel()
                {
                    TaxonomyID = x.TaxonomyID,
                    ParentID = x.ParentID,
                    Taxonomy_Code = x.Taxonomy_Code,
                    Specialization = x.Specialization,
                    Description = x.Description,
                    Taxonomy_Type = x.Taxonomy_Type,
                    Taxonomy_Level = x.Taxonomy_Level,
                    SpecialtyText = x.SpecialtyText,
                    IsSpecialty = x.IsSpecialty,
                    IsActive = x.IsActive,
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
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

        //---- Get Speciality Details

        public PartialViewResult PharmacySpeciality(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.PharmacyID = Session["PharmacyID"];

            }

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrganisationSpecialityViewModel>("spOrganizationSpeciality_GetById @TaxonomyID",
                    new SqlParameter("TaxonomyID", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrganisationSpecialityUpdateViewModel
                    {
                        TaxonomyID = x.TaxonomyID,
                        ParentID = x.ParentID,
                        Taxonomy_Code = x.Taxonomy_Code,
                        Specialization = x.Specialization,
                        ParentText = x.ParentID > 0 ? GetTaxonomyCodeById(x.ParentID.Value, "special") : null,
                        Taxonomy_Level = x.Taxonomy_Level,
                        Taxonomy_Type = x.Taxonomy_Type,
                        SpecialtyText = x.SpecialtyText,
                        IsActive = x.IsActive,
                        IsSpecialty = x.IsSpecialty != null ? x.IsSpecialty.Value : false
                    }).First();

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacySpeciality.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrganisationSpecialityUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacySpeciality.cshtml", orgInfo);
            }
        }

        //--Add/Update Speciality

        [HttpPost, Route("AddEditPharmacySpeciality"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacySpeciality(OrganisationSpecialityUpdateViewModel model)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizationSpeciality_Create " +
                        "@TaxonomyID," +
                        "@Taxonomy_Code," +
                        "@Specialization," +
                        "@Description," +
                        "@ParentID," +
                        "@IsActive," +
                        "@CreatedBy," +
                        "@Taxonomy_Type," +
                        "@Taxonomy_Level," +
                        "@SpecialtyText," +
                        "@IsSpecialty",
                                      new SqlParameter("TaxonomyID", System.Data.SqlDbType.Int) { Value = model.TaxonomyID > 0 ? model.TaxonomyID : 0 },
                                      new SqlParameter("Taxonomy_Code", System.Data.SqlDbType.VarChar) { Value = (object)model.Taxonomy_Code ?? DBNull.Value },
                                      new SqlParameter("Specialization", System.Data.SqlDbType.VarChar) { Value = (object)model.Specialization ?? DBNull.Value },
                                      new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (object)model.Description ?? DBNull.Value },
                                      new SqlParameter("ParentID", System.Data.SqlDbType.Int) { Value = (object)model.ParentID ?? DBNull.Value },
                                      new SqlParameter("IsActive", System.Data.SqlDbType.VarChar) { Value = model.IsActive },
                                      new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                      new SqlParameter("Taxonomy_Type", System.Data.SqlDbType.VarChar) { Value = (object)model.Taxonomy_Type ?? DBNull.Value },
                                      new SqlParameter("Taxonomy_Level", System.Data.SqlDbType.VarChar) { Value = (object)model.Taxonomy_Level ?? DBNull.Value },
                                      new SqlParameter("SpecialtyText", System.Data.SqlDbType.VarChar) { Value = (object)model.SpecialtyText ?? DBNull.Value },
                                      new SqlParameter("IsSpecialty", System.Data.SqlDbType.Bit) { Value = (object)model.IsSpecialty ?? DBNull.Value }
                                  );

                return Json(new JsonResponse() { Status = 1, Message = "Pharmacy Speciality info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy Speciality info saving Error.." });
            }

        }

        //-- Delete Pharmacy Speciality
        [Route("DeletePharmacySpeciality/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacySpeciality(int id)
        {
            try
            {

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizationSpeciality_Delete @TaxonomyID, @ModifiedBy",
                    new SqlParameter("TaxonomyID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Speciality info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Speciality info deleted Error.." });
            }
        }

        #endregion

        #region Procedure

        // GET: Procedure
        public ActionResult Procedure(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetPharmacyProcedureList/{flag}/{id}")]
        public ActionResult GetPharmacyProcedureList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var orgInfo = _repo.ExecWithStoreProcedure<OrgProcedureViewModel>("spOrgProcedure_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = orgInfo
                .Select(x => new OrgProcedureListViewModel()
                {
                    OrgProcedureId = x.OrgProcedureId,
                    OrganisationId = x.ReferenceID,
                    OrganisationName = x.ReferenceName,
                    OrganizatonTypeID = x.OrganizatonTypeID,
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Name = x.Name,
                    Description = x.Description,
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


        //---- Get Procedure Details

        public PartialViewResult PharmacyProcedure(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgProcedureViewModel>("spOrgProcedure_GetById @OrgProcedureId",
                    new SqlParameter("OrgProcedureId", System.Data.SqlDbType.Int) { Value = id }
                    ).Select(x => new OrgProcedureUpdateViewModel
                    {
                        OrgProcedureId = x.OrgProcedureId,
                        OrganisationId = x.ReferenceID,
                        OrganizatonTypeID = 1005,
                        OrganisationName = x.ReferenceName,
                        UserTypeID = 3,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        Name = x.Name,
                        Description = x.Description
                    }).First();


                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyProcedure.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgProcedureUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyProcedure.cshtml", orgInfo);
            }
        }


        //---- Add Edit Pharmacy Procedure

        [HttpPost, Route("AddEditPharmacyProcedure"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyProcedure(OrgProcedureUpdateViewModel model)
        {

            try
            {
                if (model.OrganisationId > 0)
                {

                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrgProcedure_Create " +
                            "@OrgProcedureId," +
                            "@ReferenceID," +
                            "@UserTypeID," +
                            "@Name," +
                            "@Description," +
                            "@IsActive," +
                            "@CreatedBy",
                                          new SqlParameter("OrgProcedureId", System.Data.SqlDbType.Int) { Value = model.OrgProcedureId > 0 ? model.OrgProcedureId : 0 },
                                          new SqlParameter("ReferenceID", System.Data.SqlDbType.Int) { Value = model.OrganisationId },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                                          new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = (object)model.Name ?? DBNull.Value },
                                          new SqlParameter("Description", System.Data.SqlDbType.NVarChar) { Value = (object)model.Description ?? DBNull.Value },
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy Procedure info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Pharmacy Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy Procedure info saving Error.." });
            }

        }

        //-- Delete Pharmacy Procedure
        [Route("DeletePharmacyProcedure/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyProcedure(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrgProcedure_Delete @OrgProcedureId, @ModifiedBy",
                    new SqlParameter("OrgProcedureId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Procedure info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Procedure info deleted Error.." });
            }
        }

        #endregion


        #region Booking

        // GET: Booking
        public ActionResult Booking(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }
        [HttpPost]
        public JsonResult GetOrgAddressById(int ReferenceId, int TypeId)
        {
            var planList = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModel>("sprGetDocOrganiationAddresses @OrganizationId, @UserTypeId",
                new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ReferenceId },
                new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = TypeId }

                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [Route("GetPharmacyBookingList/{flag}/{id}")]
        public ActionResult GetPharmacyBookingList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spDocOrgBooking_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = orgInfo
                .Select(x => new OrgBookingListViewModel()
                {
                    SlotId = x.SlotId,
                    DoctorId = x.DoctorId,
                    OrganisationName = x.OrganisationName,
                    OrganisationId = x.OrganisationId.HasValue ? x.OrganisationId.Value : 0,
                    DoctorName = x.DoctorName + " [" + x.Credential + "]",
                    OrganizatonTypeID = x.OrganizatonTypeID.HasValue ? x.OrganizatonTypeID.Value: 0,
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


        //---- Get Booking Details

        public PartialViewResult PharmacyBooking(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];


            var addressList = new List<OrgAddressDropdownViewModel>();
            /*_repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
                new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.PharmacyID },
                new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }
                ).Select(x=>new OrgAddressDropdownViewModel() {
                    AddressId = x.AddressId,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country
                });
                */
            ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress");

            var doctorsList = _repo.ExecWithStoreProcedure<OrgDoctorsDropDownViewModel>("spDocOrgBooking_GetOrgDoc @OrganizationId",
               new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ViewBag.PharmacyID }
               ).Select(x => new OrgDoctorsDropDownViewModel()
               {
                   DoctorId = x.DoctorId,
                   DisplayName = x.DoctorName + " [" + x.Credential + "]"
               });

            ViewBag.doctorsList = new SelectList(doctorsList.OrderBy(o => o.DoctorName), "DoctorId", "DisplayName");

            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get");
            ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name");

            //  var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get");
            var planList = new List<InsurancePlanDropDownViewModel>();
            ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spDocOrgBooking_GetById @SlotId, @OrganizationId",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ViewBag.PharmacyID }
                    ).Select(x => new OrgBookingUpdateViewModel
                    {
                        SlotId = x.SlotId,
                        OrganisationId = x.OrganisationId.HasValue ? x.OrganisationId.Value : 0,
                        DoctorId = x.DoctorId,
                        AddressId = x.AddressId,
                        BookedFor = x.BookedFor,
                        OrganizatonTypeID = 1005,
                        OrganisationName = x.OrganisationName,
                        FullName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        UserTypeID = 3,
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
                        FullAddress = x.FullAddress
                    }).First();

                var addressesList = _repo.ExecWithStoreProcedure<OrganizationAddressDropDownViewModel>("sprGetDocOrganiationAddresses @OrganizationId, @UserTypeId",
          new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = orgInfo.DoctorId },
          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 3 }
          ).ToList();

                ViewBag.addressList = new SelectList(addressesList.OrderBy(o => o.AddressId), "AddressId", "OrganizationAddress", orgInfo.AddressId);
                ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name", orgInfo.InsurancePlanId);
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyBooking.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgBookingUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyBooking.cshtml", orgInfo);
            }
        }


        //--- Add Update Pharmacy Booking Info

        [HttpPost, Route("AddEditPharmacyBooking"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyBooking(OrgBookingUpdateViewModel model)
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
                                          new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                          new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = model.InsurancePlanId },
                                          new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId },
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = model.Description },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }

                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy booking info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Pharmacy Info Not Found! Should be select Pharmacy Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy booking info saving Error.." });
            }

        }

        //-- Delete Pharmacy Booking
        [Route("DeletePharmacyBooking/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy Booking info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Booking info deleted Error.." });
            }
        }

        #endregion


        #region Patient Order

        // GET: Patient Order
        public ActionResult PatientOrder(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetPharmacyPatientOrderList/{flag}/{id}")]
        public ActionResult GetPharmacyPatientOrderList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var orgInfo = _repo.ExecWithStoreProcedure<OrgPatientOrderViewModel>("spPatientOrder_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1005 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
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
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    NetPrice = x.NetPrice,
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


        //---- Get Order Details

        public PartialViewResult PharmacyOrder(int? id)
        {
            ViewBag.PharmacyID = Session["PharmacyID"];


            var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
                new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.PharmacyID },
                new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 3 }
                ).Select(x => new OrgAddressDropdownViewModel()
                {
                    AddressId = x.AddressId,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country
                });

            ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress");

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
                        FullName = getPatinetNameById(x.PatientId).Count() > 0 ? (getPatinetNameById(x.PatientId).First().FirstName + " " + getPatinetNameById(x.PatientId).First().LastName) : "",
                        PhoneNumber = getPatinetNameById(x.PatientId).Count() > 0 ? getPatinetNameById(x.PatientId).First().PhoneNumber : "",
                        Email = getPatinetNameById(x.PatientId).Count() > 0 ? getPatinetNameById(x.PatientId).First().Email : "",
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

                ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress", orgInfo.AddressId);
                ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name", orgInfo.InsurancePlanId);


                if (ItemList((long)id).Count() > 0)
                {
                    ViewBag.orderItems = ItemList((long)id);
                }
                else
                {
                    ViewBag.orderItems = null;
                }

                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyOrder.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgPatientOrderUpdateViewModel();
                return PartialView(@"/Views/Pharmacy/Partial/_PharmacyOrder.cshtml", orgInfo);
            }
        }

        public List<OrgPatientInfoViewModel> getPatinetNameById(int id)
        {

            var patientNameInfo = _repo.ExecWithStoreProcedure<OrgPatientInfoViewModel>("spPatientName_GetByID @PatientId",
                   new SqlParameter("PatientId", System.Data.SqlDbType.BigInt) { Value = id }
                   );

            return patientNameInfo.ToList();
        }

        public List<OrgPatientOrderDetailsViewModel> ItemList(long id)
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

        public int GetInsuranceTypeId(int id)
        {
            int InsuranceTypeId = 0;

            var planInfo = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceInfo_GetByPlanID @InsurancePlanId",
                   new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = id }
                   );

            InsuranceTypeId = planInfo.First().InsuranceTypeId;

            return InsuranceTypeId;
        }

        //--- Add Update Pharmacy Order Info

        [HttpPost, Route("AddEditPharmacyOrder"), ValidateAntiForgeryToken]
        public JsonResult AddEditPharmacyOrder(OrgPatientOrderUpdateViewModel model, HttpPostedFileBase Image1)
        {
            try
            {
                if (model.OrganisationId > 0)
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
                                                  new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                                  new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)model.IsActive ?? DBNull.Value }
                                              ).First();

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

                    return Json(new JsonResponse() { Status = 1, Message = "Pharmacy booking info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! Pharmacy Info Not Found! Should be select Pharmacy Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "Pharmacy booking info saving Error.." });
            }

        }

        //-- Delete Pharmacy Order
        [Route("DeletePharmacyOrder/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyOrder(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spPatientOrder_Delete @OrderId, @ModifiedBy",
                    new SqlParameter("OrderId", System.Data.SqlDbType.BigInt) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy order info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Order info deleted Error.." });
            }
        }

        //-- Delete Pharmacy Order
        [Route("DeletePharmacyOrderItem/{id}")]
        [HttpPost]
        public JsonResult DeletePharmacyOrderItem(int id)
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
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The Pharmacy order item has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"Pharmacy Order item deleted Error.." });
            }
        }

        #endregion

        #endregion


        #region Cost
        public ActionResult Cost(int id)
        {
            ViewBag.costId = id;
            Session["ReferenceID"] = id;
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("Pharmacy"))
                {
                    int userId = User.Identity.GetUserId<int>();
                    var userInfo = _appUser.GetById(userId);
                    string NPI = userInfo.Uniquekey;
                    var pharmacyInfo = _repo.Find<Organisation>(x => x.OrganizationTypeID == 1005 && x.IsDeleted == false && x.UserId == userId);
                    TempData["PharmacyData"] = "Yes";
                    ViewBag.PharmacyID = pharmacyInfo.OrganisationId;
                    Session["PharmacyID"] = pharmacyInfo.OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("Pharmacy/GetSeniorCareCostList/{id?}")]
        public ActionResult GetSeniorCareCostList(JQueryDataTableParamModel param, int id = 0)
        {
            id = Convert.ToInt32(Session["ReferenceID"]);
            var allCosts = _repo.ExecWithStoreProcedure<CostViewModel>("spCost_GetList @Search, @UserTypeID, @OrganizationID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.Pharmacy },
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
                    UserTypeID = x.UserTypeID
                }).Where(x => x.UserTypeID == (int)UserTypes.Pharmacy).ToList();

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
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.Pharmacy },
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

        [Route("Pharmacy/DeleteSeniorCareCost/{id}")]
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
