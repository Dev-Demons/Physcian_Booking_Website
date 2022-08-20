using Doctyme.Model;

using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{

    public class DoctorInsuranceAcceptedRepository : GenericRepository<DoctorInsuranceAccepted>, IDoctorInsuranceAcceptedService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorInsuranceAcceptedRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
    
}
