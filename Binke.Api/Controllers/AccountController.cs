using Binke.Api.Models;
using Binke.Api.ViewModels;
using Doctyme.Model;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Data;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Binke.Api.Controllers
{

    public class AccountController : ApiController
    {
        #region 
        private ApplicationUserManager _userManager;
        private readonly IRepository _repo;
        private readonly IUserService _appUser;
        //private readonly IUserTypeService _userType;
        #endregion

        public AccountController(ApplicationUserManager userManager, IUserService appUser, IRepository repo)
        {
            _userManager = userManager;
            _appUser = appUser;
            _repo = repo;
            //_userType = userType;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Account/register")]

        public HttpResponseMessage Register(ApiRegisterViewModel model)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            list.Add("2", "Doctor");
            list.Add("4", "Facility");
            list.Add("12", "Patient");
            list.Add("3", "Pharmacy");
            list.Add("5", "SeniorCare");
			list.Add("6", "Admin");
			list.Add("1", "User");
			list.Add("9", "Staff");

			//var roleName = GetUserTypes().FirstOrDefault(x => x.UserTypeId.ToString() == model.GroupTypeId)?.UserTypeName ?? "";//Added by Reena
			using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (string.IsNullOrEmpty(model.UserType))
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Please Select User Type!" });
                    var roleName = list[model.UserType];

                    // Check ApiRegisterViewModel model validations are matched
                    string errorMessage = string.Empty;
                    if(!ValidateRegisterModel(model,roleName,out errorMessage))
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Status = HttpStatusCode.NotFound, Message = errorMessage });
                    }

                    //if (!roleName.Equals(UserRoles.Patient))
                    //{
                    //    bool isUnique = _appUser.GetSingle(x => x.Uniquekey == model.Uniquekey) != null;
                    //    if (isUnique)
                    //    {
                    //        txscope.Dispose();
                    //        return Json(new JsonResponse { Status = 0, Message = $@"{roleName} already exist.." }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //var userType =_userManager

                    var email = false;
                    if (!string.IsNullOrEmpty(model.Email))
                        email = _appUser.GetSingle(x => x.Email == model.Email) != null;
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Please Enter Email!" });
                    var NPI = false;
                    if (!string.IsNullOrEmpty(model.Uniquekey))
                        NPI = _appUser.GetAll().Where(x => x.Uniquekey == model.Uniquekey).Count() > 0;
                    if (email)
                    {
                        txscope.Dispose();
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Email Already Exists !" });
                    }
                    if (string.IsNullOrEmpty(model.PhoneNumber))
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Please Enter Phone Number!" });
                    if (_appUser.GetAll().Where(x => x.PhoneNumber == model.PhoneNumber).Count() > 0)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "PhoneNumber Already Exists !" });


                    while (_appUser.GetAll().Where(x => x.Uniquekey == model.Uniquekey).Count() > 0)
                    {
                        Random rand = new Random();
                        model.Uniquekey = Convert.ToString(rand.Next(11111, 99999)) + Convert.ToString(rand.Next(11111, 99999));

                    }
                    var user = new ApplicationUser
                    {
                        FirstName = model.FirstName == null ? "" : model.FirstName,
                        MiddleName = model.MiddleName == null ? "" : model.MiddleName,
                        LastName = model.LastName == null ? "" : model.LastName,
                        UserName = model.Email,
                        Email = model.Email,
                        ProfilePicture = StaticFilePath.ProfilePicture,
                        CreatedDate = DateTime.UtcNow,
                        LastResetPassword = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                        Uniquekey = model.Uniquekey,
                        UserTypeId = Convert.ToInt32(model.UserType),
                        RegisterViewModel = JsonConvert.SerializeObject(model),
                        EmailConfirmed = false,
                        PhoneNumber = model.PhoneNumber,
                        PhoneNumberConfirmed = model.PhoneNumberVerified
                    };
                    var result = _userManager.CreateAsync(user, model.Password);
                    result.Wait();
                    var xy = _userManager.AddToRoleAsync(user.Id, roleName.Replace(" ", ""));
                    xy.Wait();



                    var provider = new MachineKeyProtectionProvider();// new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Doctyme");
                    _userManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("EmailConfirmation"));


                    var codes = Task.Run(async () => await _userManager.GenerateEmailConfirmationTokenAsync(user.Id)); //Task.Run(()=> await );
                    string code = codes.Result;
                    if (!String.IsNullOrEmpty(code))
                    {
                        string query = "spAddEmailVerificationToken @UserId,@VerificationCode";
                        int emailVerificationId = _repo.ExecuteSQLQuery(query,
                             new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = user.Id > 0 ? user.Id : 0 },
                                      new SqlParameter("VerificationCode", System.Data.SqlDbType.NVarChar) { Value = code });
                    }
                    var callbackUrl = "https://www.doctyme.com/" + "Account/ConfirmEmail?userId=" + user.Id + "&code=" + HttpUtility.UrlEncode(code);
                    //var settingUrl = Url.Action("", "Home", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    string body = Binke.Api.Models.Common.ReadEmailTemplate(EmailTemplate.ConfirmEmail);
                    body = body.Replace("{UserName}", "")
                        .Replace("{action_url}", callbackUrl)
                        .Replace("{action_url_settings}", "")
                        .Replace("{action_url_help}", "")
                        .Replace("{action_url_notmyaccount}", "");

                    SendMail.SendEmail(user.Email, "", "", "", body, "Confirm your account");

                    var registration = _appUser.GetSingle(x => x.Id == user.Id);
                    //var data = JsonConvert.SerializeObject(user);
                    txscope.Complete();
                    /* string code = _userManager.GenerateChangePhoneNumberTokenAsync(user.Id);
                      var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                      //var settingUrl = Url.Action("", "Home", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    
                    
                      SendMail.SendEmail(user.Email, "", "", "", body, "Confirm your account");*/
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = $@"Account has been created Successfully", Data = user, UserId = user.Id });
                    // return Json(new JsonResponse { Status = 1, Message = "Please Check your email account for account confirmation.", Data = new { user.Id } }, JsonRequestBehavior.AllowGet);

                    //txscope.Dispose();
                    //return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {


                    //   Common.LogError(ex, "Register-POST");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = ex.InnerException.Message });

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Account/SignUppp")]
        public HttpResponseMessage SignUp(ApiRegisterViewModel model)
        {
            Random random = new Random();
            //    ApiRegisterViewModel model = new ApiRegisterViewModel()
            //    {
            //        Password = HttpContext.Current.Request.Params["Password"],
            //        Email = HttpContext.Current.Request.Params["Email"],
            //        PhoneNumber= HttpContext.Current.Request.Params["PhoneNumber"],
            //        FirstName= HttpContext.Current.Request.Params["FirstName"],
            //        MiddleName= HttpContext.Current.Request.Params["MiddleName"],
            //        LastName= HttpContext.Current.Request.Params["LastName"],
            //        UserType= HttpContext.Current.Request.Params["UserType"],
            //        Uniquekey= Convert.ToString(random.Next(11111, 99999)) + Convert.ToString(random.Next(11111, 99999))
            //};
            model.Uniquekey = Convert.ToString(random.Next(11111, 99999)) + Convert.ToString(random.Next(11111, 99999));
            //var userTypeList = GetUserTypes();
            Dictionary<string, string> list = new Dictionary<string, string>();
            ////foreach (var item in userTypeList)
            ////{
            ////    list.Add(item.UserTypeId.ToString(), item.UserTypeName);
            ////}

            list.Add("2", "Doctor");
            list.Add("4", "Facility");
            list.Add("12", "Patient");
            list.Add("3", "Pharmacy");
            list.Add("5", "SeniorCare");



            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (string.IsNullOrEmpty(model.UserType))
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Please Select User Type!" });
                    var roleName = list[model.UserType];

                    //if (!roleName.Equals(UserRoles.Patient))
                    //{
                    //    bool isUnique = _appUser.GetSingle(x => x.Uniquekey == model.Uniquekey) != null;
                    //    if (isUnique)
                    //    {
                    //        txscope.Dispose();
                    //        return Json(new JsonResponse { Status = 0, Message = $@"{roleName} already exist.." }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //var userType =_userManager

                    var email = false;
                    if (!string.IsNullOrEmpty(model.Email))
                        email = _appUser.GetSingle(x => x.Email == model.Email) != null;
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Please Enter Email!" });
                    /* var NPI = false;
                     if (!string.IsNullOrEmpty(model.Uniquekey))
                         NPI = _appUser.GetAll().Where(x => x.Uniquekey == model.Uniquekey).Count() > 0;*/
                    if (email)
                    {
                        txscope.Dispose();
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "UserName Already Exists !" });
                    }
                    if (string.IsNullOrEmpty(model.PhoneNumber))
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "Please Enter Phone Number!" });
                    if (_appUser.GetAll().Where(x => x.PhoneNumber == model.PhoneNumber).Count() > 0)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = "PhoneNumber Already Exists !" });


                    while (_appUser.GetAll().Where(x => x.Uniquekey == model.Uniquekey).Count() > 0)
                    {
                        Random rand = new Random();
                        model.Uniquekey = Convert.ToString(rand.Next(11111, 99999)) + Convert.ToString(rand.Next(11111, 99999));

                    }
                    var user = new ApplicationUser
                    {
                        FirstName = model.FirstName == null ? "" : model.FirstName,
                        MiddleName = model.MiddleName == null ? "" : model.MiddleName,
                        LastName = model.LastName == null ? "" : model.LastName,
                        UserName = model.Email,
                        Email = model.Email,
                        ProfilePicture = StaticFilePath.ProfilePicture,
                        CreatedDate = DateTime.UtcNow,
                        LastResetPassword = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                        Uniquekey = model.Uniquekey,
                        UserTypeId = Convert.ToInt32(model.UserType),
                        RegisterViewModel = JsonConvert.SerializeObject(model),
                        EmailConfirmed = false,
                        PhoneNumber = model.PhoneNumber,
                        PhoneNumberConfirmed = model.PhoneNumberVerified
                    };
                    var result = _userManager.CreateAsync(user, model.Password);
                    result.Wait();
                    var xy = _userManager.AddToRoleAsync(user.Id, roleName.Replace(" ", ""));
                    xy.Wait();



                    var provider = new MachineKeyProtectionProvider();// new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Doctyme");
                    _userManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("EmailConfirmation"));


                    var codes = Task.Run(async () => await _userManager.GenerateEmailConfirmationTokenAsync(user.Id)); //Task.Run(()=> await );
                    string code = codes.Result;
                    var callbackUrl = "https://www.doctyme.com/" + "Account/ConfirmEmail?userId=" + user.Id + "&code=" + code;
                    //var settingUrl = Url.Action("", "Home", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    string body = Binke.Api.Models.Common.ReadEmailTemplate(EmailTemplate.ConfirmEmail);
                    body = body.Replace("{UserName}", "")
                        .Replace("{action_url}", callbackUrl)
                        .Replace("{action_url_settings}", "")
                        .Replace("{action_url_help}", "")
                        .Replace("{action_url_notmyaccount}", "");

                    SendMail.SendEmail(user.Email, "", "", "", body, "Confirm your account");

                    var registration = _appUser.GetSingle(x => x.Id == user.Id);
                    //var data = JsonConvert.SerializeObject(user);
                    txscope.Complete();
                    /* string code = _userManager.GenerateChangePhoneNumberTokenAsync(user.Id);
                      var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                      //var settingUrl = Url.Action("", "Home", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    
                    
                      SendMail.SendEmail(user.Email, "", "", "", body, "Confirm your account");*/
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = $@"Account has been created Successfully", Data = user, UserId = user.Id });
                    // return Json(new JsonResponse { Status = 1, Message = "Please Check your email account for account confirmation.", Data = new { user.Id } }, JsonRequestBehavior.AllowGet);

                    //txscope.Dispose();
                    //return Json(new JsonResponse { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {


                    //   Common.LogError(ex, "Register-POST");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = ex.Message });

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet, Route("api/Account/GetLoginUserDetails")]
        public HttpResponseMessage GetLoginUserDetails(string email)
        {
            //var userDetails = _appUser.GetSingle(x => x.Email == email);
            //return Request.CreateResponse(HttpStatusCode.OK, userDetails);
            var DocPara = new List<SqlParameter>();
            DocPara.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });          
            var userDetails = _appUser.GetUserAddressCityStateZip(StoredProcedureList.sp_GetUserAddressCityStateZip, DocPara);
            return Request.CreateResponse(HttpStatusCode.OK, userDetails);
        }

        [HttpPost, Route("api/Account/Signin")]
        public HttpResponseMessage Login(ApiLoginViewModel model)
        {
            try
            {
                var email = false;
                if (model.Login_Method == 1)
                {


                    if (!string.IsNullOrEmpty(model.UserName))
                    {
                        email = _appUser.GetAll().Where(x => x.Email == model.UserName).Count() > 0;
                        if (email)
                        {
                            var user = _appUser.GetAll().Where(x => x.Email == model.UserName).ToList();
                            var password = user.Where(x => JsonConvert.DeserializeObject<ApiRegisterViewModel>(x.RegisterViewModel).Password == model.Password).Count() > 0;
                            if (password)
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Login Successful !" });
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Incorrect Password !" });
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Incorrect Email !" });
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Email Cannot be Empty !" });
                    }
                }
                if (model.Login_Method == 2)
                {
                    var phn = false;
                    if (!string.IsNullOrEmpty(model.UserName))
                        phn = _appUser.GetAll().Where(x => x.PhoneNumber == model.UserName).Count() > 0;
                    if (phn)
                    {
                        var user = _appUser.GetAll().Where(x => x.PhoneNumber == model.PhoneNumber).ToList();
                        // var password = user.Where(x => JsonConvert.DeserializeObject<ApiRegisterViewModel>(x.RegisterViewModel).Password == model.Password).Count() > 0;
                        if (phn)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = "Login Successful !" });
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.BadRequest, Message = "PhoneNumber Does Not Exists !", IsNewUser = true });
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.BadRequest, Message = "PhoneNumber Does Not Exists !", IsNewUser = true });
                    }

                }

                if (string.IsNullOrEmpty(model.UserName))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.BadRequest, Message = "Please Enter UserName !", IsNewUser = true });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.BadRequest, Message = "Please Enter Correct Login Method !" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = ex.Message });
            }
        }

        //private List<UserType> GetUserTypes()
        //{
        //    var allUserType = _userType.GetAll(x => !x.IsDeleted).ToList();
        //    return allUserType;
        //}

        #region ForgetPassword
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Account/ForgotPassword")]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] ForgetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = $@"Please enter validate Email" });

                var user = await _userManager.FindByNameAsync(model.Email);

				if (user == null)
				{
					// Don't reveal that the user does not exist or is not confirmed
					return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = $@"No user with this email exist, please try again" });
				}
				var userresult = await _userManager.IsEmailConfirmedAsync(user.Id).ConfigureAwait(false);
                if (!(userresult))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = $@"No user with this email exist, please try again" });
                }
                string DataEncryptionKey = ConfigurationManager.AppSettings["DataEncryptionKey"];

                var provider = new DpapiDataProtectionProvider(DataEncryptionKey);

                _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("EmailConfirmation"));
              
                var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id).ConfigureAwait(false);
                // return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.InternalServerError, Message = $@"sample.", Data = user, UserId = user.Id,Enccode= code });
                bool IsSuccess = await Binke.Api.Models.Common.ForgotPassword(user, code).ConfigureAwait(false);

                if (IsSuccess)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = $@"Please check your email for new password instructions.", Data = user, UserId = user.Id });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.InternalServerError, Message = $@"Email not able to send at this moment.", Data = user, UserId = user.Id });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.InternalServerError, Message = ex.InnerException.Message });
                throw;
            }
        }

        [HttpPost, Route("api/Account/ResetPassword")]
        public async Task<HttpResponseMessage> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = $@"Please enter Required Filed" });
                }
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = $@"No user with this email exist, please try again" });
                }
                if (model.ConfirmPassword != model.Password)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Status = HttpStatusCode.NotFound, Message = $@"Confirm password and Password doesnot match" });
                }

                string Binke = ConfigurationManager.AppSettings["CompanyName"];
                var provider = new DpapiDataProtectionProvider(Binke);
                _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("EmailConfirmation"));
                var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = $@"Password change sucessfully." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Message = $@"Password is not changed." });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.NotFound, Message = ex.InnerException.Message });
                throw;
            }
        }


        private bool ValidateRegisterModel(ApiRegisterViewModel apiRegisterViewModel,string roleName,out string message)
        {
            if (apiRegisterViewModel != null)
            {
                message = "Invalid or missing information. Please enter ";
                bool successRequiredInputs = true;
                if (string.IsNullOrEmpty(apiRegisterViewModel.FirstName))
                {
                    message = message + "First name, ";
                    successRequiredInputs = false;
                }
                if (string.IsNullOrEmpty(apiRegisterViewModel.LastName))
                {
                    message = message + "Last name, ";
                    successRequiredInputs = false;
                }
                if (string.IsNullOrEmpty(apiRegisterViewModel.Email))
                {
                    message = message + "Email, ";
                    successRequiredInputs = false;
                }
                if(roleName.ToLower()== "Facility".ToLower())
                {
                    if (apiRegisterViewModel.FacilityTypeId == 0)
                    {
                        message = message + "Facility, ";
                        successRequiredInputs = false;
                    }
                }
                if (roleName.ToLower() != "Patient".ToLower())
                {
                    if (string.IsNullOrEmpty(apiRegisterViewModel.Uniquekey))
                    {
                        message = message + "NPI, ";
                        successRequiredInputs = false;
                    }
                }
                if (string.IsNullOrEmpty(apiRegisterViewModel.Password))
                {
                    message = message + "Password, ";
                    successRequiredInputs = false;
                }
                if (string.IsNullOrEmpty(apiRegisterViewModel.ConfirmPassword))
                {
                    message = message + "Confirm password ";
                    successRequiredInputs = false;
                }

                // Check Regular Expression Data
                if (successRequiredInputs)
                {
                    message = "Invalid or missing informations. Please enter valid input : ";
                    // Length Validation 
                    if (roleName.ToLower() != "Patient".ToLower())
                    {
                        if (apiRegisterViewModel.Uniquekey.Length < 10)
                        {
                            message = message + "NPI should be 10 digit, ";
                            successRequiredInputs = false;
                        } 
                    }
                    if (apiRegisterViewModel.Password.Length < 8)
                    {
                        message = message + "The Password must be at least 8 characters long, ";
                        successRequiredInputs = false;
                    }
                    if (successRequiredInputs)
                    {
                        if (roleName.ToLower() != "Patient".ToLower())
                        {

                            Regex rgx = new Regex(@"^[0-9]+$");
                            if (!rgx.IsMatch(apiRegisterViewModel.Uniquekey))
                            {
                                message = message + "NPI accept only numbers";
                                return false;
                            }
                        }
                        Regex passwordRgx = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,150}$");
                        if (!passwordRgx.IsMatch(apiRegisterViewModel.Password))
                        {
                            message = message + "Passwords must contain at least one lowercase letter, one uppercase letter, one number and one special character";
                            return false;
                        }

                        if (!apiRegisterViewModel.Password.ToLower().Equals(apiRegisterViewModel.ConfirmPassword.ToLower()))
                        {
                            message = message + "The password and confirmation password do not match";
                            return false;
                        }
                    }
                    else
                    {
                        message = message.Substring(0, message.Length - 1);
                        return false;
                    }
                }
                else
                {
                    message = message.Substring(0, message.Length - 1);
                    return false;
                }

                // Returns true if all the inputs are valid
                message = string.Empty;
                return true;
                
            }
            message = "Invalid or missing information : invalid request body";
            return false;
        }


        #endregion

    }
}
