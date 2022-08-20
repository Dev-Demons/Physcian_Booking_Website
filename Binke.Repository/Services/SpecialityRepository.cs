using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class SpecialityRepository : GenericRepository<Speciality>, ISpecialityService
    {
        private readonly BinkeDbContext _dbContext;

        public SpecialityRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
