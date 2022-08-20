using Doctyme.Model;

using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class ExperienceRepository : GenericRepository<Experience>, IExperienceService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public ExperienceRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
