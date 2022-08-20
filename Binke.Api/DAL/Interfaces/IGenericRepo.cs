using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Binke.Api.Models;
using Doctyme.Model;

namespace Binke.Api.DAL.Interfaces
{
    public interface IGenericMaster<T>
    {
        
        List<T> GetAll(string spname, string Activity, Pagination pager);
        T GetRecordsById(string spname, string Activity, int id);
        DataTable GetTableById(string spname, string Activity, int? id);
        Task<DataTable> GetTableByIdAsync(string spname, string Activity, int? id);
        List<T> GetAllById(string spname, string Activity, int? id);
    }
}
