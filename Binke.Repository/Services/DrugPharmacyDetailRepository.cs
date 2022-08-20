using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DrugPharmacyDetailRepository: GenericRepository<DrugPharmacyDetail>, IDrugPharmacyDetailService
    {
        private readonly BinkeDbContext _dbContext;

        public DrugPharmacyDetailRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
