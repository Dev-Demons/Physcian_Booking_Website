
using Binke.Repository.Interface;
using Doctyme.Model;
using Doctyme.Repository.Services;

namespace Binke.Repository.Services
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;
        public UserRoleRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
