using Binke.Api.DAL.Interfaces;
using Binke.Api.Models;
using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.Api.DAL.Repository
{
    public class DoctorRepo : GenericMasterRepo<Doctor>, IDoctor
    {
        public List<Doctor> GetAllDoctors(Pagination pager)
        {
           
          return GetAll("spDoctor", "Filter", pager);
       
        }
        public List<Doctor> GetDoctorById(int DoctorId)
        {
            return GetAllById("sprDoctor", "Find", DoctorId);
        }


    }
}