using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DoctorAgeRepository : GenericRepository<DoctorAgeGroup>, IDoctorAgeGroupService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorAgeRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
