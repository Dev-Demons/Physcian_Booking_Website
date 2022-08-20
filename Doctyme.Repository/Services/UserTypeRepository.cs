
using Doctyme.Model;
using Doctyme.Repository.Interface;


namespace Doctyme.Repository.Services
{
    public class UserTypeRepository : GenericRepository<UserType>, IUserTypeService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;
        public UserTypeRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public string GetQuerySingleResult(string query)
        {
            return _dbContext.GetQuerySingleResult(query);
        }
       
    }
}
