using System.Collections.Generic;
using System.Web.Mvc;
using Binke.App_Helpers;
using Binke.Repository.Enumerable;
using Binke.Repository.Interface;
using Binke.Utility;
using Binke.ViewModels;
using Newtonsoft.Json;

namespace Binke.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class WebSettingController : Controller
    {
        private readonly IDoctorService _doctor;
        private readonly IFacilityService _facility;

        public WebSettingController(IDoctorService doctor, IFacilityService facility)
        {
            _doctor = doctor;
            _facility = facility;
        }

        // GET: WebSetting
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NetworkCount()
        {
            string text = System.IO.File.ReadAllText(Server.MapPath(StaticFilePath.WebSettings)).NullToString();
            var jsonNetworkCount = JsonConvert.DeserializeObject<NetworkCount>(text);
            var dbDoctorCount = _doctor.GetCount(x => x.IsActive && !x.IsDeleted);
            var dbFacilityCount = _facility.GetCount(x => x.IsActive && !x.IsDeleted);
            var dbPharmacyCount = 0;
            var dbSeniorCount = 0;
            var doctorCount = jsonNetworkCount.JsonDoctorCount > 0
                                ? jsonNetworkCount.JsonDoctorCount
                                    : dbDoctorCount;
            var facilityCount = jsonNetworkCount.JsonFacilitiesCount > 0
                                ? jsonNetworkCount.JsonFacilitiesCount
                                    : dbFacilityCount;
            var pharmacyCount = jsonNetworkCount.JsonPharmacyCount > 0
                                ? jsonNetworkCount.JsonPharmacyCount
                                    : dbPharmacyCount;
            var seniorCount = jsonNetworkCount.JsonSeniorCareCount > 0
                                ? jsonNetworkCount.JsonSeniorCareCount
                                    : dbSeniorCount;
            var mostlySearchedDrug = !string.IsNullOrEmpty(jsonNetworkCount.MostlySearchedDrug) ? jsonNetworkCount.MostlySearchedDrug : string.Empty;

            var networkCount = new NetworkCount
            {

                DoctorCount = dbDoctorCount,
                FacilitiesCount = dbFacilityCount,
                PharmacyCount = dbPharmacyCount,
                SeniorCareCount = dbSeniorCount,
                JsonDoctorCount = doctorCount,
                JsonFacilitiesCount = facilityCount,
                JsonPharmacyCount = pharmacyCount,
                JsonSeniorCareCount = seniorCount,
                MostlySearchedDrug = mostlySearchedDrug
            };
            return View(networkCount);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NetworkCount(NetworkCount model)
        {
            System.IO.File.WriteAllText(Server.MapPath(StaticFilePath.WebSettings), JsonConvert.SerializeObject(model));
            var jsonModels = new List<Common.JsonModel> { new Common.JsonModel(Common.JsonType.Success, "Network Count has been save successfully") };
            this.AddJsons(jsonModels);
            return RedirectToAction("NetworkCount");
        }
    }
}
