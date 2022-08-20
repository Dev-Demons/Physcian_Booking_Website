using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class CityStateZipRepository : GenericRepository<CityStateZip>, ICityStateZipService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public CityStateZipRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
