using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;


namespace Binke.Repository.Services
{
    public class AgeGroupRepository : GenericRepository<AgeGroup>, IAgeGroupService
    {
        private readonly BinkeDbContext _dbContext;

        public AgeGroupRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
