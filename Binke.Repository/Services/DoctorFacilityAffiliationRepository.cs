using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DoctorFacilityAffiliationRepository : GenericRepository<DoctorFacilityAffiliation>, IDoctorFacilityAffiliationService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorFacilityAffiliationRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
