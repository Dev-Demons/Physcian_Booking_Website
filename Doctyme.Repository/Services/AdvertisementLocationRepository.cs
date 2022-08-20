using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class AdvertisementLocationRepository : GenericRepository<AdvertisementLocation>, IAdvertisementLocationService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public AdvertisementLocationRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
