using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Binke.Models;
using Doctyme.Repository.Interface;
using Doctyme.Model;
using System.Collections.Generic;
using System;
using Doctyme.Repository.Enumerable;
using System.Transactions;
using Binke.App_Helpers;
using Newtonsoft.Json;
using Binke.Utility;
using System.Data;
using System.Data.SqlClient;

namespace Binke.Controllers
{

    public class AccountController : Controller
    {
        // TODO: This should be moved to the constructor of the controller in combination with a DependencyResolver setup
        // NOTE: You can use NuGet to find a strategy for the various IoC packages out there (i.e. StructureMap.MVC5)
        //private readonly UserManager _manager = _userManager.Create();

        #region 
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IDoctorService _doctor;
        private readonly IFacilityService _facility;
        private readonly IFacilityTypeService _facilityType;
        private readonly IPatientService _patient;
        private readonly IPharmacyService _pharmacy;
        private readonly ISeniorCareService _seniorCare;
        private readonly IUserService _appUser;
        private readonly IAdvertisementService _advService;//Added by Reena
        private readonly IUserTypeService _usertype;//Added by Reena
        #endregion

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager,
            IAuthenticationManager authenticationManager, IDoctorService doctor, IFacilityService facility, IFacilityTypeService facilityType, IPatientService patient,
            IPharmacyService pharmacy, IUserService appUser, ISeniorCareService seniorCare, IAdvertisementService advService, IUserTypeService userType)//Added by Reena - advService, userType
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _authenticationManager = authenticationManager;
            _doctor = doctor;
            _facility = facility;
            _facilityType = facilityType;
            _patient = patient;
            _pharmacy = pharmacy;
            _appUser = appUser;
            _seniorCare = seniorCare;
            _advService = advService;//Added by Reena
            _usertype = userType;//Added by Reena
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.msgForgotPassword = TempData["msgForgotPassword"];

                if (TempData["userId"] != null)
                {

                    var user = _userManager.FindById(Convert.ToInt32(TempData["userId"].ToString()));
                   
                     ViewBag.Email = user.Email;
                    ViewBag.Password = TempData["Password"];
                }
                

                var lat1 = GetLocationbyIP().Item1;//Added by Reena
                var long1 = GetLocationbyIP().Item2;
                var adList = _advService.ExecWithStoreProcedure<ViewModels.AdvertisementListViewModel>("spAdminAdvertisementList_Get").ToList();
                //foreach (var item in adList)
                //{
                //    item.DistanceCount = _advService.GetDistanceInMile(item.Lat, item.Long, lat1, long1);
                //}
                List<string> list1 = new List<string>();
                list1 = adList.Select(x => x.ImagePath).ToList();
                if (list1 != null && list1.Count > 0)
                {
                    ViewBag.AdvList = list1;
                }
                else
                {
                    list1.Add("loginad1.jpg");
                    list1.Add("loginad3.jpg");
                    ViewBag.AdvList = list1;
                }
                return View();
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Login-GET");
                return View();
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //var temp = new ApplicationUser
                //{
                //    FirstName = "Shivam",
                //    MiddleName = String.Empty,
                //    LastName = "Agarwal",
                //    UserName = "shvm.grwl@gmail.com",
                //    Email = "shvm.grwl@gmail.com",
                //    ProfilePicture = StaticFilePath.ProfilePicture,
                //    CreatedDate = DateTime.UtcNow,
                //    LastResetPassword = DateTime.UtcNow,
                //    IsActive = true,
                //    IsDeleted = false,
                //    Uniquekey = "4526376456",
                //    RegisterViewModel = JsonConvert.SerializeObject(model)
                //};

