using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{

    public class RoleRepository : GenericRepository<Role>, IRoleService
    {

        private readonly BinkeDbContext _dbContext;
        public RoleRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
