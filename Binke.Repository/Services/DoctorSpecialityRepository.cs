using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DoctorSpecialityRepository : GenericRepository<DoctorSpeciality>, IDoctorSpecialityService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorSpecialityRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
