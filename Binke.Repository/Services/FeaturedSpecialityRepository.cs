using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class FeaturedSpecialityRepository : GenericRepository<FeaturedSpeciality>, IFeaturedSpecialityService
    {
        private readonly BinkeDbContext _dbContext;

        public FeaturedSpecialityRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
