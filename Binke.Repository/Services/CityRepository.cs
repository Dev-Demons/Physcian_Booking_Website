using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class CityRepository : GenericRepository<City>, ICityService
    {
        private readonly BinkeDbContext _dbContext;

        public CityRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
