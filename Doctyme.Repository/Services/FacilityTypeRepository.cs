using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class FacilityTypeRepository : GenericRepository<OrganisationType>, IFacilityTypeService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public FacilityTypeRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
