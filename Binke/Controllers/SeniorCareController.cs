using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using Doctyme.Model;
using Binke.Models;
using Doctyme.Repository.Interface;
using Binke.ViewModels;
using Binke.Utility;
using Microsoft.AspNet.Identity;
using System.Data;
using Doctyme.Repository.Enumerable;
using System.Data.Entity;
using System.Web;
using System.IO;
using Binke.App_Helpers;
using Binke;
using Doctyme.Repository.Services;
using System.Data.SqlClient;
using Doctyme.Model.ViewModels;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Binke.Controllers
{
    [Authorize]
    public class SeniorCareController : Controller
    {
        private readonly IAddressService _address;
        private readonly ICityService _city;
        private readonly ISeniorCareService _seniorCare;
        //private readonly ISeniorCareImageService _seniorCareImage;
        private readonly IExperienceService _experience;
        private readonly IOpeningHourService _openingHour;
        private readonly IStateService _state;
        private readonly ISocialMediaService _socialMedia;
        private readonly IFacilityService _facility;
        private readonly IUserService _appUser;
        private readonly ILanguageService _languageService;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository _repo;

        private int _careId;
        public SeniorCareController(IAddressService address, ICityService city, ISeniorCareService seniorCare, IFacilityService facility, /*ISeniorCareImageService seniorCareImage, */ILanguageService languageService, IExperienceService experience, IOpeningHourService openingHour, IStateService state, ISocialMediaService socialMedia, IUserService appUser, ApplicationUserManager applicationUserManager, RepositoryService repo)
        {
            _address = address;
            _city = city;
            //_seniorCareImage = seniorCareImage;
            _experience = experience;
            _openingHour = openingHour;
            _state = state;
            _socialMedia = socialMedia;
            _appUser = appUser;
            _languageService = languageService;
            _seniorCare = seniorCare;
            _facility = facility;
            _userManager = applicationUserManager;
            _repo = repo;
        }

        //#region Admin Section
        public ActionResult Index()
        {
            return View();
        }

        [Route("GetSeniorCareList/{flag}")]
        public ActionResult GetSeniorCareList(bool flag, JQueryDataTableParamModel param)
        {

            var allPharmacys = _repo.ExecWithStoreProcedure<OrganisationProfileViewModel>("spOrganisation_GetByAddressTypeID @Search, @UserTypeID, @OrganizationTypeID, @AddressTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.SeniorCare },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = (int)OrganisationTypes.SeniorCare },
                    new SqlParameter("AddressTypeID", System.Data.SqlDbType.Int) { Value = 0 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allPharmacys
                .Select(x => new SeniorCareData()
                {
                    OrganisationId = x.OrganisationId,
                    OrganisationName = x.OrganisationName,
                    FullName = x.AuthorizedOfficialFirstName + " " + x.AuthorizedOfficialLastName,
                    NPI = x.NPI,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate.ToDefaultFormate(),
                    IsActive = x.IsActive,
                    EnabledBooking = x.EnabledBooking,
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


            //var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            //var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            //var pageIndex = param.iDisplayStart / param.iDisplayLength;
            //var model = new SearchParamModel();
            //model.PageIndex = pageIndex;
            //model.SearchBox = param.sSearch;
            //model.PageSize = param.iDisplayLength;
            //var searchResult = GetSeniorCareSearchResult(model, OrganisationTypes.SeniorCare);
            //var allSeniorCare = searchResult.SeniorCareDataList.AsEnumerable();
            //Func<SeniorCareData, string> orderingFunction =
            //    c => sortColumnIndex == 1 ? c.OrganisationName
            //                //: sortColumnIndex == 3 ? c.Email
            //                //    : sortColumnIndex == 4 ? c.
            //                : c.OrganisationName;
            //allSeniorCare = sortDirection == "asc" ? allSeniorCare.OrderBy(orderingFunction).ToList() : allSeniorCare.OrderByDescending(orderingFunction).ToList();

            ////var display = allSeniorCare.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            //var total = searchResult.TotalRecord;

            //return Json(new
            //{
            //    param.sEcho,
            //    iTotalRecords = total,
            //    iTotalDisplayRecords = total,
            //    aaData = allSeniorCare
            //}, JsonRequestBehavior.AllowGet);
        }

        //[Route("GetSeniorCareListOld/{flag}")]
        public ActionResult GetSeniorCareListOld(bool flag, JQueryDataTableParamModel param)
        {
            var allSeniorCares = _seniorCare.GetAll(x => x.OrganizationTypeID == (int)OrganisationTypes.SeniorCare).Select(x => new SeniorCareBasicInformation()
            {
                Id = x.OrganisationId,
                FullName = x.AuthorizedOfficialFirstName + x.AuthorizedOfficialLastName,
                Email = "",
                Gender = "",
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
                IsActive = x.IsActive,
                UserId = x.UserId
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allSeniorCares = allSeniorCares.Where(x => x.FullName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.Email.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<SeniorCareBasicInformation, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.FullName
                    : sortColumnIndex == 2 ? c.Email
                    : sortColumnIndex == 3 ? c.Gender
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                            : c.FullName;
            allSeniorCares = sortDirection == "asc" ? allSeniorCares.OrderBy(orderingFunction).ToList() : allSeniorCares.OrderByDescending(orderingFunction).ToList();

            var display = allSeniorCares.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allSeniorCares.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        //[Route("GetSeniorCareListTemp/{flag}")]
        public ActionResult GetSeniorCareListTemp(bool flag, JQueryDataTableParamModel param)
        {
            var allSeniorCares = _seniorCare.GetAll(x => x.OrganizationTypeID == 1007).Select(x => new SeniorCare()
            {
                SeniorCareId = x.OrganisationId,
                SeniorCareName = x.AuthorizedOfficialFirstName + x.AuthorizedOfficialLastName,
                Summary = "",
                Description = "",
                Amenities = "",
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                IsActive = x.IsActive
            }).ToList();

            //if (!string.IsNullOrEmpty(param.sSearch))
            //{
            //    allSeniorCares = allSeniorCares.Where(x => x.FullName.ToString().ToLower().Contains(param.sSearch.ToLower())
            //                                    || x.Email.ToString().ToLower().Contains(param.sSearch.ToLower())
            //                                    || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
            //                                    || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            //}

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<SeniorCare, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.SeniorCareName
                    : sortColumnIndex == 2 ? c.Summary
                    : sortColumnIndex == 3 ? c.Description
                     : c.SeniorCareName;


            allSeniorCares = sortDirection == "asc" ? allSeniorCares.OrderBy(orderingFunction).ToList() : allSeniorCares.OrderByDescending(orderingFunction).ToList();

            var display = allSeniorCares.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allSeniorCares.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }
        [Route("SeniorCareProfile/{id?}/{flag?}/{isSeniorCareUser?}")]
        public ActionResult SeniorCareProfile(int id = 0, int flag = 0, bool isSeniorCareUser = true)
        {
            ViewBag.flag = flag;
            int userId = 0;
            int orgId = 0;
            string orgname = "";
            string orgnpi = "";
            //if (isSeniorCareUser)
            //{
            //    userId = User.Identity.GetUserId<int>();
            //    var _data = _repo.Find<Organisation>(w => w.UserId == userId);
            //    if (_data != null)
            //    {
            //        orgId = _data.OrganisationId;
            //        orgname = _data.OrganisationName;
            //        orgnpi = _data.NPI;
            //        id = _data.OrganisationId;
            //        ViewBag.SeniorcareId = _data.EnumerationDate == null ? 0 : orgId;
            //    }
            //}

            //ViewBag.UserId = userId;
            //ViewBag.orgId = orgId;
            //ViewBag.orgname = orgname;
            //ViewBag.orgnpi = orgnpi;
            //ViewBag.isSeniorCareUser = isSeniorCareUser;

            //var seniorCare = _seniorCare.GetSingle(x => x.OrganisationId == id);
            //return View(seniorCare);
            if (id > 0)
            {
                ViewBag.ID = id;
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
                    }).First();

                ViewBag.UserId = userId;
                ViewBag.orgId = orgInfo.OrganisationId;
                ViewBag.orgname = orgInfo.OrganisationName;
                ViewBag.orgnpi = orgInfo.NPI;
                ViewBag.isSeniorCareUser = isSeniorCareUser;

                //var orgInfo = _repo.ExecWithStoreProcedure<Organisation>("spGetOrganizationInfoByID @orgnizationID",
                //    new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = id }
                //    );
                //return View(@"/Views/SeniorCare/Partial/_SeniorCareCreateProfile.cshtml", orgInfo);
                return View(orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrganisationProfileViewModel();
                //return View(orgInfo);
                orgInfo.OrganizationTypeID = (int)OrganisationTypes.SeniorCare;
                orgInfo.UserTypeID = (int)UserTypes.SeniorCare;
                //return View(@"/Views/SeniorCare/Partial/_SeniorCareCreateProfile.cshtml", orgInfo);
                return View(orgInfo);
            }
        }

        [HttpPost, Route("SaveSeniorCareProfile"), ValidateAntiForgeryToken]
        public JsonResult SaveSeniorCareProfile(Organisation model)
        {
            try
            {
                #region
                //var parameters = new List<SqlParameter>()
                //{
                //    new SqlParameter("@OrganisationId", System.Data.SqlDbType.Int) { Value = model.OrganisationId > 0 },
                //         new SqlParameter("@OrganisationName", System.Data.SqlDbType.VarChar) { Value = (object)model.OrganisationName ?? DBNull.Value },
                //         new SqlParameter("@AuthorizedOfficialFirstName", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialFirstName ?? DBNull.Value },
                //         new SqlParameter("@AuthorizedOfficialLastName", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialLastName ?? DBNull.Value },
                //         new SqlParameter("@AuthorizedOfficialTitleOrPosition", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialTitleOrPosition ?? DBNull.Value },
                //         new SqlParameter("@AuthorizedOfficialTelephoneNumber", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialTelephoneNumber ?? DBNull.Value },
                //         new SqlParameter("@ShortDescription", System.Data.SqlDbType.VarChar) { Value = (object)model.ShortDescription ?? DBNull.Value },
                //         new SqlParameter("@LongDescription", System.Data.SqlDbType.VarChar) { Value = (object)model.LongDescription ?? DBNull.Value }
                //};
                #endregion

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

                _seniorCare.UpdateSeniorCareProfile(StoredProcedureList.GetSeniorCareProfileUpdate, model);
                return Json(new JsonResponse { Status = 1, Message = "Organisation updated successfully" });
            }
            catch (Exception ex)
            {
                //txscope.Dispose();
                Common.LogError(ex, "SaveSeniorCareProfile-Post");
                return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
            }
        }

        [HttpPost, Route("ActiveDeActiveSeniorCare/{flag}/{id}")]
        public JsonResult ActiveDeActiveSeniorCare(bool flag, int id)
        {
            var seniorCare = _seniorCare.GetById(id);
            seniorCare.IsActive = !flag;
            seniorCare.IsActive = !flag;
            _seniorCare.UpdateData(seniorCare);
            _seniorCare.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }


        //#endregion

        //#region MyProfile
        //[HttpGet, Route("SeniorCareBasicInformation/{id?}")]
        //public ActionResult BasicInformation(int id = 0)
        //{
        //    ViewBag.StateList = _state.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
        //    {
        //        Text = x.StateName,
        //        Value = x.StateId.ToString()
        //    }).ToList();
        //    if (User.IsInRole(UserRoles.Admin))
        //        _careId = id;
        //    else
        //        _careId = GetSeniorCareId();
        //    var seniorCare = _seniorCare.GetById(_careId);
        //    var result = new SeniorCareBasicInformation();
        //    result.SeniorCareId = seniorCare.SeniorCareId;
        //    result.Id = seniorCare.UserId;
        //    result.Prefix = seniorCare.SeniorCareUser?.Prefix ?? "";
        //    result.FirstName = seniorCare.SeniorCareUser?.FirstName ?? "";
        //    result.MiddleName = seniorCare.SeniorCareUser?.MiddleName ?? "";
        //    result.LastName = seniorCare.SeniorCareUser?.LastName ?? "";
        //    result.Suffix = seniorCare.SeniorCareUser?.Suffix ?? "";
        //    result.Gender = seniorCare.SeniorCareUser?.Gender ?? "";
        //    result.PhoneNumber = seniorCare.SeniorCareUser?.PhoneNumber;
        //    result.FaxNumber = seniorCare.SeniorCareUser?.FaxNumber;
        //    result.ProfilePicture = seniorCare.SeniorCareUser?.ProfilePicture ?? "";
        //    result.SeniorCareName = seniorCare.SeniorCareName ?? "";
        //    result.Summary = seniorCare.Summary ?? "";
        //    result.Description = seniorCare.Description ?? "";
        //    result.Amenities = seniorCare.Amenities ?? "";
        //    result.AddressView = AutoMapper.Mapper.Map<AddressViewModel>(seniorCare.Address.FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.IsDefault));
        //    result.SocialMediaViewModel = AutoMapper.Mapper.Map<SocialMediaViewModel>(seniorCare.SocialMediaLinks.FirstOrDefault());

        //    return View(result);
        //}

        //[HttpPost, Route("SeniorCareBasicInformation")]
        //public JsonResult BasicInformation(SeniorCareBasicInformation model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var seniorCare = _seniorCare.GetById(model.SeniorCareId);

        //            #region SeniorCare Basic
        //            seniorCare.SeniorCareUser.Prefix = model.Prefix;
        //            seniorCare.SeniorCareUser.FirstName = model.FirstName ?? "";
        //            seniorCare.SeniorCareUser.MiddleName = model.MiddleName ?? "";
        //            seniorCare.SeniorCareUser.LastName = model.LastName ?? "";
        //            seniorCare.SeniorCareUser.DateOfBirth = model.DateOfBirth;
        //            seniorCare.SeniorCareUser.Suffix = model.Suffix ?? "";
        //            seniorCare.SeniorCareUser.Gender = model.Gender ?? "";
        //            seniorCare.SeniorCareUser.PhoneNumber = model.PhoneNumber;
        //            seniorCare.SeniorCareUser.FaxNumber = model.FaxNumber;
        //            seniorCare.Summary = model.Summary;
        //            seniorCare.Amenities = model.Amenities;
        //            seniorCare.Description = model.Description;
        //            seniorCare.SeniorCareName = model.SeniorCareName;
        //            _seniorCare.UpdateData(seniorCare);
        //            _seniorCare.SaveData();
        //            #endregion

        //            #region SeniorCare Address

        //            var address = AutoMapper.Mapper.Map<AddressViewModel, Address>(model.AddressView);
        //            address.AddressId = model.AddressView.AddressId;
        //            if (address.AddressId == 0)
        //            {
        //                address.SeniorCareId = model.SeniorCareId;
        //                address.IsDefault = true;
        //                address.IsActive = true;
        //                _address.InsertData(address);
        //                _address.SaveData();
        //            }
        //            else
        //            {
        //                var editAddress = _address.GetById(address.AddressId);
        //                editAddress.Address1 = address.Address1;
        //                editAddress.Address2 = address.Address2;
        //                editAddress.CityId = address.CityId;
        //                editAddress.StateId = address.StateId;
        //                editAddress.Country = address.Country;
        //                editAddress.ZipCode = address.ZipCode;
        //                _address.UpdateData(editAddress);
        //                _address.SaveData();
        //            }
        //            #endregion

        //            #region SeniorCare Social Media
        //            var socialMedia = AutoMapper.Mapper.Map<SocialMediaViewModel, SocialMedia>(model.SocialMediaViewModel);
        //            socialMedia.SocialMediaId = model.SocialMediaViewModel.SocialMediaId;
        //            if (model.SocialMediaViewModel.SocialMediaId == 0)
        //            {
        //                socialMedia.SeniorCareId = model.SeniorCareId;
        //                _socialMedia.InsertData(socialMedia);
        //                _socialMedia.SaveData();
        //            }
        //            else
        //            {
        //                var editSocialMedia = _socialMedia.GetById(socialMedia.SocialMediaId);
        //                if (editSocialMedia != null)
        //                {
        //                    editSocialMedia.Facebook = socialMedia.Facebook;
        //                    editSocialMedia.Twitter = socialMedia.Twitter;
        //                    editSocialMedia.LinkedIn = socialMedia.LinkedIn;
        //                    editSocialMedia.Instagram = socialMedia.Instagram;
        //                    _socialMedia.UpdateData(editSocialMedia);
        //                    _socialMedia.SaveData();
        //                }
        //            }
        //            #endregion

        //            txscope.Complete();
        //            return Json(new JsonResponse { Status = 1, Message = "SeniorCare basic information updated." }, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            Common.LogError(ex, "SeniorCareBasicInformation-Post");
        //            return Json(new JsonResponse { Status = 0, Message = "Something is wrong." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        //[HttpPost]
        //public JsonResult UploadProfile(HttpPostedFileBase profilePic)
        //{
        //    using (var txscopr = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        string filePath = string.Empty;
        //        try
        //        {
        //            if (profilePic != null)
        //            {
        //                _careId = GetSeniorCareId();
        //                var seniorCare = _seniorCare.GetById(_seniorCare);
        //                var oldPic = seniorCare.SeniorCareUser.ProfilePicture;
        //                string extension = Path.GetExtension(profilePic.FileName);
        //                string profilePicture = $@"SeniorCare-{DateTime.Now.Ticks}{extension}";
        //                filePath = Path.Combine(Server.MapPath(FilePathList.ProfilePic), profilePicture);
        //                Common.CheckServerPath(Server.MapPath(FilePathList.ProfilePic));
        //                profilePic.SaveAs(filePath);
        //                seniorCare.SeniorCareUser.ProfilePicture = $@"{FilePathList.ProfilePic}{profilePicture}";
        //                User.Identity.AddUpdateClaim(UserClaims.ProfilePicture, seniorCare.SeniorCareUser.ProfilePicture);

        //                _seniorCare.UpdateData(seniorCare);
        //                _seniorCare.SaveData();
        //                txscopr.Complete();
        //                if (oldPic != StaticFilePath.ProfilePicture)
        //                    Common.DeleteFile(oldPic);
        //                return Json(new JsonResponse { Status = 1, Message = "Profile Picture is updated.", Data = seniorCare.SeniorCareUser.ProfilePicture }, JsonRequestBehavior.AllowGet);
        //            }
        //            txscopr.Dispose();
        //            return Json(new JsonResponse { Status = 0, Message = "Profile Picture is not updated." }, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            if (!string.IsNullOrEmpty(filePath))
        //                Common.DeleteFile(filePath, true);
        //            txscopr.Dispose();
        //            Common.LogError(ex, "UploadProfile-Post");
        //            return Json(new JsonResponse { Status = 0, Message = "Profile Picture is not updated." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        //#endregion

        //#region My Images
        //[HttpGet, Route("MyImages")]
        //public ActionResult MyImages()
        //{
        //    return View();
        //}

        //public ActionResult GetMyImagesList(JQueryDataTableParamModel param)
        //{
        //    _careId = GetSeniorCareId();
        //    var allImages = _seniorCareImage.GetAll(x => x.IsActive && !x.IsDeleted && x.SeniorCareId == _careId).Select(x => new SeniorCareImageViewModel()
        //    {
        //        Id = x.SeniorCareImageId,
        //        ImagePath = FilePathList.SeniorCare + x.ImagePath,
        //        CreatedDate = x.CreatedDate.ToDefaultFormate(),
        //        UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate()
        //    }).ToList();

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        allImages = allImages.Where(x => x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                      || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
        //    }
        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

        //    Func<SeniorCareImageViewModel, string> orderingFunction =
        //        c => sortColumnIndex == 2 ? c.CreatedDate
        //            : sortColumnIndex == 3 ? c.UpdatedDate
        //                    : c.CreatedDate;
        //    allImages = sortDirection == "asc" ? allImages.OrderBy(orderingFunction).ToList() : allImages.OrderByDescending(orderingFunction).ToList();


        //    var display = allImages.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var total = allImages.Count;

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet, Route("_MyImages/{id?}")]
        //public PartialViewResult MyImages(int id)
        //{
        //    _careId = GetSeniorCareId();
        //    if (id == 0) return PartialView(@"Partial/_MyImages", new SeniorCareImageViewModel() { SeniorCareId = _careId });
        //    var seniorCareImage = _seniorCareImage.GetById(id);
        //    var result = AutoMapper.Mapper.Map<SeniorCareImageViewModel>(seniorCareImage);
        //    if (result == null) return PartialView(@"Partial/_MyImages", new SeniorCareImageViewModel());
        //    return PartialView(@"Partial/_MyImages", result);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public JsonResult MyImages(SeniorCareImageViewModel model)
        //{
        //    var filePath = new List<string>();
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var seniorCareImages = new List<SeniorCareImage>();
        //            foreach (var item in model.Image)
        //            {
        //                string extension = Path.GetExtension(item.FileName);
        //                model.ImagePath = $@"SeniorCare-{DateTime.Now.Ticks}{extension}";
        //                string singleFile = Path.Combine(Server.MapPath(FilePathList.SeniorCare), model.ImagePath);
        //                filePath.Add(singleFile);
        //                Common.CheckServerPath(Server.MapPath(FilePathList.SeniorCare));

        //                item.SaveAs(singleFile);
        //                seniorCareImages.Add(new SeniorCareImage()
        //                {
        //                    SeniorCareId = model.SeniorCareId,
        //                    ImagePath = model.ImagePath,
        //                    IsActive = true
        //                });
        //            }
        //            _seniorCareImage.InsertData(seniorCareImages);
        //            _seniorCareImage.SaveData();
        //            txscope.Complete();
        //            return Json(new JsonResponse() { Status = 1, Message = "Image upload successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            if (filePath.Any())
        //            {
        //                filePath.ForEach(x => Common.DeleteFile(x, true));
        //            }
        //            txscope.Dispose();
        //            Common.LogError(ex, "MyImages-Post");
        //            return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
        //        }
        //    }
        //}

        //[HttpPost, Route("RemoveMyImage/{id?}")]
        //public JsonResult RemoveMyImage(int id)
        //{
        //    var seniorCareImage = _seniorCareImage.GetById(id);
        //    Common.DeleteFile(seniorCareImage.ImagePath);
        //    _seniorCareImage.DeleteData(seniorCareImage);
        //    _seniorCareImage.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = "Image deleted successfully" });
        //}
        //#endregion

        //#region OpeningHours
        //[Route("SeniorCare/OpeningHours")]
        //public ActionResult OpeningHours()
        //{
        //    return View();
        //}

        //public ActionResult GetOpeningHoursList(JQueryDataTableParamModel param)
        //{
        //    _careId = GetSeniorCareId();
        //    var allOpeningHours = _openingHour.GetAll(x => !x.IsDeleted && x.SeniorCareId == _careId).Select(x => new OpeningHoursViewModel()
        //    {
        //        Id = x.OpeningHourId,
        //        DayOfWeek = Common.GetDayofWeek(x.WeekDay),
        //        Start = x.StartTime.ToString("hh:mm tt"),
        //        End = x.EndTime.ToString("hh:mm tt"),
        //        CreatedDate = x.CreatedDate.ToDefaultFormate(),
        //        UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
        //        IsActive = x.IsActive
        //    }).ToList();

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        allOpeningHours = allOpeningHours.Where(x => x.DayOfWeek.NullToString().ToLower().Contains(param.sSearch.ToLower())
        //                                                  || x.Start.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                                  || x.End.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                                  || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                                  || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
        //    }
        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

        //    Func<OpeningHoursViewModel, string> orderingFunction =
        //        c => sortColumnIndex == 1 ? c.DayOfWeek
        //                    : sortColumnIndex == 2 ? c.Start
        //                        : sortColumnIndex == 3 ? c.End
        //                            : sortColumnIndex == 4 ? c.CreatedDate
        //                                : sortColumnIndex == 5 ? c.UpdatedDate
        //                                    : c.DayOfWeek;
        //    allOpeningHours = sortDirection == "asc" ? allOpeningHours.OrderBy(orderingFunction).ToList() : allOpeningHours.OrderByDescending(orderingFunction).ToList();

        //    var display = allOpeningHours.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var total = allOpeningHours.Count;

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet, Route("_OpeningHours/{id?}")]
        //public PartialViewResult OpeningHours(int id)
        //{
        //    _careId = GetSeniorCareId();
        //    if (id == 0) return PartialView(@"Partial/_OpeningHours", new OpeningHoursViewModel() { SeniorCareId = _careId });
        //    var slot = _openingHour.GetById(id);
        //    var result = AutoMapper.Mapper.Map<OpeningHoursViewModel>(slot);
        //    if (result == null) return PartialView(@"Partial/_OpeningHours", new OpeningHoursViewModel() { SeniorCareId = _careId });
        //    result.Start = result.StartTime.ToString("HH:mm");
        //    result.End = result.EndTime.ToString("HH:mm");
        //    return PartialView(@"Partial/_OpeningHours", result);
        //}

        //[HttpPost, Route("AddEditOpeningHours"), ValidateAntiForgeryToken]
        //public JsonResult AddEditOpeningHours(OpeningHoursViewModel model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            DateTime toDay = DateTime.Today;
        //            model.StartTime = toDay.Add(TimeSpan.Parse(model.Start));
        //            model.EndTime = toDay.Add(TimeSpan.Parse(model.End));
        //            if (ModelState.IsValid)
        //            {
        //                if (model.OpeningHourId == 0)
        //                {
        //                    var bResult = _openingHour.GetSingle(x => x.IsActive && !x.IsDeleted && x.WeekDay == model.WeekDay);
        //                    if (bResult != null)
        //                    {
        //                        txscope.Dispose();
        //                        return Json(new JsonResponse { Status = 0, Message = "OpeningHours already exists. Could not add the data!" });
        //                    }
        //                    var oHour = AutoMapper.Mapper.Map<OpeningHoursViewModel, OpeningHour>(model);
        //                    oHour.IsActive = true;
        //                    _openingHour.InsertData(oHour);
        //                    _openingHour.SaveData();
        //                    txscope.Complete();
        //                    return Json(new JsonResponse { Status = 1, Message = "OpeningHours save successfully" });
        //                }
        //                else
        //                {
        //                    var bResult = _openingHour.GetSingle(x => x.OpeningHourId != model.OpeningHourId && x.WeekDay == model.WeekDay && x.IsActive && !x.IsDeleted);
        //                    if (bResult != null)
        //                    {
        //                        txscope.Dispose();
        //                        return Json(new JsonResponse { Status = 0, Message = "OpeningHours already exists. Could not add the data!" });
        //                    }
        //                    var oHour = _openingHour.GetById(model.OpeningHourId);
        //                    oHour.WeekDay = model.WeekDay;
        //                    oHour.StartTime = model.StartTime;
        //                    oHour.EndTime = model.EndTime;
        //                    _openingHour.UpdateData(oHour);
        //                    _openingHour.SaveData();
        //                    txscope.Complete();
        //                    return Json(new JsonResponse { Status = 1, Message = "OpeningHours update successfully" });
        //                }
        //            }
        //            else
        //            {
        //                return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            Common.LogError(ex, "AddEditOpeningHours-Post");
        //            return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
        //        }
        //    }
        //}

        //[HttpPost, Route("IsActiveOpeningHours/{id?}")]
        //public JsonResult IsActiveOpeningHours(int id)
        //{
        //    var oHour = _openingHour.GetById(id);
        //    oHour.IsActive = !oHour.IsActive;
        //    _openingHour.UpdateData(oHour);
        //    _openingHour.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = $@"Opening Hours {(oHour.IsActive ? "IsActive" : "DeActive")} successfully" });
        //}

        //[HttpPost, Route("RemoveOpeningHours/{id?}")]
        //public JsonResult RemoveOpeningHours(int id)
        //{
        //    var oHour = _openingHour.GetById(id);
        //    oHour.IsDeleted = true;
        //    _openingHour.UpdateData(oHour);
        //    _openingHour.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = "Opening Hours deleted successfully" });
        //}
        //#endregion

        //#region Locations

        //[HttpGet, Route("MyLocations")]
        //public ActionResult MyLocations()
        //{
        //    return View();
        //}

        //public ActionResult GetLocationList(JQueryDataTableParamModel param)
        //{
        //    _careId = GetSeniorCareId();
        //    var locations = _address.GetAll(x => x.IsActive && !x.IsDeleted && x.SeniorCareId == _careId).Select(x => new AddressViewModel()
        //    {
        //        Id = x.AddressId,
        //        CityName = x.City.CityName,
        //        StateName = x.State.StateName,
        //        Country = x.Country,
        //        Address1 = x.Address1,
        //        Address2 = x.Address2,
        //        ZipCode = x.ZipCode,
        //        CreatedDate = x.CreatedDate.ToDefaultFormate(),
        //        UpdatedDate = x.UpdatedDate.ToDefaultFormate()
        //    }).ToList();

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        locations = locations.Where(x => x.CityName.NullToString().ToLower().Contains(param.sSearch.ToLower())
        //                                               || x.StateName.NullToString().ToLower().Contains(param.sSearch.ToLower())
        //                                               || x.Country.NullToString().ToLower().Contains(param.sSearch.ToLower())
        //                                               || x.Address1.NullToString().ToLower().Contains(param.sSearch.ToLower())
        //                                               || x.Address2.NullToString().ToLower().Contains(param.sSearch.ToLower())
        //                                               || x.ZipCode.NullToString().ToLower().Contains(param.sSearch.ToLower())
        //                                               || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                               || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
        //    }
        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

        //    Func<AddressViewModel, string> orderingFunction =
        //             c => sortColumnIndex == 1 ? c.Address1
        //                 : sortColumnIndex == 2 ? c.Address2
        //                    : sortColumnIndex == 3 ? c.CityName
        //                        : sortColumnIndex == 4 ? c.StateName
        //                            : sortColumnIndex == 5 ? c.Country
        //                                : sortColumnIndex == 6 ? c.ZipCode
        //                                    : sortColumnIndex == 7 ? c.CreatedDate
        //                                        : c.UpdatedDate;
        //    locations = sortDirection == "asc" ? locations.OrderBy(orderingFunction).ToList() : locations.OrderByDescending(orderingFunction).ToList();


        //    var display = locations.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var total = locations.Count;

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet, Route("MySeniorCareLocations/{id?}")]
        //public PartialViewResult MySeniorCareLocations(int id)
        //{
        //    _careId = GetSeniorCareId();
        //    ViewBag.StateList = _state.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
        //    {
        //        Text = x.StateName,
        //        Value = x.StateId.ToString()
        //    }).ToList();

        //    if (id == 0) return PartialView(@"Partial/_MyLocation", new AddressViewModel() { SeniorCareId = _careId });
        //    var seniorCareAddress = _address.GetById(id);
        //    var result = AutoMapper.Mapper.Map<AddressViewModel>(seniorCareAddress);
        //    if (result == null) return PartialView(@"Partial/_MyLocation", new AddressViewModel());
        //    return PartialView(@"Partial/_MyLocation", result);
        //}

        //[HttpPost, ValidateAntiForgeryToken, Route("AddEditSeniorCareLocation")]
        //public JsonResult AddEditSeniorCareLocation(AddressViewModel model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var address = new Address
        //            {
        //                SeniorCareId = model.SeniorCareId,
        //                IsActive = true,
        //                IsDeleted = false,
        //                Address1 = model.Address1,
        //                Address2 = model.Address2,
        //                CityId = model.CityId,
        //                StateId = model.StateId,
        //                ZipCode = model.ZipCode,
        //                Country = model.Country,
        //                IsDefault = model.IsDefault
        //            };
        //            if (model.IsDefault)
        //            {
        //                var locations = _address.GetAll(x => x.SeniorCareId == model.SeniorCareId);
        //                foreach (var item in locations)
        //                {
        //                    item.IsDefault = false;
        //                    _address.UpdateData(item);
        //                }
        //            }
        //            if (model.AddressId > 0)
        //            {
        //                address.AddressId = model.AddressId;
        //                _address.UpdateData(address);
        //            }
        //            else
        //            {
        //                _address.InsertData(address);
        //            }
        //            _address.SaveData();
        //            txscope.Complete();
        //            return Json(new JsonResponse() { Status = 1, Message = "Address saved successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            Common.LogError(ex, "AddReview-Post");
        //            return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        //[HttpPost, Route("RemoveSeniorCareLocation/{id?}")]
        //public JsonResult RemoveSeniorCareLocation(int id)
        //{
        //    var address = _address.GetById(id);
        //    _address.DeleteData(address);
        //    _address.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = "Address removed successfully" });
        //}

        //#endregion

        //#region Controller Common
        //private int GetSeniorCareId()
        //{
        //    int userId = User.Identity.GetUserId<int>();
        //    return _seniorCare.GetSingle(x => x.UserId == userId).SeniorCareId;
        //}
        //#endregion


        [HttpPost, Route("SaveSeniorCareUser/")]
        public JsonResult SaveSeniorCareUser(UserAndAddress userAndAddress)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    ApplicationUser applicationUser = new ApplicationUser();
                    Address address = new Address();
                    var applicationUserId = 0;
                    Patient patient = new Patient();
                    var isExist = _userManager.FindByEmailAsync(userAndAddress.Email);

                    if (isExist == null || isExist.Result == null)
                    {
                        applicationUser.CreatedDate = DateTime.Now;
                        applicationUser.LastResetPassword = DateTime.Now;
                        applicationUser.IsActive = true;
                        applicationUser.UserName = userAndAddress.Email;
                        applicationUser.DateOfBirth = userAndAddress.DateOfBirth;
                        applicationUser.Email = userAndAddress.Email;
                        applicationUser.FirstName = userAndAddress.FirstName;
                        applicationUser.LastName = userAndAddress.LastName;
                        applicationUser.PhoneNumber = userAndAddress.PhoneNumber;

                        var result = _userManager.CreateAsync(applicationUser, "Patient@123");
                        if (!result.Result.Succeeded)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        var createUser = _userManager.FindByEmail(userAndAddress.Email);
                        if (createUser != null && createUser.Id != 0)
                        {
                            //applicationUserId = createUser.Id;
                            //patient.UserId = createUser.Id;
                            //patient.IsActive = true;
                            //_patient.InsertData(patient);
                            //_patient.SaveData();

                            //var patientDetails = _patient.GetSingle(x => x.UserId == applicationUserId)?.PatientId;

                            //if (patientDetails != null)
                            //{
                            //    address.PatientId = patientDetails;
                            //    address.Address1 = userAndAddress.Address;
                            //    address.CityId = userAndAddress.CityId;
                            //    address.StateId = userAndAddress.StateId;
                            //    address.ZipCode = userAndAddress.Zipcode;
                            //    _address.InsertData(address);
                            //    _address.SaveData();
                            //}
                        }

                        txscope.Complete();
                    }
                    else
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse() { Status = 0, Message = "Email already exist.", Data = "" }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new JsonResponse() { Status = 200, Message = "Data Saved Successfully", Data = applicationUserId }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "Patent-Book-POST");
                    return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Route("SeniorCareStatusChange/{id?}/{flag?}/{isstatus?}")]
        [HttpPost]
        public JsonResult SeniorCareStatusChange(int id, int flag = 0, int IsStatus = 0)
        {
            if (id > 0)
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSeniorCare_StatusChange  " +
                    "@OrganizationId,@Flag,@IsStatus", new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = id }, new SqlParameter("Flag", System.Data.SqlDbType.Int) { Value = flag }, new SqlParameter("IsStatus", System.Data.SqlDbType.Int) { Value = IsStatus });
                return Json(new JsonResponse() { Status = 1, Message = "Senior Care Status Update successfully" });
            }
            else
            {
                return Json(new JsonResponse() { Status = 0, Message = "Something is wrong" });
            }
        }


        private SeniorCareDataListModel GetSeniorCareSearchResult(SearchParamModel model, int organizationTypeId)
        {
            try
            {
                int userIda = User.Identity.GetUserId<int>();
                //var userData = _repo.Find<ApplicationUser>(userIda);


                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationTypeID",SqlDbType.Int) {Value = organizationTypeId},
                    new SqlParameter("@Sorting",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex-1},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                    new SqlParameter("@UserTypeID", System.Data.SqlDbType.Int) { Value =  (int)UserTypes.SeniorCare},
                };

                int totalRecord = 0;
                var allslotList = _seniorCare.GetSeniorCareListByTypeId(StoredProcedureList.GetSeniorCareListByType, parameters, out totalRecord).ToList();
                var searchResult = new SeniorCareDataListModel() { SeniorCareDataList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecord = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PartialViewResult SeniorCareCreate(int? id)
        {
            if (id > 0)
            {
                ViewBag.ID = id;


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
                    }).First();

                //var orgInfo = _repo.ExecWithStoreProcedure<Organisation>("spGetOrganizationInfoByID @orgnizationID",
                //    new SqlParameter("orgnizationID", System.Data.SqlDbType.Int) { Value = id }
                //    );

                return PartialView(@"/Views/SeniorCare/Partial/_SeniorCareCreateProfile.cshtml", orgInfo);

                //return View(orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrganisationProfileViewModel();
                //return View(orgInfo);
                orgInfo.OrganizationTypeID = (int)OrganisationTypes.SeniorCare;
                orgInfo.UserTypeID = (int)UserTypes.SeniorCare;
                return PartialView(@"/Views/SeniorCare/Partial/_SeniorCareCreateProfile.cshtml", orgInfo);
            }
        }

        [HttpPost, Route("AddEditSeniorCareProfile"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareProfile(OrganisationProfileViewModel model, HttpPostedFileBase Image1)
        {
            try
            {
                string imagePath = "";
                model.UserId = User.Identity.GetUserId<int>();
                if (Image1 != null && Image1.ContentLength > 0)
                {
                    //Changes Made against Issue#25
                    if (!string.IsNullOrWhiteSpace(model.LogoFilePath))
                    {
                        string pathOld = Path.Combine(Server.MapPath(FilePathList.SeniorCare), model.LogoFilePath);
                        FileInfo file = new FileInfo(pathOld);
                        if (file.Exists)//check file exsit or not  
                        {
                            file.Delete();
                        }
                    }

                    DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/SeniorCareProfile/"));
                    if (!dir.Exists)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/SeniorCareProfile"));
                    }

                    string extension = Path.GetExtension(Image1.FileName);
                    string newImageName = "SeniorCare-" + DateTime.Now.Ticks.ToString() + extension;

                    var path = Path.Combine(Server.MapPath("~/Uploads/SeniorCareProfile"), newImageName);
                    Image1.SaveAs(path);

                    imagePath = newImageName;
                    Session["SeniorCareProfileImg"] = "/Uploads/SeniorCareProfile/" + newImageName;
                }

                if (model.OrganisationId > 0 && imagePath == "" && Image1!=null)
                {
                    imagePath = model.LogoFilePath;
                }

                if(Image1==null) 
                {
                    imagePath = "";
                }

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

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganisation_Create_SeniorCare " +
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
                                      new SqlParameter("@OrganisationId", System.Data.SqlDbType.Int) { Value = model.OrganisationId > 0 ? model.OrganisationId : 0 },
                                      new SqlParameter("@UserId", System.Data.SqlDbType.Int) { Value = (object)model.UserId ?? 0 },
                                      new SqlParameter("@OrganizationTypeID", System.Data.SqlDbType.Int) { Value = model.OrganizationTypeID },
                                      new SqlParameter("@UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                                      new SqlParameter("@OrganisationName", System.Data.SqlDbType.VarChar) { Value = model.OrganisationName },
                                      new SqlParameter("@OrganisationSubpart", System.Data.SqlDbType.NChar) { Value = (object)model.OrganisationSubpart ?? DBNull.Value },
                                      new SqlParameter("@EnumerationDate", System.Data.SqlDbType.Date) { Value = (object)model.EnumerationDate ?? DBNull.Value },
                                      new SqlParameter("@Status", System.Data.SqlDbType.NChar) { Value = " " },
                                      new SqlParameter("@AuthorisedOfficialCredential", System.Data.SqlDbType.NChar) { Value = (object)model.AuthorisedOfficialCredential ?? DBNull.Value },
                                      new SqlParameter("@AuthorizedOfficialFirstName", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialFirstName ?? DBNull.Value },
                                      new SqlParameter("@AuthorizedOfficialLastName", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialLastName ?? DBNull.Value },
                                      new SqlParameter("@AuthorizedOfficialTelephoneNumber", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialTelephoneNumber ?? DBNull.Value },
                                      new SqlParameter("@AuthorizedOfficialTitleOrPosition", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialTitleOrPosition ?? DBNull.Value },
                                      new SqlParameter("@AuthorizedOfficialNamePrefix", System.Data.SqlDbType.VarChar) { Value = (object)model.AuthorizedOfficialNamePrefix ?? DBNull.Value },
                                      new SqlParameter("@CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                      new SqlParameter("@IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                      new SqlParameter("@ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = (object)model.ApplicationUser_Id ?? 0 },
                                      new SqlParameter("@AliasBusinessName", System.Data.SqlDbType.NVarChar) { Value = (object)model.AliasBusinessName ?? DBNull.Value },
                                      new SqlParameter("@OrganizatonEIN", System.Data.SqlDbType.NVarChar) { Value = (object)model.OrganizatonEIN ?? DBNull.Value },
                                      new SqlParameter("@NPI", System.Data.SqlDbType.NVarChar) { Value = model.NPI },
                                      new SqlParameter("@EnabledBooking", System.Data.SqlDbType.Bit) { Value = (object)model.EnabledBooking ?? DBNull.Value },
                                      //Changes made against Issue#25
                                      new SqlParameter("@LogoFilePath", System.Data.SqlDbType.NVarChar) { Value = imagePath ?? "" },
                                      new SqlParameter("@ShortDescription", System.Data.SqlDbType.NVarChar) { Value = (object)model.ShortDescription ?? DBNull.Value },
                                      new SqlParameter("@LongDescription", System.Data.SqlDbType.NVarChar) { Value = (object)model.LongDescription ?? DBNull.Value }
                                  );

                //int ExecWithStoreProcedure = 0;

                //var seniorModel = new Organisation();
                //seniorModel.OrganisationId = model.OrganisationId > 0 ? model.OrganisationId : 0;
                //seniorModel.UserId = model.UserId ?? User.Identity.GetUserId<int>();
                //seniorModel.OrganizationTypeID = model.OrganizationTypeID;
                //seniorModel.UserTypeID = 5;
                //seniorModel.OrganisationName = model.OrganisationName;
                //seniorModel.OrganisationSubpart = model.OrganisationSubpart ?? null;
                //seniorModel.EnumerationDate = model.EnumerationDate ?? null;
                //seniorModel.Status = " ";
                //seniorModel.AuthorisedOfficialCredential = model.AuthorisedOfficialCredential ?? null;
                //seniorModel.AuthorizedOfficialFirstName = model.AuthorizedOfficialFirstName ?? null;
                //seniorModel.AuthorizedOfficialLastName = model.AuthorizedOfficialLastName ?? null;
                //seniorModel.AuthorizedOfficialTelephoneNumber = model.AuthorizedOfficialTelephoneNumber ?? null;
                //seniorModel.AuthorizedOfficialTitleOrPosition = model.AuthorizedOfficialTitleOrPosition ?? null;
                //seniorModel.AuthorizedOfficialNamePrefix = model.AuthorizedOfficialNamePrefix ?? null;
                //seniorModel.IsActive = model.IsActive;
                //seniorModel.ApplicationUser_Id = model.ApplicationUser_Id ?? User.Identity.GetUserId<int>();
                //seniorModel.AliasBusinessName = model.AliasBusinessName ?? null;
                //seniorModel.OrganizatonEIN = model.OrganizatonEIN ?? null;
                //seniorModel.NPI = model.NPI;
                //seniorModel.EnabledBooking = model.EnabledBooking ?? null;
                //seniorModel.LogoFilePath = imagePath;
                //seniorModel.ShortDescription = model.ShortDescription ?? null;
                //seniorModel.LongDescription = model.LongDescription ?? null;
                //seniorModel.IsDeleted = false;

                //if (model.OrganisationId > 0)
                //{
                //    seniorModel.ModifiedBy = User.Identity.GetUserId<int>();
                //    seniorModel.UpdatedDate = Convert.ToDateTime(DateTime.Now) > DateTime.MinValue ? Convert.ToDateTime(DateTime.Now) : (DateTime?)null;
                //    ExecWithStoreProcedure = _repo.Update<Organisation>(seniorModel, true);
                //}
                //else
                //{
                //    seniorModel.CreatedBy = User.Identity.GetUserId<int>();
                //    seniorModel.CreatedDate = DateTime.Now;
                //    seniorModel = _repo.Insert<Organisation>(seniorModel, true);
                //}

                //ExecWithStoreProcedure = seniorModel.OrganisationId;

                if (model.OrganisationId == 0)
                {
                    //ExecWithStoreProcedure = _seniorCare.GetAll().OrderByDescending(w => w.OrganisationId).FirstOrDefault().OrganisationId;
                    ExecWithStoreProcedure = _repo.Find<Organisation>(x => x.NPI == model.NPI).OrganisationId;
                }
                else
                {
                    ExecWithStoreProcedure = model.OrganisationId;
                }

                return Json(new JsonResponse() { Status = ExecWithStoreProcedure, Message = "SeniorCare profile save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "SeniorCare profile saving Error.." });
            }

        }

        #region :: Reviews :: 

        [Route("SeniorCare/Reviews")]
        public ActionResult Reviews()
        {
            return View();
        }
        [Route("SeniorCare/GetReviewList/{id}")]
        public ActionResult GetReviewList(JQueryDataTableParamModel param, int id = 0)
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
                    new SqlParameter("@UserTypeID",SqlDbType.Int) {Value = 5 },
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
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editReviews.IsActive == true ? 1 : 0 });
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
            try
            {
                if (createReview.ReferenceId > 0)
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
                                              new SqlParameter("ReviewId", System.Data.SqlDbType.BigInt) { Value = createReview.ReviewId > 0 ? createReview.ReviewId : 0 },
                                              new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createReview.ReferenceId },
                                              new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (object)createReview.Description ?? DBNull.Value },
                                              new SqlParameter("Rating", System.Data.SqlDbType.Int) { Value = (object)createReview.Rating ?? DBNull.Value },
                                              new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = (object)createReview.IsActive ?? DBNull.Value },
                                              new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 },
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                              new SqlParameter("ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                              new SqlParameter("Doctor_DoctorId", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                              new SqlParameter("SeniorCare_SeniorCareId", System.Data.SqlDbType.Int) { Value = (object)DBNull.Value },
                                              new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.SeniorCare }
                                          );

                    return Json(new JsonResponse() { Status = 1, Message = "SeniorCare Review info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "SeniorCare Review info saving Error.." });
            }
            //var parameters = new List<SqlParameter>();
            //        parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createReview.ReferenceId });
            //        parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = createReview.Description });
            //        parameters.Add(new SqlParameter("Rating", SqlDbType.Int) { Value = createReview.Rating });
            //        parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createReview.IsActiveString == "on" ? 1 : 0 });
            //        parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            //        parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            //        parameters.Add(new SqlParameter("ApplicationUser_Id", System.Data.SqlDbType.Int) { Value = null });
            //        parameters.Add(new SqlParameter("Doctor_DoctorId", System.Data.SqlDbType.Int) { Value = null });
            //        parameters.Add(new SqlParameter("SeniorCare_SeniorCareId", System.Data.SqlDbType.Int) { Value = null });
            //        parameters.Add(new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.SeniorCare });
            //        parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            //        try
            //        {
            //            _facility.ExecuteSqlCommandForInsert("Review", parameters);
            //            return Json(new JsonResponse { Status = 1, Message = "Review Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            //        }
            //        catch (Exception ex)
            //        {
            //            return Json(new JsonResponse { Status = 0, Message = "Error occured in creating review", Data = new { } }, JsonRequestBehavior.AllowGet);
            //        }
        }

        [Route("SeniorCare/DeleteSeniorCareReview/{id}")]
        [HttpPost]
        public JsonResult DeleteSeniorCareReview(int id)
        {
            try
            {

                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spReview_Remove @ReviewId, @ModifiedBy",
                    new SqlParameter("ReviewId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare review has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"SeniorCare review deleted Error.." });
            }
        }

        #endregion 

        #region :: Cost ::

        [Route("SeniorCare/Cost")]
        public ActionResult Cost()
        {
            return View();
        }

        [Route("SeniorCare/GetSeniorCareCostList/{id?}")]
        public ActionResult GetSeniorCareCostList(JQueryDataTableParamModel param, int id = 0)
        {
            var allCosts = _repo.ExecWithStoreProcedure<CostViewModel>("spCost_GetList @Search, @UserTypeID, @OrganizationID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.SeniorCare },
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

        //--- Add/Edit Cost
        [HttpPost]
        //[HttpPost, Route("SeniorCare/AddEditSeniorCareCost"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareCost(CostViewModel model)
        {
            try
            {
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
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.SeniorCare },
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

        [Route("SeniorCare/DeleteSeniorCareCost/{id}")]
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

        #region :: Booking ::
        /*
        [Route("SeniorCare/Booking")]
        public ActionResult Booking()
        {
            return View();
        }

        [Route("SeniorCare/GetBookingList/{id}")]
        public ActionResult GetBookingList(int id, JQueryDataTableParamModel param)
        {
            try
            {

                var allBookingList = _facility.ExecWithStoreProcedure<OrganisationSlotList>("SpGetBookingByOrgId @Search, @Sort, @PageIndex, @PageSize, @ReferenceId",
                        new SqlParameter("Search", System.Data.SqlDbType.NVarChar) { Value = param.sSearch == null ? " " : param.sSearch },
                        new SqlParameter("Sort", System.Data.SqlDbType.NVarChar) { Value = !string.IsNullOrEmpty(param.sSortDir_0) ? param.sSortDir_0 : "Asc" },
                        new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                        new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                        new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id }
                        ).ToList();

                foreach (var item in allBookingList)
                {
                    item.Address = this.GetAddressById(item.AddressId);
                }

                int TotalRecordCount = 0;
                var data = allBookingList.ToList();
                if (data.Count > 0)
                    TotalRecordCount = data[0].TotalRows;


                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetAddressById(int AddressId)
        {
            var addressList = _facility.SQLQuery<DrpAddress>("select Address1, Address2 from Address where AddressId = " + AddressId).ToList();
            var address = new DrpAddress();
            if (addressList.Count > 0)
                address = addressList[0];
            return address.Address1 + (!string.IsNullOrEmpty(address.Address1) ? ", " : "") + address.Address2;
        }
        [HttpPost]
        public JsonResult CreateBooking(OrganisationSlotList createSlot)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createSlot.ReferenceId });
            parameters.Add(new SqlParameter("SlotDate", SqlDbType.VarChar) { Value = createSlot.SlotDate });
            parameters.Add(new SqlParameter("SlotTime", SqlDbType.VarChar) { Value = createSlot.SlotTime });
            parameters.Add(new SqlParameter("AddressId", SqlDbType.Int) { Value = createSlot.AddressId });
            parameters.Add(new SqlParameter("BookedFor", SqlDbType.Int) { Value = createSlot.BookedFor });
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = createSlot.InsurancePlanId });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = createSlot.Description });
            parameters.Add(new SqlParameter("IsBooked", SqlDbType.Bit) { Value = createSlot.IsBookedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsEmailReminder", SqlDbType.Bit) { Value = createSlot.IsEmailReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsTextReminder", SqlDbType.Bit) { Value = createSlot.IsTextReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsInsuranceChanged", SqlDbType.Bit) { Value = createSlot.IsInsuranceChangedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createSlot.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".",":") });
            try
            {
                _facility.ExecuteSqlCommandForInsert("Slot", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Booking Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating booking", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateBooking(OrganisationSlotList editBooking)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SlotDate", SqlDbType.VarChar) { Value = editBooking.SlotDate });
            parameters.Add(new SqlParameter("SlotTime", SqlDbType.VarChar) { Value = editBooking.SlotTime });
            parameters.Add(new SqlParameter("AddressId", SqlDbType.Int) { Value = editBooking.AddressId });
            parameters.Add(new SqlParameter("BookedFor", SqlDbType.Int) { Value = editBooking.BookedFor });
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = editBooking.InsurancePlanId });
            parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { Value = editBooking.Description });
            parameters.Add(new SqlParameter("IsBooked", SqlDbType.Bit) { Value = editBooking.IsBookedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsEmailReminder", SqlDbType.Bit) { Value = editBooking.IsEmailReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsTextReminder", SqlDbType.Bit) { Value = editBooking.IsTextReminderString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsInsuranceChanged", SqlDbType.Bit) { Value = editBooking.IsInsuranceChangedString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editBooking.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("Slot", "SlotId", editBooking.SlotId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Booking Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating booking", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("SeniorCare/GetDrpAddressList/{id}")]
        public ActionResult GetDrpAddressList(string Prefix, int id)
        {
            try
            {
                //var addList = _facility.SQLQuery<DrpAddress>("select Address1, Address2, AddressId from Address where " +
                //    "(Address1 like '%" + Prefix + "%'" + " or Address2 like '%" + Prefix + "%') and " +
                //    " ReferenceId = '" + id + "'").ToList();
                var addList = _facility.SQLQuery<DrpAddress>("select Address1, Address2, AddressId from Address where " +
                    "(Address1 like '%" + Prefix + "%'" + " or Address2 like '%" + Prefix + "%') ").ToList();
                return Json(addList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("SeniorCare/GetDrpUserList")]
        public ActionResult GetDrpUserList(string Prefix)
        {
            try
            {
                var query = "select anu.Id, anu.FirstName, anu.MiddleName, anu.LastName from Patient p join AspNetUsers anu on p.UserId = anu.Id " +
                    "where " +
                    "(FirstName like '%" + Prefix + "%'" + " or MiddleName like '%" + Prefix + "%'" + " or LastName like '%" + Prefix + "%')";
                var userList = _facility.SQLQuery<DrpUser>(query).ToList();
                return Json(userList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("SeniorCare/GetDrpInsurancePlanList")]
        public ActionResult GetDrpInsurancePlanList(string Prefix)
        {
            try
            {
                var userList = _facility.SQLQuery<DrpInsurancePlan>("select InsurancePlanId, Name from InsurancePlan where " +
                    "Name like '%" + Prefix + "%'").ToList();
                return Json(userList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        */
        #endregion

        #region Booking

        // GET: Booking
        public ActionResult Booking(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["TempDataData"] = "Yes";
                ViewBag.SeniorCareID = id;
                //Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("SeniorCare"))
                {
                    //int userId = User.Identity.GetUserId<int>();
                    //var userInfo = _appUser.GetById(userId);
                    //string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1007 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    //TempData["PharmacyData"] = "Yes";
                    //ViewBag.SeniorCareID = pharmacyInfo.First().OrganisationId;
                    //Session["PharmacyID"] = pharmacyInfo.First().OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetSeniorCareBookingList/{flag}/{id}")]
        public ActionResult GetSeniorCareBookingList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spSeniorDocOrgBooking_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1007 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = orgInfo
                .Select(x => new OrgBookingListViewModel()
                {
                    SlotId = x.SlotId,
                    //DoctorId = x.DoctorId,
                    OrganisationName = x.OrganisationName,
                    OrganisationId = x.ReferenceId,
                    //DoctorName = x.DoctorName + " [" + x.Credential + "]",
                    OrganizatonTypeID = x.OrganizatonTypeID.HasValue ? x.OrganizatonTypeID.Value : 0,
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

        public PartialViewResult SeniorCareBooking(int? id, int seniorCareId = 0)
        {
            ViewBag.SeniorCareID = seniorCareId;


            //var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
            //    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.SeniorCareID },
            //    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 }
            //    ).Select(x => new OrgAddressDropdownViewModel()
            //    {
            //        AddressId = x.AddressId,
            //        FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country
            //    });

            //ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress");

            var doctorsList = _repo.ExecWithStoreProcedure<OrgDoctorsDropDownViewModel>("spDocOrgBooking_GetOrgDoc @OrganizationId",
               new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ViewBag.SeniorCareID }
               ).Select(x => new OrgDoctorsDropDownViewModel()
               {
                   DoctorId = x.DoctorId,
                   DisplayName = x.DoctorName + " [" + x.Credential + "]"
               });

            ViewBag.doctorsList = new SelectList(doctorsList.OrderBy(o => o.DoctorName), "DoctorId", "DisplayName");

            var typeList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsuranceTypeDropDownList_Get");
            ViewBag.typeList = new SelectList(typeList.OrderBy(o => o.InsuranceTypeId), "InsuranceTypeId", "Name");

            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get");
            //var planList = new List<InsurancePlanDropDownViewModel>();
            ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name");

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgInfo = _repo.ExecWithStoreProcedure<OrgBookingViewModel>("spSeniorCareDocOrgBooking_GetById @SlotId,@OrganizationId",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ViewBag.SeniorCareID }
                    ).Select(x => new OrgBookingUpdateViewModel
                    {
                        SlotId = x.SlotId,
                        OrganisationId = x.OrganisationId.HasValue ? x.OrganisationId.Value : 0,
                        DoctorId = x.DoctorId,
                        AddressId = x.AddressId,
                        BookedFor = x.BookedFor,
                        OrganizatonTypeID = 1007,
                        OrganisationName = x.OrganisationName,
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
                        InsurancePlanId = x.InsurancePlanId.HasValue ? x.InsurancePlanId.Value : 0,
                        InsuranceTypeId = x.InsuranceTypeId.HasValue ? x.InsuranceTypeId.Value : 0
                    }).First();

                var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddressInfoByID @AddressID",
           new SqlParameter("AddressID", System.Data.SqlDbType.Int) { Value = orgInfo.AddressId }
           ).Select(x => new OrgAddressDropdownViewModel()
           {
               AddressId = x.AddressId,
               FullAddress = x.Address1 + " " + x.Address2 + " " + GetCityStateInfoById(x.CityStateZipCodeID, "zip") + " " + GetCityStateInfoById(x.CityStateZipCodeID, "city") + " " + GetCityStateInfoById(x.CityStateZipCodeID, "state") + " " + GetCityStateInfoById(x.CityStateZipCodeID, "country")
           });

                ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress", orgInfo.AddressId);
                //ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress", orgInfo.AddressId);
                ViewBag.planList = new SelectList(planList.OrderBy(o => o.InsurancePlanId), "InsurancePlanId", "Name", orgInfo.InsurancePlanId);
                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyBooking.cshtml", orgInfo);
            }
            else
            {
                var addressList = _repo.ExecWithStoreProcedure<OrgAddressDropdownViewModel>("spGetAddress_ByReference @ReferenceId, @UserTypeID",
                new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = ViewBag.SeniorCareID },
                new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 }
                ).Select(x => new OrgAddressDropdownViewModel()
                {
                    AddressId = x.AddressId,
                    FullAddress = x.Address1 + " " + x.Address2 + " " + x.ZipCode + " " + x.City + " " + x.State + " " + x.Country
                });

                ViewBag.addressList = new SelectList(addressList.OrderBy(o => o.AddressId), "AddressId", "FullAddress");
                ViewBag.ID = 0;
                var orgInfo = new OrgBookingUpdateViewModel();
                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyBooking.cshtml", orgInfo);
            }
        }


        //--- Add Update SeniorCare Booking Info

        //[HttpPost, Route("AddEditSeniorCareBooking"), ValidateAntiForgeryToken]
        //public JsonResult AddEditSeniorCareBooking(OrgBookingUpdateViewModel model)
        //{
        //    try
        //    {
        //        if (model.OrganisationId > 0)
        //        {
        //            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Create " +
        //                    "@SlotId," +
        //                    "@SlotDate," +
        //                    "@SlotTime," +
        //                    "@ReferenceId," +
        //                    "@BookedFor," +
        //                    "@IsBooked," +
        //                    "@IsEmailReminder," +
        //                    "@IsTextReminder," +
        //                    "@IsInsuranceChanged," +
        //                    "@IsActive," +
        //                    "@InsurancePlanId," +
        //                    "@AddressId," +
        //                    "@Description," +
        //                    "@CreatedBy," +
        //                    "@UserTypeID",
        //                                  new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = model.SlotId > 0 ? model.SlotId : 0 },
        //                                  new SqlParameter("SlotDate", System.Data.SqlDbType.NVarChar) { Value = model.SlotDate },
        //                                  new SqlParameter("SlotTime", System.Data.SqlDbType.NVarChar) { Value = model.SlotTime },
        //                                  new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = model.DoctorId },
        //                                  new SqlParameter("BookedFor", System.Data.SqlDbType.Int) { Value = model.BookedFor },
        //                                  new SqlParameter("IsBooked", System.Data.SqlDbType.Bit) { Value = model.IsBooked },
        //                                  new SqlParameter("IsEmailReminder", System.Data.SqlDbType.Bit) { Value = model.IsEmailReminder },
        //                                  new SqlParameter("IsTextReminder", System.Data.SqlDbType.Bit) { Value = model.IsTextReminder },
        //                                  new SqlParameter("IsInsuranceChanged", System.Data.SqlDbType.Bit) { Value = model.IsInsuranceChanged },
        //                                  new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
        //                                  new SqlParameter("InsurancePlanId", System.Data.SqlDbType.Int) { Value = model.InsurancePlanId },
        //                                  new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = model.AddressId },
        //                                  new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = model.Description },
        //                                  new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
        //                                  new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 }

        //                              );

        //            return Json(new JsonResponse() { Status = 1, Message = "SeniorCare booking info save successfully" });
        //        }
        //        else
        //        {
        //            return Json(new JsonResponse() { Status = 0, Message = "Error..! SeniorCare Info Not Found! Should be select SeniorCare Name." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new JsonResponse() { Status = 0, Message = "SeniorCare booking info saving Error.." });
        //    }

        //}

        [HttpPost, Route("AddEditSeniorCareBooking"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareBooking(OrgBookingUpdateViewModel model)
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
                                          new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (model.Description != null ? model.Description : "") },
                                          new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                          new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 }

                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "SeniorCare booking info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! SeniorCare Info Not Found! Should be select SeniorCare Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "SeniorCare booking info saving Error.." });
            }

        }

        //-- Delete SeniorCare Booking
        [Route("DeleteSeniorCareBooking/{id}")]
        [HttpPost]
        public JsonResult DeleteSeniorCareBooking(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spDocOrgBooking_Delete @SlotId, @ModifiedBy",
                    new SqlParameter("SlotId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare Booking info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"SeniorCare Booking info deleted Error.." });
            }
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
            //var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spOrgInsurancePlanListForSeniorCare_Get @OrganizationId, @InsuranceTypeId",
            //    new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = ReferenceId },
            //    new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
            //    );
            var planList = _repo.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownList_Get");

            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region SeniorCare Address

        // GET: StateLicense 
        public ActionResult Address(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["PharmacyData"] = "Yes";
                ViewBag.PharmacyID = id;
                //Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("SeniorCare"))
                {
                    //int userId = User.Identity.GetUserId<int>();
                    //var userInfo = _appUser.GetById(userId);
                    //string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1007 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    //TempData["PharmacyData"] = "Yes";
                    //ViewBag.seniorCareId = pharmacyInfo.First().OrganisationId;
                    //Session["PharmacyID"] = pharmacyInfo.First().OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetSeniorCareOfficeLocationList/{flag}/{id}")]
        public ActionResult GetSeniorCareOfficeLocationList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allSeniorCareAddress = _repo.ExecWithStoreProcedure<OrganisationAddressViewModel>("spAddress_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1007 },
                    new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                    new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                    new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                    );

            var result = allSeniorCareAddress
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

        public PartialViewResult SeniorCareAddress(int? id, int seniorCareId = 0)
        {
            ViewBag.seniorCareId = seniorCareId;

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
                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyAddress.cshtml", orgAddressInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgAddressInfo = new OrganisationAddressViewModel();
                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyAddress.cshtml", orgAddressInfo);
            }
        }




        [HttpPost, Route("AddEditSeniorCareAddress"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareAddress(OrganisationAddressViewModel model)
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

                return Json(new JsonResponse() { Status = 1, Message = "SeniorCare address info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "SeniorCare address info saving Error.." });
            }

        }

        [Route("DeleteSeniorCareAddress/{id}")]
        [HttpPost]
        public JsonResult DeleteSeniorCareAddress(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_Delete @AddressId, @ModifiedBy",
                    new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare Address has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"SeniorCare Address deleted Error.." });
            }


        }

        [Route("ActiveDeActiveSeniorCareAddress/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActiveSeniorCareAddress(bool flag, int id)
        {
            try
            {
                bool DelFlag = false;
                if (flag == false)
                    DelFlag = true;


                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spAddress_ActiveDeActive @AddressId, @IsActive, @IsDelete,@ModifiedBy",
                    new SqlParameter("AddressId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = flag },
                    new SqlParameter("IsDelete", System.Data.SqlDbType.Bit) { Value = DelFlag },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare address info has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"SeniorCare address info {(flag ? "reactivated" : "deleted")} Error.." });
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
                //Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("SeniorCare"))
                {
                    //int userId = User.Identity.GetUserId<int>();
                    //var userInfo = _appUser.GetById(userId);
                    //string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1007 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    //TempData["PharmacyData"] = "Yes";
                    //ViewBag.seniorCareId = pharmacyInfo.First().OrganisationId;
                    //Session["PharmacyID"] = pharmacyInfo.First().OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetSeniorCareOpeningHoursList/{flag}/{id}")]
        public ActionResult GetSeniorCareOpeningHoursList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var allPharmacy = _repo.ExecWithStoreProcedure<OrgOpeningHoursViewModel>("spOpeningHour_Get @Search, @OrganizationID, @OrganizationTypeID, @UserTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1007 },
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

        [HttpGet, Route("SeniorCare/GetOpeningHours/{id?}")]
        public ActionResult GetOpeningHours(int? id, JQueryDataTableParamModel param)
        {
            try
            {
                var allPharmacy = _facility.ExecWithStoreProcedure<OrgOpeningHoursViewModel>("spOpeningHour_Get @Search, @OrganizationID, @OrganizationTypeID, @UserTypeID, @PageIndex, @PageSize, @Sort",
                         new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                         new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = id },
                         new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                         new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1007 },
                         new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength)) + 1 : 1 },
                         new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayLength > 0 ? param.iDisplayLength : 10 },
                         new SqlParameter("Sort", System.Data.SqlDbType.NChar) { Value = param.sSortDir_0 != null ? param.sSortDir_0 : "Asc" }
                         ).ToList();

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
                        Comments = x.Comments,
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getWeekDayName(int day)
        {
            string DayName = "";

            if (day == 1)
                DayName = "Sunday";
            if (day == 2)
                DayName = "Monday";
            if (day == 3)
                DayName = "Tuesday";
            if (day == 4)
                DayName = "Wednesday";
            if (day == 5)
                DayName = "Thursday";
            if (day == 6)
                DayName = "Friday";
            if (day == 7)
                DayName = "Saturday";

            return DayName;
        }


        //---- Get Opening Hours Details

        public PartialViewResult SeniorCareOpeningHours(int? id, int? seniorCareId)
        {
            ViewBag.SeniorCareID = seniorCareId;

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



                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyOpeningHours.cshtml", orgInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgInfo = new OrgOpeningHoursUpdateViewModel();
                ViewBag.OrgName = null;


                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyOpeningHours.cshtml", orgInfo);
            }
        }


        //--- Update Pharmacy Opening Hours

        [HttpPost, Route("AddEditSeniorCareOpeningHours"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareOpeningHours(OrgOpeningHoursUpdateViewModel model)
        {
            try
            {

                if (model.OrganizationID > 0)
                {
                    for (int i = 0; i < 7; i++)
                    {

                        int ExecWithStoreProcedure = _facility.ExecuteSQLQuery("spOrgOpeningHour_Create " +
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
                                              new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = (object)false },
                                              new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                              new SqlParameter("Comments", System.Data.SqlDbType.NVarChar) { Value = (object)model.Comments[i] ?? DBNull.Value }
                                          );

                        continue;
                    }


                    return Json(new JsonResponse() { Status = 1, Message = "SeniorCare Opening Hours info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Error..! SeniorCare Info Not Found! Should be select SeniorCare Name." });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "SeniorCare OpeningHours info saving Error.." });
            }

        }


        //-- Delete SeniorCare Featured
        [Route("DeleteSeniorCareOpeningHours/{id}")]
        [HttpPost]
        public JsonResult DeleteSeniorCareOpeningHours(int id)
        {
            try
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrgOpeningHour_Delete @OpeningHourID, @ModifiedBy",
                    new SqlParameter("OpeningHourID", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("ModifiedBy", System.Data.SqlDbType.Bit) { Value = User.Identity.GetUserId<int>() }
                    );

                return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare opening hours info has been deleted successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"SeniorCare opening hours info deleted Error.." });
            }
        }


        #endregion

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


        #region Social Media 

        [Route("SeniorCare/SocialMedia")]
        public ActionResult SocialMedia()
        {
            return View();
        }
        [Route("SeniorCare/GetSocialMedia/{id?}")]
        public ActionResult GetSocialMedia(int id, JQueryDataTableParamModel param)
        {
            var socialMediaInfo = _facility.ExecWithStoreProcedure<OrgSocialMediaViewModel>("spSocialMedia_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                     new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                     new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                     new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                     new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = OrganisationTypes.SeniorCare },
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

        [HttpPost]
        public JsonResult UpdateSocialMedia(OrgSocialMediaListViewModel editSocialMedia)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Facebook", SqlDbType.VarChar) { Value = editSocialMedia.Facebook });
            parameters.Add(new SqlParameter("Twitter", SqlDbType.VarChar) { Value = editSocialMedia.Twitter });
            parameters.Add(new SqlParameter("LinkedIn", SqlDbType.VarChar) { Value = editSocialMedia.LinkedIn });
            parameters.Add(new SqlParameter("Instagram", SqlDbType.VarChar) { Value = editSocialMedia.Instagram });
            parameters.Add(new SqlParameter("Youtube", SqlDbType.VarChar) { Value = editSocialMedia.Youtube });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editSocialMedia.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("SocialMedia", "SocialMediaId", editSocialMedia.SocialMediaId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Social Media Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating social media", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateSocialMedia(OrgSocialMediaListViewModel createSocialMedia)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = createSocialMedia.OrganisationId });
            parameters.Add(new SqlParameter("Facebook", SqlDbType.VarChar) { Value = createSocialMedia.Facebook });
            parameters.Add(new SqlParameter("Twitter", SqlDbType.VarChar) { Value = createSocialMedia.Twitter });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createSocialMedia.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            parameters.Add(new SqlParameter("LinkedIn", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.LinkedIn });
            parameters.Add(new SqlParameter("Instagram", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.Instagram });
            parameters.Add(new SqlParameter("Youtube", System.Data.SqlDbType.VarChar) { Value = createSocialMedia.Youtube });
            parameters.Add(new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 });
            //parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            try
            {
                _facility.ExecuteSqlCommandForInsert("SocialMedia", parameters);

                return Json(new JsonResponse { Status = 1, Message = "Social Media Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating social media", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateSwitch(SwitchUpdateViewModel switchUpdateViewModel)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter(switchUpdateViewModel.FieldToUpdateName, SqlDbType.VarChar) { Value = switchUpdateViewModel.FieldToUpdateValue });
            try
            {
                _facility.ExecuteSqlCommandForUpdate(switchUpdateViewModel.TableName, switchUpdateViewModel.PrimaryKeyName, switchUpdateViewModel.PrimaryKeyValue, parameters);
                return Json(new JsonResponse { Status = 1, Message = switchUpdateViewModel.FieldToUpdateName + " Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating " + switchUpdateViewModel.FieldToUpdateName.ToLower(), Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region

        [Route("SeniorCare/InsurancePlan")]
        public ActionResult InsurancePlan()
        {
            return View();
        }

        [HttpGet, Route("SeniorCare/GetInsurancePlan/{id?}")]
        public ActionResult GetInsurancePlan(int id, JQueryDataTableParamModel param)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetInsurancePlanSearchResult(model, id);
            var allFacilitys = searchResult.InsurancePlanProviderList.AsEnumerable();

            Func<InsurancePlanProviderModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                            //: sortColumnIndex == 3 ? c.Email
                            //    : sortColumnIndex == 4 ? c.
                            : c.Name;
            allFacilitys = sortDirection == "asc" ? allFacilitys.OrderBy(orderingFunction).ToList() : allFacilitys.OrderByDescending(orderingFunction).ToList();

            //var display = allFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var total = searchResult.TotalRecordCount;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allFacilitys
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("SeniorCare/GetDrpInsuranceTypeList")]
        public ActionResult GetDrpInsuranceTypeList()
        {
            try
            {
                var insuranceTypeList = _facility.ExecWithStoreProcedure<DrpInsuranceType>("spInsuranceTypeDropDownList_Get").ToList();
                return Json(insuranceTypeList.OrderBy(m => m.Name).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("SeniorCare/GetDrpInsurancePlanByTypeId")]
        public JsonResult GetDrpInsurancePlanByTypeId(int TypeId)
        {
            var planList = _facility.ExecWithStoreProcedure<InsurancePlanDropDownViewModel>("spInsurancePlanDropDownListByType_Get @InsuranceTypeId",
                new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = TypeId }
                );


            return Json(planList.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("SeniorCare/GetDrpInsurancePlanList")]
        public ActionResult GetDrpInsurancePlanList(int insuranceTypeId, int referenceId)
        {
            try
            {
                var planList = _facility.ExecWithStoreProcedure<DrpInsurancePlan>("spOrgInsurancePlanDropDownList_Get @OrganizationId, @InsuranceTypeId",
                 new SqlParameter("OrganizationId", System.Data.SqlDbType.Int) { Value = referenceId },
                 new SqlParameter("InsuranceTypeId", System.Data.SqlDbType.Int) { Value = insuranceTypeId }
                 ).ToList();
                return Json(planList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("SeniorCare/GetDrpInsuranceProviderList")]
        public ActionResult GetDrpInsuranceProviderList()
        {
            try
            {
                var insuranceProviderList = _facility.SQLQuery<DrpInsuranceProvider>("select InsProviderId, InsCompanyName from InsuranceProviders").ToList();
                return Json(insuranceProviderList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Route("SeniorCare/GetDrpStateList")]
        public ActionResult GetDrpStateList()
        {
            try
            {
                var stateList = _facility.SQLQuery<DrpState>("select distinct State from CityStateZip Where Country = 'US' order by State asc").ToList();
                return Json(stateList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult UpdateInsurancePlan(InsurancePlanProviderModel editInsurancePlan)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = editInsurancePlan.InsurancePlanId });
            parameters.Add(new SqlParameter("InsuranceIdentifierId", SqlDbType.VarChar) { Value = editInsurancePlan.InsuranceIdentifierId });
            parameters.Add(new SqlParameter("StateId", SqlDbType.VarChar) { Value = editInsurancePlan.StateId });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = editInsurancePlan.IsActiveString == "on" ? 1 : 0 });
            try
            {
                _facility.ExecuteSqlCommandForUpdate("DocOrgInsurances", "DocOrgInsuranceId", editInsurancePlan.DocOrgInsuranceId, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Insurance Plan Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating insurance plan", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateInsurancePlan(InsurancePlanProviderModel createInsurancePlan)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ReferenceId", SqlDbType.Int) { Value = createInsurancePlan.ReferenceId });
            parameters.Add(new SqlParameter("InsurancePlanId", SqlDbType.Int) { Value = createInsurancePlan.InsurancePlanId });
            parameters.Add(new SqlParameter("InsuranceIdentifierId", SqlDbType.VarChar) { Value = createInsurancePlan.InsuranceIdentifierId });
            parameters.Add(new SqlParameter("StateId", SqlDbType.VarChar) { Value = createInsurancePlan.StateId });
            parameters.Add(new SqlParameter("IsActive", SqlDbType.Bit) { Value = createInsurancePlan.IsActiveString == "on" ? 1 : 0 });
            parameters.Add(new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = 0 });
            parameters.Add(new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() });
            //parameters.Add(new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now });
            parameters.Add(new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 5 });
            try
            {
                _facility.ExecuteSqlCommandForInsert("DocOrgInsurances", parameters);
                return Json(new JsonResponse { Status = 1, Message = "Insurance Plan Created Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in creating insurance plan", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }
        private InsurancePlanProviderListModel GetInsurancePlanSearchResult(SearchParamModel model, int organizationId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex + 1;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationId",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@Sort",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                };

                int RecordCount = 0;
                var allslotList = _facility.GetInsurancePlanByOrgId(StoredProcedureList.GetInsurancePlan, parameters, out RecordCount).ToList();
                var searchResult = new InsurancePlanProviderListModel() { InsurancePlanProviderList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecordCount = RecordCount;
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

        #region DocOrgTaxonomy

        // GET: Taxonomy 
        public ActionResult Taxonomy(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["SeniorCareData"] = "Yes";
                ViewBag.SeniorCareID = id;
                //Session["SeniorCareID"] = id;
            }
            else
            {
                if (User.IsInRole("SeniorCare"))
                {
                    //int userId = User.Identity.GetUserId<int>();
                    //var userInfo = _appUser.GetById(userId);
                    //string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    //TempData["PharmacyData"] = "Yes";
                    //ViewBag.PharmacyID = pharmacyInfo.First().OrganisationId;
                    //Session["PharmacyID"] = pharmacyInfo.First().OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }


        [Route("GetSeniorCareTaxonomyList/{flag}/{id}")]
        public ActionResult GetSeniorCareTaxonomyList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var Info = _repo.ExecWithStoreProcedure<OrganisationTaxonomyViewModel>("spDocOrgTaxonomy_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1007 },
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

        public PartialViewResult SeniorCareTaxonomy(int? id, int seniorCareId = 0)
        {
            ViewBag.SeniorCareID = seniorCareId;

            if (id > 0)
            {
                ViewBag.ID = id;

                var orgTaxonomyInfo = _repo.ExecWithStoreProcedure<OrganisationTaxonomyViewModel>("spDocOrgTaxonomySeniorCare_GetById @DocOrgTaxonomyID",
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


                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyTaxonomy.cshtml", orgTaxonomyInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgTaxonomyInfo = new OrganisationTaxonomyUpdateViewModel();
                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyTaxonomy.cshtml", orgTaxonomyInfo);
            }
        }


        //-- Add/Edit Taxonomy

        [HttpPost, Route("AddEditSeniorCareTaxonomy"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareTaxonomy(OrganisationTaxonomyUpdateViewModel model)
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
                                      new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 5 }
                                  );

                return Json(new JsonResponse() { Status = 1, Message = "SeniorCare Taxonomy info save successfully" });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "SeniorCare Taxonomy info saving Error.." });
            }

        }



        [Route("ActiveDeActiveSeniorCareTaxonomy/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActiveSeniorCareTaxonomy(bool flag, int id)
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

                return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare Taxonomy info has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"SeniorCare Taxonomy info {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }
        [HttpPost]
        public JsonResult GetTaxonomy(string Prefix)
        {
            var taxonomyList = _repo.ExecWithStoreProcedure<TaxonomyAutoCompleteDropDownViewModel>("spGetTaxonomyCodesAutoComplete @Taxonomy_Code",
                new SqlParameter("Taxonomy_Code", System.Data.SqlDbType.VarChar) { Value = Prefix }
                );


            return Json(taxonomyList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region SeniorCare State License
        // GET: StateLicense 
        public ActionResult StateLicense(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["SeniorCareData"] = "Yes";
                ViewBag.PharmacyID = id;
                //Session["PharmacyID"] = id;
            }
            else
            {
                if (User.IsInRole("SeniorCare"))
                {
                    //int userId = User.Identity.GetUserId<int>();
                    //var userInfo = _appUser.GetById(userId);
                    //string NPI = userInfo.Uniquekey;
                    //var pharmacyInfo = _repo.All<Organisation>().Where(x => x.OrganizationTypeID == 1005 && x.NPI == NPI && x.IsDeleted == false && x.UserId == userId);
                    //TempData["PharmacyData"] = "Yes";
                    //ViewBag.PharmacyID = pharmacyInfo.First().OrganisationId;
                    //Session["PharmacyID"] = pharmacyInfo.First().OrganisationId;
                }
                else
                {
                    return View("Error");
                }
            }

            return View();
        }

        [Route("GetSeniorCareStateLicenseList/{flag}/{id}")]
        public ActionResult GetSeniorCareStateLicenseList(bool flag, JQueryDataTableParamModel param, int? id)
        {
            var result = _repo.ExecWithStoreProcedure<OrgStateLicenseViewModel>("spDocOrgStateLicenses_Get @Search, @UserTypeID, @ReferenceId, @OrganizationTypeID, @PageIndex, @PageSize, @Sort",
                    new SqlParameter("Search", System.Data.SqlDbType.NChar) { Value = param.sSearch == null ? " " : param.sSearch },
                    new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = 5 },
                    new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = id },
                    new SqlParameter("OrganizationTypeID", System.Data.SqlDbType.Int) { Value = 1007 },
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

        public PartialViewResult SeniorCareStateLicense(int? id, int seniorCareId = 0)
        {
            ViewBag.SeniorCareID = seniorCareId;

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
                        OrganizatonTypeID = 1007,
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
                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyStateLicense.cshtml", orgLicenseInfo);
            }
            else
            {
                ViewBag.ID = 0;
                var orgLicenseInfo = new OrgStateLicenseUpdateViewModel();
                return PartialView(@"/Views/SeniorCare/Partial/_PharmacyStateLicense.cshtml", orgLicenseInfo);
            }
        }

        //-- Active DeActive SeniorCare State License
        [Route("ActiveDeActiveSeniorCareStateLicense/{flag}/{id}")]
        [HttpPost]
        public JsonResult ActiveDeActiveSeniorCareStateLicense(bool flag, int id)
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

                return Json(new JsonResponse() { Status = 1, Message = $@"The SeniorCare State License has been {(flag ? "reactivated" : "deleted")} successfully" });
            }
            catch
            {
                return Json(new JsonResponse() { Status = 0, Message = $@"SeniorCare State License {(flag ? "reactivated" : "deleted")} Error.." });
            }
        }

        //--Update State License

        [HttpPost, Route("AddEditSeniorCareStateLicense"), ValidateAntiForgeryToken]
        public JsonResult AddEditSeniorCareStateLicense(OrgStateLicenseUpdateViewModel model)
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
                                          new SqlParameter("UserTypeId", System.Data.SqlDbType.Int) { Value = 5 }
                                      );

                    return Json(new JsonResponse() { Status = 1, Message = "SeniorCare State License info save successfully" });
                }
                else
                {
                    return Json(new JsonResponse() { Status = 0, Message = "Invalid Fields!. Enter correct Organisation Name" });
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { Status = 0, Message = "SeniorCare State License info saving Error.." });
            }

        }

        #endregion

        [Route("SeniorCare/Profile/{userId}")]
        public ActionResult Profile(int userId)
        {
            var users = _facility.SQLQuery<DrpUser>("select * from AspNetUsers where Id = " + userId.ToString()).ToList();
            var user = new DrpUser();
            if (users.Count > 0)
                user = users.First();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSeniorCareUser(DrpUser user)
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

                _facility.ExecuteSqlCommandForUpdate("AspNetUsers", "Id", user.Id, parameters);
                return Json(new JsonResponse { Status = 1, Message = "Senior Care user updated successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating facility user" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
