using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class QualificationRepository : GenericRepository<Qualification>, IQualificationService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public QualificationRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
