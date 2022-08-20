
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{

    public class RoleRepository : GenericRepository<Role>, IRoleService
    {

        private readonly Doctyme.Model.Doctyme _dbContext;
        public RoleRepository(Doctyme.Model.Doctyme dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
