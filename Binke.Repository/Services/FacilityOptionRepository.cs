using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class FacilityOptionRepository : GenericRepository<FacilityOption>, IFacilityOptionService
    {
        private readonly BinkeDbContext _dbContext;

        public FacilityOptionRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
