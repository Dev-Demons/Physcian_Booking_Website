using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DoctorImageRepository : GenericRepository<DoctorImage>, IDoctorImageService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorImageRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
