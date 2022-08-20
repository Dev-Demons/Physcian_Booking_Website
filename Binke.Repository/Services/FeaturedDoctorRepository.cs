using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class FeaturedDoctorRepository : GenericRepository<FeaturedDoctor>, IFeaturedDoctorService
    {
        private readonly BinkeDbContext _dbContext;

        public FeaturedDoctorRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
