using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class PharmacyRepository : GenericRepository<Pharmacy>, IPharmacyService
    {
        private readonly BinkeDbContext _dbContext;

        public PharmacyRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
