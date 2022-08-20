using Binke.Model.DBContext;
using Binke.Model.Utility;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserService
    {
        private readonly BinkeDbContext _dbContext;
        public UserRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public string GetQuerySingleResult(string query)
        {
            return _dbContext.GetQuerySingleResult(query);
        }
    }
}
