
using Binke.Api.Models;
using Doctyme.Repository;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;

namespace Binke.Api.Controllers
{
    
    public class HomeController : ApiController
    {
        private readonly IFeaturedSpecialityService _featuredSpeciality;
        private readonly IFeaturedService _featured;
        private readonly IFeaturedDoctorService _featuredDoctor;
        private readonly IDoctorFacilityAffiliationService _doctorFacility;
        private readonly IDoctorService _doctor;
        private readonly IFacilityService _facility;
        private readonly IPharmacyService _pharmacy;
        private readonly ISeniorCareService _seniorCare;

        public HomeController(IFeaturedSpecialityService featuredSpeciality,
            IFeaturedDoctorService featuredDoctor,
            IDoctorFacilityAffiliationService doctorFacility,
            IDoctorService doctor,
            IFacilityService facility,
            IPharmacyService pharmacy,
            ISeniorCareService seniorCare,
            IFeaturedService featured
            )
        {
            _featuredSpeciality = featuredSpeciality;
            _featuredDoctor = featuredDoctor;
            _doctorFacility = doctorFacility;
            _doctor = doctor;
            _facility = facility;
            _pharmacy = pharmacy;
            _seniorCare = seniorCare;
            _featured = featured;
        }


        [HttpGet]
        [Route("api/Home/GetFeaturedSpecialityDetails")]
        public HttpResponseMessage GetFeaturedSpecialityDetails()
        {
            var FeaturedSpecialityList = _featuredSpeciality.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, FeaturedSpecialityList);

        }

        [HttpGet]
        [Route("api/Home/GetFeaturedDoctorsDetails")]
        public HttpResponseMessage GetFeaturedDoctorsDetails()
        {
            try
            {
                var FeaturedDoctorList = _featuredDoctor.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
                var result = FeaturedDoctorList.Select(i => new Doctyme.Model.FeaturedDoctor
                {
                    CreatedBy = i.CreatedBy,
                    CreatedDate = i.CreatedDate,

                    DoctorId = i.DoctorId,
                    IsActive = i.IsActive,
                    IsDeleted = i.IsDeleted,
                    ModifiedBy = i.ModifiedBy,
                    ProfilePicture = i.ProfilePicture,
                    UpdatedDate = i.UpdatedDate,
                    FeaturedDoctorId = i.FeaturedDoctorId

                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new List<Doctyme.Model.FeaturedDoctor>());
            }

        }

        [HttpGet]
        [Route("api/Home/GetDoctorsFacilityDetails")]
        public HttpResponseMessage GetDoctorsFacilityDetails()
        {
            var DoctorFacilityList = _doctorFacility.GetAll(x => x.IsActive && !x.IsDeleted).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, DoctorFacilityList);
        }

        [HttpGet]
        [Route("api/Home/GetHomePageFeaturedDoctorList")]
        public HttpResponseMessage GetHomePageFeaturedDoctorList()
        {
            var FeaturedDoctorList = _featured.GetHomePageFeaturedDoctorList(StoredProcedureList.GetHomePageFeaturedDoctorList, new object[] { }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, FeaturedDoctorList);
        }

        [HttpGet]
        [Route("api/Home/GetHomePageFeaturedFacilityList")]
        public HttpResponseMessage GetHomePageFeaturedFacilityList()
        {
            var FeaturedFacilityList = _featured.GetHomePageFeaturedFacilityList(StoredProcedureList.GetHomePageFeaturedFacilityList, new object[] { }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, FeaturedFacilityList);
        }

        [HttpGet]
        [Route("api/Home/GetDoctorCount")]
        public HttpResponseMessage GetDoctorCount()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _doctor.GetCount(x => x.IsActive));
        }

        [HttpGet]
        [Route("api/Home/GetFacilityCount")]
        public HttpResponseMessage GetFacilityCount()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _facility.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Facility"));
        }

        [HttpGet]
        [Route("api/Home/GetPharmacyCount")]
        public HttpResponseMessage GetPharmacyCount()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _pharmacy.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Pharmacy"));
        }

        [HttpGet]
        [Route("api/Home/GetSeniorCareCount")]
        public HttpResponseMessage GetSeniorCareCount()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _seniorCare.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Senior Care"));
        }
        [HttpPost]
        [Authorize]
        [Route("api/Home/HomeFeaturesData")]
        public HttpResponseMessage HomeFeaturesData(HomeViewModel obj)
        {
            var model = new HomeViewModel();
            model.CityName = obj.CityName;
            model.PostalCode = obj.PostalCode;
            var FeaturedSpecialityDetails = _featuredSpeciality.GetAll(x => x.IsActive && !x.IsDeleted).ToList();

            model.FeaturedSpecialities = null;
            model.FeaturedSpecialities = FeaturedSpecialityDetails.Select(x => new FeaturedSpecialityViewModel()
            {
                Id = x.FeaturedSpecialityId == 0 ? 0 : x.FeaturedSpecialityId,
                SpecialityName = x.Speciality == null ? "" : x.Speciality.SpecialityName,
                Description = x.Description,
                ProfilePicture = FilePathList.FeaturedSpeciality == null ? "" : "https://www.doctyme.com/" + FilePathList.FeaturedSpeciality + x.ProfilePicture
            }).ToList();

            var parametersDoctor = new List<SqlParameter>();
            parametersDoctor.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = obj.CityName });
            parametersDoctor.Add(new SqlParameter("@PostalCode", SqlDbType.NVarChar) { Value = obj.PostalCode });

            var parametersFacility = new List<SqlParameter>();
            parametersFacility.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = obj.CityName });
            parametersFacility.Add(new SqlParameter("@PostalCode", SqlDbType.NVarChar) { Value = obj.PostalCode });

            var FeaturedDoctorList = _featured.GetHomePageFeaturedDoctorList(StoredProcedureList.GetHomePageFeaturedDoctorList, parametersDoctor).ToList();
            model.FeaturedDoctors = FeaturedDoctorList;
            //model.FeaturedDoctors = _featured.GetHomePageFeaturedDoctorList(StoredProcedureList.GetHomePageFeaturedDoctorList, new object[] { }).ToList();
            model.FeaturedDoctors.ForEach(q => q.ProfileImage = "https://www.doctyme.com/"+FilePathList.FeaturedDoctor + q.ProfileImage);

            model.Facilities = _featured.GetHomePageFeaturedFacilityList(StoredProcedureList.GetHomePageFeaturedFacilityList, parametersFacility).ToList();
            //model.Facilities = _featured.GetHomePageFeaturedFacilityList(StoredProcedureList.GetHomePageFeaturedFacilityList, new object[] { }).ToList();
            model.Facilities.ForEach(q => q.ProfileImage = "https://www.doctyme.com/"+FilePathList.FeaturedFacility + q.ProfileImage);

            model.DoctorCount = _doctor.GetCount(x => x.IsActive);
            model.FacilityCount = _facility.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Facility");
            model.PharmacyCount = _pharmacy.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Pharmacy");
            model.SeniorcareCount = _seniorCare.GetCount(x => x.IsActive && !x.IsDeleted && x.OrganisationType.Org_Type_Name == "Senior Care");
            return Request.CreateResponse(HttpStatusCode.OK, new {Status= HttpStatusCode.OK,Data = model });
        }
      


    }
}
