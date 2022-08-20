using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleService
    {
        private readonly BinkeDbContext _dbContext;
        public UserRoleRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
