using Binke.Model;
using Binke.Model.DBContext;
using Binke.Model.ViewModels;
using Binke.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Repository.Services
{
   public class DrugDetailRepository: GenericRepository<DrugDetail>, IDrugDetailService
    {
        private readonly BinkeDbContext _dbContext;

        public DrugDetailRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SpSearchDrugViewModel> SearchDrug(string spName, int drugDetailId)
        {
            var parameters = new SqlParameter("@DrugDetailId", SqlDbType.Int) { Value = drugDetailId };
            var result = _dbContext.Database.SqlQuery<SpSearchDrugViewModel>(spName, parameters).Where(item=>item.DrugDetailId==drugDetailId).ToList();
            return result;
        }

        public void UpdateDate(DrugDetail obj)
        {
            throw new NotImplementedException();
        }
    }
}
