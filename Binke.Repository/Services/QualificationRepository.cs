using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class QualificationRepository : GenericRepository<Qualification>, IQualificationService
    {
        private readonly BinkeDbContext _dbContext;

        public QualificationRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
