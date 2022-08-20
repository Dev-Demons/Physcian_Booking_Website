using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class DoctorFacilityAffiliationRepository : GenericRepository<DoctorFacilityAffiliation>, IDoctorFacilityAffiliationService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorFacilityAffiliationRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
