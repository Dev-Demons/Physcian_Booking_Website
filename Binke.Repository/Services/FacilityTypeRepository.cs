using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class FacilityTypeRepository : GenericRepository<FacilityType>, IFacilityTypeService
    {
        private readonly BinkeDbContext _dbContext;

        public FacilityTypeRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
