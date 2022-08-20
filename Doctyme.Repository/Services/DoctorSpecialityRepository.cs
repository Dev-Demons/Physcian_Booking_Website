using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class DoctorSpecialityRepository : GenericRepository<DoctorSpeciality>, IDoctorSpecialityService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorSpecialityRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
