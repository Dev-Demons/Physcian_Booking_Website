using Doctyme.Model;

using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class FeaturedSpecialityRepository : GenericRepository<FeaturedSpeciality>, IFeaturedSpecialityService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public FeaturedSpecialityRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
