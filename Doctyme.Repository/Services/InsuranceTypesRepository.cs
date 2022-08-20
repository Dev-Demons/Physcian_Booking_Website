using Doctyme.Model;
using Doctyme.Repository.Interface;
namespace Doctyme.Repository.Services
{
    public class InsuranceTypeRepository : GenericRepository<InsuranceType>, IInsuranceTypeRepository
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public InsuranceTypeRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
