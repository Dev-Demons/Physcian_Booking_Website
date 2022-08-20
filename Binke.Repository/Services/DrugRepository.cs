using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DrugRepository : GenericRepository<Drug>, IDrugService
    {
        private readonly BinkeDbContext _dbContext;

        public DrugRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
