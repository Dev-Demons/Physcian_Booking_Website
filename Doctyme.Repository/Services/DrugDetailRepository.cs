using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Repository.Services
{
   public class DrugDetailRepository: GenericRepository<DrugDetail>, IDrugDetailService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DrugDetailRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SpSearchDrugViewModel> SearchDrug(string spName, int drugDetailId)
        {
            var parameters = new SqlParameter("@DrugDetailId", SqlDbType.Int) { Value = drugDetailId };
            var result = _dbContext.Database.SqlQuery<SpSearchDrugViewModel>(spName, parameters).Where(item=>item.DrugId == drugDetailId).ToList();
            return result;
        }

        public void UpdateDate(DrugDetail obj)
        {
            throw new NotImplementedException();
        }
    }
}
