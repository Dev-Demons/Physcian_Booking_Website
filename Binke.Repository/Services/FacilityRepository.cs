using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;
using System.Linq;

namespace Binke.Repository.Services
{
    public class FacilityRepository : GenericRepository<Facility>, IFacilityService
    {
        private readonly BinkeDbContext _dbContext;

        public FacilityRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Facility GetByUserId(int userId)
        {
            return _dbContext.Facilitys.Where(f => f.UserId == userId).FirstOrDefault();
        }
    }

}
