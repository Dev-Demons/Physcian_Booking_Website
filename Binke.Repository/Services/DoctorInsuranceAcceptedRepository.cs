using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{

    public class DoctorInsuranceAcceptedRepository : GenericRepository<DoctorInsuranceAccepted>, IDoctorInsuranceAcceptedService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorInsuranceAcceptedRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
    
}
