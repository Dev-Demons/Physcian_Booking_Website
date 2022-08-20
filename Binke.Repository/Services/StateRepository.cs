using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class StateRepository : GenericRepository<State>, IStateService
    {
        private readonly BinkeDbContext _dbContext;

        public StateRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