                //var x = await _userManager.CreateAsync(temp, "Admin@123");
                // model.Email = model.Email.Trim();
                var user = await _userManager.FindAsync(model.Email, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", @"Invalid username and/or password.");
                    return View(model);
                }
                var rolesList = GetUserRoles(user.Id);
                if (!rolesList[0].Equals(UserRoles.Admin))
                {
                }
                else
                {
                    user.HeaderName = $@"<h3>Admin Panel</h3>";
                }
                if (user.EmailConfirmed)
                {
                    if (user.IsActive)
                    {
                        user.LoginHeader = $@"{user.FirstName.FirstCharToUpper()} {user.LastName.FirstCharToUpper()} logged in at {DateTime.UtcNow.UtcToUserTime():HH:mm tt, dd-MMM-yyyy} on IP {RequestHelpers.GetClientIpAddress()}";

                        // This doesn't count login failures towards account lockout
                        // To enable password failures to trigger account lockout, change to shouldLockout: true
                        if (model.RememberMe) user.IsRemember = "T"; else user.IsRemember = "F";
                        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                        switch (result)
                        {
                            case SignInStatus.Success:                                
                                user.LastLogin = DateTime.UtcNow;
                                await _userManager.UpdateAsync(user);
                                if (string.IsNullOrEmpty(rolesList[0]))
                                    return RedirectToLocal(returnUrl);
                                else
                                {
                                    switch (rolesList[0])
                                    {
                                        case UserRoles.Admin:
                                            return RedirectToAction("Index", "Dashboard");
                                        case UserRoles.Doctor:
                                            return RedirectToAction("Doctor", "Dashboard");
                                        case UserRoles.Facility:
                                            return RedirectToAction("Facility", "Dashboard");
                                        case UserRoles.Pharmacy:
                                            return RedirectToAction("Pharmacy", "Dashboard");
                                        case UserRoles.Patient:
                                            if (returnUrl != null)
                                                if (returnUrl.Contains("RefillPrescription"))
                                                    return RedirectToLocal(returnUrl);
                                            return RedirectToAction("Patient", "Dashboard");
                                        case UserRoles.SeniorCare:
                                            return RedirectToAction("SeniorCare", "Dashboard");
                                    }
                                    return RedirectToLocal(returnUrl);
                                }

                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Invalid login attempt.");
                                return View(model);
                        }
                    }
                    ModelState.AddModelError("", @"Your Account Is Not Active.");
                }
                ModelState.AddModelError("", @"Please check your email account to confirm your email address.");
                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Login-POST");
                return View(new LoginViewModel());
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await _signInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        public ActionResult Register()
        {
            ViewBag.IsLoginShow = true;

            return View();
        }
        //

