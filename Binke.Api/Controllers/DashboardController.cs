using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Doctyme.Model;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;

namespace DocTyme.Api.Controllers
{
    public class DashboardController : ApiController
    {

        private readonly IDashboardService _dashboardService;
        private readonly IDoctorService _doctor;

        public DashboardController(IDashboardService dashboardService, IDoctorService doctor)
        {
            _dashboardService = dashboardService;
            _doctor = doctor;
        }
        [HttpGet]
        public Doctor GetDoctorDetailsById(int id) 
        {
             Doctor doctor = _doctor.GetByUserId(id);
             return doctor;
        }
    }
}
