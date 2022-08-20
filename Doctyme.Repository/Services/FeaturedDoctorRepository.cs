using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class FeaturedDoctorRepository : GenericRepository<FeaturedDoctor>, IFeaturedDoctorService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public FeaturedDoctorRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
