using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class AgeGroupRepository : GenericRepository<AgeGroup>, IAgeGroupService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public AgeGroupRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
