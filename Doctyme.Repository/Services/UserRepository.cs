

using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;
        public UserRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public string GetQuerySingleResult(string query)
        {
            return _dbContext.GetQuerySingleResult(query);
        }
    }
}
