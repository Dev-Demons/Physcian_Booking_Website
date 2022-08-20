using Binke.Api.Models;
using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Api.DAL.Interfaces
{
    public interface IDoctor
    {
        List<Doctor> GetAllDoctors(Pagination  pager);
    }
}
