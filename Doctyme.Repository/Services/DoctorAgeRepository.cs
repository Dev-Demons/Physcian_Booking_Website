using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class DoctorAgeRepository : GenericRepository<DoctorAgeGroup>, IDoctorAgeGroupService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorAgeRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
