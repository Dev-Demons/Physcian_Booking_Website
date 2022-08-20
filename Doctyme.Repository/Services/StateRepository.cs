using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class StateRepository : GenericRepository<State>, IStateService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public StateRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
