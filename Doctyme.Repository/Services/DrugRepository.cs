using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Doctyme.Repository.Services
{
    public class DrugRepository : GenericRepository<Drug>, IDrugService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DrugRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SpSearchDrugViewModel> SearchDrug(string spName, int drugId)
        {
            var parameters = new SqlParameter("@DrugDetailId", SqlDbType.Int) { Value = drugId };
            var result = _dbContext.Database.SqlQuery<SpSearchDrugViewModel>(spName, parameters).Where(item => item.DrugId == drugId).ToList();
            return result;
        }
    }

}
