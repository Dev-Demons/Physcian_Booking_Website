using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class ExperienceRepository : GenericRepository<Experience>, IExperienceService
    {
        private readonly BinkeDbContext _dbContext;

        public ExperienceRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
