using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DoctorInsuranceRepository : GenericRepository<DoctorInsurance>, IDoctorInsuranceService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorInsuranceRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
