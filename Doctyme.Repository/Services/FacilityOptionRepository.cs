using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class FacilityOptionRepository : GenericRepository<OrganizationAmenityOption>, IFacilityOptionService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public FacilityOptionRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
