using Doctyme.Model;

using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DrugPharmacyDetailRepository: GenericRepository<DrugPharmacyDetail>, IDrugPharmacyDetailService
    {
        private readonly Doctyme.Model.Doctyme _dbContext;

        public DrugPharmacyDetailRepository(Doctyme.Model.Doctyme dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
