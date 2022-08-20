using System;
using System.Linq;
using System.Web.Mvc;
using Binke.Models;
//using Binke.Repository.Enumerable;
//using Binke.Repository.Interface;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Binke.Utility;
using Binke.ViewModels;
using Newtonsoft.Json;
using Doctyme.Repository;
using Binke.App_Helpers;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json.Linq;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Binke.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICityService _city;
        private readonly IDoctorService _doctor;
        private readonly IFacilityService _facility;
        private readonly IPharmacyService _pharmacy;
        private readonly ISeniorCareService _seniorCare;
        private readonly IDoctorFacilityAffiliationService _doctorFacility;
        private readonly IFeaturedService _featured;
        private readonly IFeaturedDoctorService _featuredDoctor;
        private readonly IFeaturedSpecialityService _featuredSpeciality;
        private readonly ISpecialityService _speciality;
        private readonly INewsletterSubscriberService _newsletterSubscriber;
        private readonly IContactUsService _contactUs;
        private readonly IpInfo _ipInfo;
        private readonly ITestimonialsService _testimonial;
        private readonly IAuthenticationManager _authenticationManager;
        public HomeController(ICityService city, IDoctorService doctor, IDoctorFacilityAffiliationService doctorFacility, IFeaturedDoctorService featuredDoctor,
            IFeaturedSpecialityService featuredSpeciality, IFeaturedService featuredService, IAuthenticationManager authenticationManager,
            IFacilityService facilityService, IPharmacyService pharmacyService, ISeniorCareService seniorCareService, ISpecialityService speciality, INewsletterSubscriberService newsletterSubscriber, IContactUsService contactUs, IpInfo ipInfo, ITestimonialsService testimonial)
        {
            _city = city;
            _doctor = doctor;
            _doctorFacility = doctorFacility;
            _featuredDoctor = featuredDoctor;
            _speciality = speciality;
            _featured = featuredService;
            _authenticationManager = authenticationManager;
            _featuredSpeciality = featuredSpeciality;
            _facility = facilityService;
            _pharmacy = pharmacyService;
            _seniorCare = seniorCareService;
            _newsletterSubscriber = newsletterSubscriber;
            _contactUs = contactUs;
            string ipString = GetIPString();
            try
            {
                //ErrorLog err = new ErrorLog();
                //err.LogDate = DateTime.UtcNow;
                //err.Type = "IP-Trace";
                //err.Message = ipString;
                //Common.LogsInformation(err);
                if (!string.IsNullOrEmpty(ipString))
                    _ipInfo = JsonConvert.DeserializeObject<IpInfo>(ipString);
                else
                    _ipInfo = new IpInfo();
            }
            catch
            {
                _ipInfo = new IpInfo();
            }

            _testimonial = testimonial;
        }

        [HttpGet, Route("GetZipCityStateList/{cityZipCode}")]
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
        [HttpGet, Route("GetZipCityStateListById/{cityZipCode}")]
        public ActionResult GetZipCityStateListById(string cityZipCode)
        {
            List<CityStateZipDetail> result = new List<CityStateZipDetail>();
            try
            {
                result = GetZipCityStateById(cityZipCode);
                return Json(result
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetZipCityStateListById/{zipCode}");
                return Json(new
                {
                    lstZipCode = result
                }, JsonRequestBehavior.AllowGet);
            }
        }
      
        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            //Session.Clear();
            //Session.Abandon();
            //Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetNoStore();
            //CookieHelper.DeleteCookie(CookieKey.LoggedInUserId);
            //_authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        [HttpPost]
        public ActionResult GetAllSpecialization(string specialization)
        {
            var searchName = HttpUtility.UrlDecode(specialization);
            List<string> result = new List<string>();
            try
            {
                result = GetSpecialization(searchName);
                return Json(result
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetAllSpecialization");
                return Json(new
                {
                    lstZipCode = result
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetAllOrgName(string searchText,string OrgType)
        {
            var searchName = HttpUtility.UrlDecode(searchText);
           List<string> result = new List<string>();
            try
            {
                result = GetOrgNameByType(searchName, OrgType);
                return Json(result
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetAllOrgName");
                return Json(new
                {
                    lstZipCode = result
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private List<string> GetZipCityState(string cityZipCode)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ZipcodeCity", SqlDbType.VarChar,50) { Value = cityZipCode });

            var results  = _doctor.GetDataList<CityStateZipDetail>(StoredProcedureList.spGetCityStateZipByZipcode, parameters);
            //var result = ds.Tables[0].Rows.Cast<DataRow>().ToArray();
            var list = results.Select(x => x.ZipCityState).ToList();
            return list;
        }
        private List<CityStateZipDetail> GetZipCityStateById(string cityZipCode)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ZipcodeCity", SqlDbType.VarChar, 50) { Value = cityZipCode });

            var results = _doctor.GetDataList<CityStateZipDetail>(StoredProcedureList.spGetCityStateZipByZipcode, parameters);
            //var result = ds.Tables[0].Rows.Cast<DataRow>().ToArray();
            var list = results.OrderBy(x=>x.ZipCityState).ToList();
            return list;
        }
        private List<string> GetSpecialization(string specialization)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Specialization", SqlDbType.VarChar, 250) { Value = specialization });

            var results = _doctor.GetDataList<string>(StoredProcedureList.spGetSpecialization, parameters).ToList();
            var list = results.Select(x => x.Trim()).ToList();
            return list;
        }

        [HttpPost]
        public ActionResult GetAllDrugName(string drugname)
        {
            List<string> result = new List<string>();
            try
            {
               result = GetDrugName(drugname);
                return Json(result
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetAllSpecialization");
                return Json(new
                {
                    lstZipCode = result
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<string> GetDrugName(string drugname)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DrugName", SqlDbType.VarChar, 250) { Value = drugname });
            var results = _doctor.GetDataList<string>(StoredProcedureList.spGetDrug, parameters).ToList();
            var list = results.Select(x => x.Trim()).ToList();
            return list;
        }

        private List<string> GetOrgNameByType(string searchText, string OrgType)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OrgType", SqlDbType.VarChar, 250) { Value = OrgType });
            parameters.Add(new SqlParameter("@SearchText", SqlDbType.VarChar, 250){ Value = searchText });
            var results = _doctor.GetDataList<string>(StoredProcedureList.spGetOrgNameByTypeId, parameters).ToList();
            var list = results.Select(x => x.Trim()).ToList();
            return list;
        }

      

        /// <summary>
        /// Returns ZIP and City for the input IPAddress if exists
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> getCityZipByIP(string IPAddress)
        {
            List<Dictionary<string, object>> CityZipList = null;
            DataSet ds = null;

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@IPAddress", SqlDbType.NVarChar)  {Value = IPAddress },

            };

            ds = _doctor.GetDataSetList(StoredProcedureList.GetIPZIPMapping, parameters);

            CityZipList = Common.ConvertToList(ds.Tables[0]);

            return CityZipList;

        }


        /// <summary>
        /// Add Mapping data for city, zip and ipaddress
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <param name="City"></param>
        /// <param name="Zip"></param>
        private void AddIPCityZip(string IPAddress, string City, string Zip, string RegionCode, string LocationJson)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@IPAddress", SqlDbType.NVarChar) { Value = IPAddress });
            parameters.Add(new SqlParameter("@ZipCode", SqlDbType.NVarChar) { Value = Zip });
            parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = City });
            parameters.Add(new SqlParameter("@RegionCode", SqlDbType.NVarChar) { Value = RegionCode });
            parameters.Add(new SqlParameter("@LocationJson", SqlDbType.NVarChar) { Value = LocationJson });
            try
            {
                _doctor.AddOrUpdateExecuteProcedure("prc_IPCityZip_Insert", parameters);
            }
            catch (Exception ex)
            {

            }
        }

        private List<FeaturedFacilityListModel> GetAdvertiesmentOther(string City, string PostalCode)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = City });
            parameters.Add(new SqlParameter("@PostalCode", SqlDbType.NVarChar) { Value = PostalCode });
            return  _doctor.GetDataList<FeaturedFacilityListModel>(StoredProcedureList.GetHomePageFeaturedFacilityList, parameters).ToList();
            
           
        }

        [OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Index()
        {
            try
            {
                var model = new HomeViewModel();
                string text = System.IO.File.ReadAllText(Server.MapPath(StaticFilePath.WebSettings)).NullToString();
                model.NetworkCount = string.IsNullOrEmpty(text) ? new NetworkCount() : JsonConvert.DeserializeObject<NetworkCount>(text);
                var FeaturedSpecialityDetails = _featuredSpeciality.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
                model.FeaturedSpecialities = new List<FeaturedSpecialityViewModel>();
                if (FeaturedSpecialityDetails != null)
                {
                    model.FeaturedSpecialities = FeaturedSpecialityDetails.Select(x => new FeaturedSpecialityViewModel()
                    {
                        Id = x.FeaturedSpecialityId == 0 ? 0 : x.FeaturedSpecialityId,
                        SpecialityName = x.Speciality == null ? "" : x.Speciality.SpecialityName,
                        Description = x.Description,
                        ProfilePicture = FilePathList.FeaturedSpeciality == null ? "" : FilePathList.FeaturedSpeciality + x.ProfilePicture
                    }).ToList();
                }



                /*
                 CityZipList = getCityZipByIP(IpAddress);

                 if (CityZipList != null && CityZipList.Count > 0)
                 {
                     objLoc.Add("city", CityZipList[0]["City"].ToString());
                     objLoc.Add("zip", CityZipList[0]["ZipCode"].ToString());
                     objLoc.Add("region_code", CityZipList[0]["RegionCode"].ToString());
                 }
                 else
                 {
                     strLoc =Common.GetIPString(IpAddress);
                     objLoc = JObject.Parse(strLoc);

                     if (objLoc["city"] != null && objLoc["zip"] != null && objLoc["region_code"] != null)
                         AddIPCityZip(IpAddress, objLoc["city"].ToString(), objLoc["zip"].ToString(), objLoc["region_code"].ToString(), strLoc);
                 }


                 var parameters = new List<SqlParameter>();
                 parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = objLoc["city"].ToString() });
                 parameters.Add(new SqlParameter("@PostalCode", SqlDbType.NVarChar) { Value = objLoc["zip"].ToString() });

                 model.FeaturedDoctors = _featured.GetHomePageFeaturedDoctorList(StoredProcedureList.GetHomePageFeaturedDoctorList, parameters).ToList();
                 //model.FeaturedDoctors = App_Helpers.APIHelper.GetSyncList<FeaturedDoctorListModel>(App_Helpers.APIHelper.GetHomePageFeaturedDoctorList).ToList();
                 //model.FeaturedDoctors = _featured.GetHomePageFeaturedDoctorList(StoredProcedureList.GetHomePageFeaturedDoctorList, new object[] { }).ToList();
                 var fDs = new List<FeaturedDoctorListModel>();
                 foreach (var item in model.FeaturedDoctors)
                 {
                     if (!fDs.Any(f => f.DoctorId == item.DoctorId))
                         fDs.Add(item);
                 }

                 model.FeaturedDoctors = fDs;
                 model.FeaturedDoctors.ForEach(q => q.ProfileImage = FilePathList.Advertisement + q.ProfileImage);

                 model.Facilities = GetAdvertiesmentOther(objLoc["city"].ToString(), objLoc["zip"].ToString()).ToList(); 
                 model.Facilities.ForEach(q => q.ProfileImage = FilePathList.Advertisement + q.ProfileImage);*/

                //model.Facilities = App_Helpers.APIHelper.GetSyncList<FeaturedFacilityListModel>(App_Helpers.APIHelper.GetHomePageFeaturedFacilityList).ToList();
                // model.Facilities = _featured.GetHomePageFeaturedFacilityList(StoredProcedureList.GetHomePageFeaturedFacilityList, parameters).ToList();
                //model.Facilities = _featured.GetHomePageFeaturedFacilityList(StoredProcedureList.GetHomePageFeaturedFacilityList, new object[] { }).ToList();
                char[] whitespace = new char[] { ' ', '\t' };
                if(!System.IO.File.Exists(Server.MapPath(StaticFilePath.recordslogFile)))
                {
                    FileStream fs = new FileStream(Server.MapPath(StaticFilePath.recordslogFile), FileMode.Create);                   
                    fs.Close();
                }                   
                string data = System.IO.File.ReadAllText(Server.MapPath(StaticFilePath.recordslogFile)).NullToString();
                if(data.Contains(DateTime.Now.ToString("MM/dd/yyyy")))
                {
                    model.DoctorCount = Convert.ToInt32(data.Split(',')[0].Split(whitespace)[1].ToString());
                    model.FacilityCount = Convert.ToInt32(data.Split(',')[1].ToString());
                    model.PharmacyCount = Convert.ToInt32(data.Split(',')[2].ToString());
                    model.SeniorcareCount = Convert.ToInt32(data.Split(',')[3].ToString());
                }
                else
                {
                    model.DoctorCount = _doctor.GetCount(x => x.IsActive & !x.IsDeleted);
                    model.FacilityCount = _facility.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Facility");
                    model.PharmacyCount = _pharmacy.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Pharmacy");
                    model.SeniorcareCount = _seniorCare.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Senior Care");

                    string output = DateTime.Now.ToString("MM/dd/yyyy") + " "+ model.DoctorCount +","+ model.FacilityCount+","+ model.PharmacyCount + ","+ model.SeniorcareCount;
                    System.IO.File.WriteAllText(Server.MapPath(StaticFilePath.recordslogFile), output);
                }
               
                DataSet BlogDataset = _doctor.GetQueryResult(StoredProcedureList.GetHomePageBlogDetails + " ");
                model.BlogItems = Common.ConvertDataTable<Doctyme.Model.ViewModels.BlogItem>(BlogDataset.Tables[0]).ToList();
                //List<string> lst = new List<string>();
                //lst.Add("/Uploads/Blog/Facility-637274022689072341.jpg");
                //lst.Add("/Uploads/Blog/Facility-637274030616000926.jpg");
                //model.BlogItems = result.OrderByDescending(i => i.CreateDate).Take(3).ToList();
                 //model.BlogItems = result.Where(i => i.MainSite == true && i.IsActive && !i.Deleted).OrderByDescending(x=>x.CreateDate).Take(3).ToList();
                //model.BlogItems = result.Where(i => i.MainSite).Take(3).ToList();
                string _blogUrl = RequestHelpers.GetConfigValue("SMTP_USERNAME");
                model.BlogItems.ForEach(i => i.BlogUrl = _blogUrl+ "//Blog//BlogItem//" + i.ArticleName.Replace(" ", "-").Replace("&", "-"));

                model.Testimonials = _testimonial.GetTestiMonialsForHome();

                //var featuredSpecialities = _featuredSpeciality.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
                //model.FeaturedSpecialities = featuredSpecialities
                //  .Select(x => new FeaturedSpecialityViewModel()
                //  {
                //      Id = x.FeaturedSpecialityId,
                //      SpecialityName = x.Speciality.SpecialityName,
                //      Description = x.Description,
                //      ProfilePicture = FilePathList.FeaturedSpeciality + x.ProfilePicture
                //  }).ToList();

                //var FeaturedDoctorList = App_Helpers.APIHelper.GetSyncList<Featured>(App_Helpers.APIHelper.GetFeaturedDoctorList);

                //var FeaturedDoctorDetails = App_Helpers.APIHelper.GetSyncList<FeaturedDoctor>(App_Helpers.APIHelper.GetFeaturedDoctorsDetails);

                //model.FeaturedDoctors = null;
                //model.FeaturedDoctors = _featured.GetAll(x => x.IsActive && !x.IsDeleted && x.UserTypeID == 2).ToList()
                //    .Select(x => new FeaturedDoctorViewModel()
                //    {
                //        Id = x.Doctor.DoctorId,
                //        DoctorName = x.Doctor.FirstName + x.Doctor.LastName,
                //        ProfilePicture = FilePathList.FeaturedDoctor + x.ProfileImage,
                //        SpecialityName = x.Doctor.DoctorSpecialities.Count > 0 ? x.Doctor.DoctorSpecialities.FirstOrDefault().Speciality.SpecialityName : string.Empty,
                //        Reviews = x.Doctor.Reviews == null ? 0 : x.Doctor.Reviews.Count()
                //    }).ToList();

                //model.FeaturedDoctors = _featuredDoctor.GetAll(x => x.IsActive && !x.IsDeleted)
                //    .Select(x => new FeaturedDoctorViewModel()
                //    {
                //        Id = x.Doctor.DoctorId,
                //        DoctorName = x.Doctor.FirstName + x.Doctor.LastName,
                //        ProfilePicture = FilePathList.FeaturedDoctor + x.ProfilePicture,
                //        SpecialityName = x.Doctor.DoctorSpecialities.Count > 0 ? x.Doctor.DoctorSpecialities.FirstOrDefault().Speciality.SpecialityName : string.Empty,
                //        Reviews = x.Doctor.Reviews == null ? 0 : x.Doctor.Reviews.Count(),
                //        SocialMedia = AutoMapper.Mapper.Map<SocialMediaViewModel>(x.Doctor.SocialMediaLinks.FirstOrDefault())
                //    }).ToList();


                //var DoctorFacilityDetails = App_Helpers.APIHelper.GetSyncList<DoctorFacilityAffiliation>(App_Helpers.APIHelper.GetDoctorsFacilityDetails);
                //model.Facilities = DoctorFacilityDetails
                //    .Select(x => new ViewModels.Facility()
                //    {
                //        Id = x.FacilityId,
                //        FacilityName = x.Facility.FacilityName,
                //        ProfilePicture = x.Facility.FacilityUser.ProfilePicture
                //    }).ToList();


                //model.Facilities = _doctorFacility.GetAll(x => x.IsActive && !x.IsDeleted)
                //   .Select(x => new ViewModels.Facility()
                //   {
                //       Id = x.FacilityId,
                //       FacilityName = x.Facility.FacilityName,
                //       ProfilePicture = x.Facility.FacilityUser.ProfilePicture
                //   }).ToList();

                // model.DoctorCount = _doctor.GetCount(x => x.IsActive);
                //model.FacilityCount = _facility.GetCount(x => x.IsActive);
                //model.PharmacyCount = _pharmacy.GetCount(x => x.IsActive);
                // model.SeniorcareCount = _seniorCare.GetCount(x => x.IsActive);
                model.IpInfo = _ipInfo;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetTopDoctor()
        {
            Random rnd = new Random();
            var FeaturedDoctors = _featuredDoctor.GetAll(x => x.IsActive && !x.IsDeleted).OrderBy(doc => rnd.Next()).Take(3)
                .Select(x => new FeaturedDoctorViewModel()
                {
                    Id = x.Doctor.DoctorId,
                    DoctorName = x.Doctor.NamePrefix + x.Doctor.FirstName + " " + x.Doctor.LastName,
                    ProfilePicture = FilePathList.FeaturedDoctor + x.ProfilePicture,
                    SpecialityName = x.Doctor.Credential, //x.Doctor.DoctorSpecialities.Count > 0 ? x.Doctor.DoctorSpecialities.FirstOrDefault().Speciality.SpecialityName : string.Empty,
                    Reviews = x.Doctor.Reviews == null ? 0 : x.Doctor.Reviews.Count(),
                    NPI = x.Doctor.NPI
                }).ToList();
            return PartialView("_TopDoctor", FeaturedDoctors);
        }

        public ActionResult GetSpeciality()
        {
            var featuredSpecialities = _speciality.GetAll(x => x.IsActive && !x.IsDeleted).Take(8).Select(x => new FeaturedSpecialityViewModel
            {
                SpecialityName = x.SpecialityName,
                SpecialityId = x.SpecialityId
            }).ToList();

            return PartialView("_Speciality", featuredSpecialities);
        }

        [HttpPost]
        public JsonResult SubscribeNewsLetter(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return Json(new JsonResponse { Status = 0, Message = "Please enter email address." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var NewsLetter = _newsletterSubscriber.GetSingle(x => x.EmailID.ToLower().Trim() == Email.ToLower().Trim() && x.IsActive && !x.IsDeleted);
                if (NewsLetter != null)
                    return Json(new JsonResponse { Status = 0, Message = "You are already subscribed to our newsletter." }, JsonRequestBehavior.AllowGet);
                else
                {
                    var newsletterSubscriber = new NewsletterSubscriber();
                    newsletterSubscriber.EmailID = Email;
                    newsletterSubscriber.IsActive = true;
                    newsletterSubscriber.IsDeleted = false;
                    newsletterSubscriber.SubscribeDate = DateTime.Now;
                    _newsletterSubscriber.InsertData(newsletterSubscriber);
                    _newsletterSubscriber.SaveData();
                    return Json(new JsonResponse { Status = 1, Message = "You have successfully subscribed to our newsletter." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Route("AboutUs")]
        public ActionResult AboutUs()
        {
            ViewBag.Message = "About Us";
            ViewBag.DoctorsCount = _doctor.GetAll().Where(x => x.IsActive && !x.IsDeleted).Count();
            ViewBag.SpecialitiesCount = _speciality.GetAll().Where(x => x.IsActive && !x.IsDeleted).Count();
            // ViewBag.HospitalsCount = _facility.GetAll().Where(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Facility").Count();
            ViewBag.HospitalsCount = _facility.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Facility");
            return View();
        }

        //[Route("Home/ContactUs")]
        [HttpGet]
        public ActionResult ContactUs()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ContactUs(ContactUsViewModel ContactUs)
        {
            string Message = "Thank you! Your message has been successfully sent.";
            int Status = 1;
            try
            {
                var Contact_Us = new ContactUs();
                Contact_Us.Name = ContactUs.Name;
                Contact_Us.Subject = ContactUs.Subject;
                Contact_Us.Department = ContactUs.Department;
                Contact_Us.Message = ContactUs.Message;
                Contact_Us.Email = ContactUs.Email;
                Contact_Us.DateSubmit = DateTime.UtcNow.Date;
                Contact_Us.CreatedDate = DateTime.UtcNow;

                Contact_Us.IsActive = true;
                Contact_Us.IsDeleted = false;
                _contactUs.InsertData(Contact_Us);
                _contactUs.SaveData();
                return Json(new JsonResponse { Status = Status, Message = Message });
            }
            catch (Exception ex)
            {
                Message = "Error";
                Status = 2;
                return Json(new JsonResponse { Status = Status, Message = Message });
            }

        }

        [Route("Home/TermsAndConditions")]
        public ActionResult TermsAndConditions()
        {
            ViewBag.Message = "Terms And Conditions";

            return View();
        }

        [Route("Home/PrivacyPolicy")]
        public ActionResult PrivacyPolicy()
        {
            ViewBag.Message = "Privacy Policy";

            return View();
        }

        [Route("Doctors")]
        public ActionResult Doctors()
        {
            ViewBag.Message = "Doctors";

            return View();
        }

        [Route("Doctors/Specialities")]
        public ActionResult Specialities()
        {
            ViewBag.Message = "Doctor Specialities";

            return View();
        }

        [Route("Home/Facility")]
        public ActionResult Facility()
        {
            ViewBag.Message = "Facility";

            return View();
        }

        [Route("Home/ClaimYourListing")]
        public ActionResult ClaimYourListing()
        {
            ViewBag.Message = "ClaimYourListing";

            return View();
        }

        [Route("Home/AdvertiseWithUs")]
        public ActionResult AdvertiseWithUs()
        {
            ViewBag.Message = "AdvertiseWithUs";

            return View();
        }

        [Route("Facility/Types")]
        public ActionResult FacilityTypes()
        {
            ViewBag.Message = "Facility Types";

            return View();
        }

        [Route("Home/Pharmacy")]
        public ActionResult Pharmacy()
        {
            ViewBag.Message = "Pharmacy";

            return View();
        }

        [Route("home/SeniorCare")]
        public ActionResult SeniorCare()
        {
            ViewBag.Message = "Senior Care";

            return View();
        }

        [Route("Home/Blog")]
        public ActionResult Blog()
        {
            ViewBag.Message = "Blog";

            return View();
        }



        public ActionResult TestMail()
        {
            try
            {
                SendMail.SendEmail("cgankit@gmail.com", "", "", "", "<h1>Test<h1>", "Test Mail");
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ex.ToString();
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public JsonResult GetCityList(int id)
        {
            var result = _city.GetAll(x => x.IsActive && !x.IsDeleted && x.StateId == id).Select(x => new SelectListItem
            {
                Text = x.CityName,
                Value = x.CityId.ToString()
            }).ToList();
            return Json(new JsonResponse { Status = 1, Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string Test()
        {
            return "Application is running fine";
        }

        private string GetIPString()
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

            //VisitorsIPAddr = "68.37.76.206";
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

        [Route("Home/AdsView")]
        public ActionResult AdsHomeView(string IpAddress)
        {
            var model = new HomeViewModel();
            JObject objLoc = new JObject();
            JObject objLocServer = new JObject();
            Dictionary<string, object> returnDict = new Dictionary<string, object>();
            var AddList = new List<Dictionary<string, object>>();
            var CityZipList = new List<Dictionary<string, object>>();
            var strLoc = "";

            if(string.IsNullOrEmpty(IpAddress))
             IpAddress = Common.GetPublicIPOfServer();

            CityZipList = getCityZipByIP(IpAddress);

            if (CityZipList != null && CityZipList.Count > 0)
            {
                objLoc.Add("city", CityZipList[0]["City"].ToString());
                objLoc.Add("zip", CityZipList[0]["ZipCode"].ToString());
                objLoc.Add("region_code", CityZipList[0]["RegionCode"].ToString());
            }
            else
            {
                strLoc = Common.GetIPString(IpAddress);
                objLoc = JObject.Parse(strLoc);

                if (objLoc["city"] != null && objLoc["zip"] != null && objLoc["region_code"] != null)
                    AddIPCityZip(IpAddress, objLoc["city"].ToString(), objLoc["zip"].ToString(), objLoc["region_code"].ToString(), strLoc);
            }


            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = objLoc["city"].ToString() });
            parameters.Add(new SqlParameter("@PostalCode", SqlDbType.NVarChar) { Value = objLoc["zip"].ToString() });

            model.FeaturedDoctors = _featured.GetHomePageFeaturedDoctorList(StoredProcedureList.GetHomePageFeaturedDoctorList, parameters).ToList();
            //model.FeaturedDoctors = App_Helpers.APIHelper.GetSyncList<FeaturedDoctorListModel>(App_Helpers.APIHelper.GetHomePageFeaturedDoctorList).ToList();
            //model.FeaturedDoctors = _featured.GetHomePageFeaturedDoctorList(StoredProcedureList.GetHomePageFeaturedDoctorList, new object[] { }).ToList();
            //var fDs = new List<FeaturedDoctorListModel>();
            //foreach (var item in model.FeaturedDoctors)
            //{
            //    if (!fDs.Any(f => f.DoctorId == item.DoctorId))
            //        fDs.Add(item);
            //}

            model.FeaturedDoctors = model.FeaturedDoctors;
            model.FeaturedDoctors.ForEach(q => q.ProfileImage = FilePathList.Advertisement + q.ProfileImage);

            ViewBag.FeaturedDoctors = model.FeaturedDoctors;
            
            model.Facilities = GetAdvertiesmentOther(objLoc["city"].ToString(), objLoc["zip"].ToString()).ToList();
            model.Facilities.ForEach(q => q.ProfileImage = FilePathList.Advertisement + q.ProfileImage);

            ViewBag.Facilities = model.Facilities;
            if(User.Identity.GetClaimValue(UserClaims.IsRemember) == "F")
              EnsureLoggedOut();
            return PartialView("_homeAdevertiesment");
        }
    }
}
