using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class CityRepository : GenericRepository<City>, ICityService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public CityRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