        // GET: /Account/Register
        [AllowAnonymous]
        [HttpGet]
        public ActionResult EditUserInfo(int id)
        {
            var userRole = User.Identity.GetClaimValue(UserClaims.UserRole);
            var userinfo = _appUser.GetById(id);
            return View(userinfo);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult EditUserInfo(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                UserName = model.Email,
                ProfilePicture = StaticFilePath.ProfilePicture,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,

            };
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult RegisterPartial(string userType)
        {
            try
            {
                if (userType == "SeniorCare")
                    userType = "Senior Care";
                var model = new RegisterViewModel
                {
                    UserType = userType
                };
                if (userType == UserRoles.Facility)
                {
                    ViewBag.FacilityTypeList = _facilityType.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem
                    {
                        Value = x.OrganizationTypeID.ToString(),
                        Text = x.Org_Type_Name,
                    }).ToList();
                }
                //model.GroupTypeId = Common.GetUserTypes().FirstOrDefault(x => x.Text.ToLower().Equals(userType.ToLower()))?.Value ?? "";
                model.GroupTypeId = GetUserTypes().FirstOrDefault(x => x.UserTypeName.ToLower().Equals(userType.ToLower()))?.UserTypeId.ToString() ?? "";//Added by Reena
                return PartialView("Partial/_RegisterPartial", model);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Account/RegisterPartial");
                return PartialView("Partial/_RegisterPartial", new RegisterViewModel());
            }
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Random random = new Random();
                    //var roleName = Common.GetUserTypes().FirstOrDefault(x => x.Value == model.GroupTypeId)?.Text ?? "";
                    var roleName = GetUserTypes().FirstOrDefault(x => x.UserTypeId.ToString() == model.GroupTypeId)?.UserTypeName ?? "";//Added by Reena
                    //if (model.Uniquekey == null && model.GroupTypeId == "6")
                    if (model.Uniquekey == null && roleName.Equals(UserRoles.Patient))//Added by Reena
                    {
                        model.Uniquekey = Convert.ToString(random.Next(11111, 99999)) + Convert.ToString(random.Next(11111, 99999));
                        while (_appUser.GetAll().Where(x => x.Uniquekey == model.Uniquekey).Count() > 0)
                        {
                            Random rand = new Random();
                            model.Uniquekey = Convert.ToString(rand.Next(11111, 99999)) + Convert.ToString(rand.Next(11111, 99999));
                        }

                    }
                    if (!roleName.Equals(UserRoles.Patient))
                    {
                        bool isUnique = _appUser.GetSingle(x => x.Uniquekey == model.Uniquekey) != null;
                        if (isUnique)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse { Status = 0, Message = $@"{roleName} NPI already exist.." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    //var userType =_userManager
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
                        Uniquekey = model.Uniquekey,
                        UserTypeId = Convert.ToInt32(model.GroupTypeId),
                        RegisterViewModel = JsonConvert.SerializeObject(model)
                    };

                    var isExist = await _userManager.FindByEmailAsync(user.Email);
                    if (isExist != null)
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse { Status = 0, Message = "Email already exist.." }, JsonRequestBehavior.AllowGet);
                    }

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    await _userManager.AddToRoleAsync(user.Id, roleName.Replace(" ", ""));
                    txscope.Complete();
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code,Password= model.Password }, protocol: Request.Url.Scheme);
                    //var settingUrl = Url.Action("", "Home", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    string body = Common.ReadEmailTemplate(EmailTemplate.ConfirmEmail);
                    body = body.Replace("{UserName}", "")
                        .Replace("{action_url}", callbackUrl)
                        .Replace("{action_url_settings}", "")
                        .Replace("{action_url_help}", "")
                        .Replace("{action_url_notmyaccount}", "https://www.doctyme.com/Account/Login/Account/Login");

                    SendMail.SendEmail(user.Email, "", "", "", body, "Confirm your account");

                    return Json(new JsonResponse { Status = 1, Message = "Please Check your email account for account confirmation.", Data = new { user.Id } }, JsonRequestBehavior.AllowGet);

                    //txscope.Dispose();
                    //return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "Register-POST");
                    return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
        }


        [HttpGet]
        public JsonResult GetBanner()
        {
            int intStatus = 0;
            string strMsg = "";
            var AddList = new List<Dictionary<string, object>>();
            try
            {
                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetLoginBanner);
                AddList = Common.ConvertToList(ds.Tables[0]);
                if (AddList != null && AddList.Count() > 0)
                {
                    intStatus = 1;
                    strMsg = "Success";
                }
                else
                {
                        intStatus = 0;
                        strMsg = "Data not found!";
                    
                }
                
            }
            catch (Exception ex)
            {
                intStatus = -1;
                strMsg = ex.Message + ":" + ex.StackTrace;
                Common.LogError(ex, "GetBanner");
            }

            return Json(new { JsonStatus = intStatus, JsonMessage = strMsg, JsonResponse = AddList }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code,string Password)
        {
            try
            {
                if (userId == 0 || code == null)
                {
                    return View("Error");
                }
               // code = new UrlHelper().Encode(code);
                var result = await _userManager.ConfirmEmailAsync(userId, code);
                if (!result.Succeeded)
                {
                    bool IsEmailVerified = _usertype.ExecuteSQLQueryWithOutParam("spVerifyEmailLinkForAPI @UserId,@Code",
                        new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userId },
                    new SqlParameter("Code", System.Data.SqlDbType.NVarChar) { Value = code }
                        );

                    if(!IsEmailVerified)
                    {
                        return View("Error");
                    }
                }
                
                var user = await _userManager.FindByIdAsync(userId);
                 var roleName = GetUserRoles(userId).FirstOrDefault();
                CreateUserBaseOnType(user, roleName);

                TempData["userId"] = userId;
                TempData["Password"] = Password;

                var jsonModels = new List<Common.JsonModel> { new Common.JsonModel(Common.JsonType.Success, @"Your account hass been confirm successfully. Please fill up your basic information first.") };
                this.AddJsons(jsonModels);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ConfirmEmail-Account");
            }

            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            var adList = _advService.ExecWithStoreProcedure<ViewModels.AdvertisementListViewModel>("spAdminAdvertisementList_Get").ToList();
            List<string> list1 = new List<string>();
            list1 = adList.Select(x => x.ImagePath).ToList();
            if (list1 != null && list1.Count > 0)
            {
                ViewBag.AdvList = list1;
            }
            else
            {
                list1.Add("loginad1.jpg");
                list1.Add("loginad3.jpg");
                ViewBag.AdvList = list1;
            }
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ModelState.AddModelError("", @"No user with this email exist, please try again");
                    return View("ForgotPassword");
                }
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                if (Request.Url == null) return RedirectToAction("ForgotPassword", "Account");

                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                var body = Common.ReadEmailTemplate("ForgetPassword.Html");
                body = body.Replace("{UserName}", $@"{user.FullName}")
                    .Replace("{action_url}", callbackUrl);
                await SendMail.SendEmailAsync(user.Email, "", "", "", body, "Reset Password");

                var jsonModels = new List<Common.JsonModel> { new Common.JsonModel(Common.JsonType.Success, @"Please check your email for new password instructions.") };
                this.AddJsons(jsonModels);

                TempData["msgForgotPassword"] = "Please check your email for new password instructions.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Common.LogError(ex, "ForgotPassword-Post");
                return View(model);
            }
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    var jsonModels = new List<Common.JsonModel> { new Common.JsonModel(Common.JsonType.Error, "user does not exist.") };
                    this.AddJsons(jsonModels);
                    return View(model);
                }
                var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    var jsonModels = new List<Common.JsonModel> { new Common.JsonModel(Common.JsonType.Success, @"Password Reset successfully.") };
                    this.AddJsons(jsonModels);

                    return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
                return View();
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ResetPassword-Post");
                return View();
            }
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            try
            {
                var userId = await _signInManager.GetVerifiedUserIdAsync();
                if (userId == 0)
                {
                    return View("Error");
                }
                var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(userId);
                var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
                return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "SendCode/Account");
                return View(new SendCodeViewModel());
            }
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                // Generate the token and send it
                if (!await _signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
                {
                    return View("Error");
                }
                return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "SendCode/Account-POST");
                return View();
            }
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            try
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    return RedirectToAction("Login");
                }

                // Sign in the user with this external login provider if the user already has a login
                var result = await _signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                    case SignInStatus.Failure:
                    default:
                        // If the user does not have an account, then prompt the user to create an account
                        ViewBag.ReturnUrl = returnUrl;
                        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
                }
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ExternalLoginCallback/Account");
                return RedirectToAction("Login");
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Manage");
                }

                if (ModelState.IsValid)
                {
                    // Get the information about the user from the external login provider
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "ExternalLoginConfirmation/Account");
                return View("ExternalLoginFailure");
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("LogOff")]
        public ActionResult LogOff()
        {
            EnsureLoggedOut();
            return RedirectToAction("Index", "Home");
        }

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            Session.Clear();
            Session.Abandon();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            CookieHelper.DeleteCookie(CookieKey.LoggedInUserId);
            //CookieHelper.DeleteAllCookies();
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }


        #region Controller Common
        private void CreateUserBaseOnType(ApplicationUser user, string userRole)
        {
            var regModel = JsonConvert.DeserializeObject<RegisterViewModel>(user.RegisterViewModel);
            switch (userRole)
            {
                case UserRoles.Doctor:
                    var doctor = _doctor.GetByUserId(user.Id);
                    if (doctor == null)
                    {
                        doctor = new Doctor
                        {
                            UserId = user.Id,
                            CreatedBy = user.Id,
                            NPI = regModel.Uniquekey,
                            IsActive = true,
                            FirstName = regModel.FirstName,
                            LastName = regModel.LastName,
                            MiddleName = regModel.MiddleName,
                            Name = regModel.FirstName + " " + regModel.MiddleName + " " + regModel.LastName,
                            CreatedDate = DateTime.Now,
                            Status = " "


                        };

                        _doctor.InsertData(doctor);
                        _doctor.SaveData();
                    }
                    break;
                case UserRoles.Facility:
                    var facility = _facility.GetByUserId(user.Id);
                    if (facility == null)
                    {
                        facility = new Organisation()
                        {
                            UserId = user.Id,
                            CreatedBy = user.Id,
                            //Changes made against Issue#25
                            NPI = regModel.Uniquekey,
                            IsActive = true,
                            OrganizationTypeID = OrganisationTypes.Facility,
                            OrganisationName = " ",
                            Status = "",
                            CreatedDate = DateTime.Now
                        };

                        _facility.InsertData(facility);
                        _facility.SaveData();
                    }

                    break;
                case UserRoles.Patient:
                    var newPatient = new Patient
                    {
                        UserId = user.Id,
                        CreatedBy = user.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };
                    _patient.InsertData(newPatient);
                    _patient.SaveData();
                    break;
                case UserRoles.Pharmacy:
                    var newPharmacy = new Organisation
                    {
                        UserId = user.Id,
                        CreatedBy = user.Id,
                        IsActive = true,
                        OrganizationTypeID = 1005,
                        OrganisationName = " ",
                        Status = "",
                        CreatedDate = DateTime.Now
                    };
                    _pharmacy.InsertData(newPharmacy);
                    _pharmacy.SaveData();
                    break;
                case UserRoles.SeniorCare:
                    var newSeniorCare = new Organisation
                    {
                        UserId = user.Id,
                        CreatedBy = user.Id,
                        IsActive = true,
                        OrganizationTypeID = 1007,
                        OrganisationName = " ",
                        Status = "",
                        CreatedDate = DateTime.Now
                    };
                    _seniorCare.InsertData(newSeniorCare);
                    _seniorCare.SaveData();
                    break;
            }
        }

        private IList<string> GetUserRoles(int userId)
        {
            var rolesList = Task.Run(() => _userManager.GetRolesAsync(userId)).Result;
            return rolesList;
        }

        /// <summary>
        /// Check NPI Number Exist in Doctor
        /// </summary>
        /// <param name="Uniquekey"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckNPINumberExist(string npi, string userRole)
        {
            //bool isNPINumberExist = _appUser.GetSingle(x => x.Uniquekey.Equals(Uniquekey)) != null;
            //return isNPINumberExist;
            bool IsNPIExist = false;
            try
            {
                switch (userRole)
                {
                    case UserRoles.Doctor:
                        IsNPIExist = _doctor.GetSingle(x => x.NPI == npi) != null;
                        break;
                    case UserRoles.Facility:
                    case UserRoles.Pharmacy:
                    case UserRoles.SeniorCare:
                        IsNPIExist = _appUser.GetSingle(x => x.Uniquekey == npi) != null;
                        break;
                }
                return Json(new JsonResponse { Status = (IsNPIExist ? 0 : 1), Message = "Npi already exists in database." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "CheckNPINumberExist/Account");
                return Json(new JsonResponse { Status = 0, Message = "Eception" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

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

        private List<UserType> GetUserTypes()//Added by Reena
        {
            var allUserType = _usertype.GetAll(x => !x.IsDeleted).ToList();
            return allUserType;
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpGet, Route("LoginPopUp")]
        public PartialViewResult LoginPopUp(int id, string selectedDate)
        {
            return PartialView(@"Partial/_Login");
        }
    }
}
