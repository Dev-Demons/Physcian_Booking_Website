using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class ErrorLogRepository : GenericRepository<ErrorLog>, IErrorLogService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public ErrorLogRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
