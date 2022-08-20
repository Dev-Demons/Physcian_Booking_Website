using Binke.Api.Utility;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Binke.Api.Controllers
{
    public class ProvidersProfileController : ApiController
    {
        private readonly IDoctorService _doctor;
        public ProvidersProfileController(IDoctorService doctor)
        {
            _doctor = doctor;
        }

        #region Doctor-Profile
        [HttpPost]
        [Route("api/Profile/Facility/{orgId}")]
        public HttpResponseMessage Facility(int orgId)
        {
            Doctyme.Model.ViewModels.ProfileViewModel _model = new Doctyme.Model.ViewModels.ProfileViewModel();
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetFacilityProfileDetails + " " + orgId);
            var orgProfile = Common.ConvertDataTable<Doctyme.Model.ViewModels.ProfileViewModel>(ds.Tables[0]).FirstOrDefault();
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

            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = orgProfile });
        }
        #endregion

        [HttpPost]
        [Route("api/Profile/Pharmacy/{orgId}")]
        public HttpResponseMessage Pharmacy(int orgId) //string OrganisationName, 
        {
            Doctyme.Model.ViewModels.ProfileViewModel _model = new Doctyme.Model.ViewModels.ProfileViewModel();
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetPharmacyProfileDetails + " " + orgId);
            var orgProfile = Common.ConvertDataTable<Doctyme.Model.ViewModels.ProfileViewModel>(ds.Tables[0]).FirstOrDefault();
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
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = orgProfile });
        }

        [HttpPost]
        [Route("api/Profile/SeniorCare/{orgId}")]
        public HttpResponseMessage SeniorCare(int orgId) //string OrganisationName, 
        {
            Doctyme.Model.ViewModels.ProfileViewModel _model = new Doctyme.Model.ViewModels.ProfileViewModel();
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetSeiorCareProfileDetails + " " + orgId);
            var orgProfile = Common.ConvertDataTable<Doctyme.Model.ViewModels.ProfileViewModel>(ds.Tables[0]).FirstOrDefault();
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
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = HttpStatusCode.OK, Data = orgProfile });
        }
    }
}
