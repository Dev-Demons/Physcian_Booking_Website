using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class SpecialityRepository : GenericRepository<Speciality>, ISpecialityService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public SpecialityRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
