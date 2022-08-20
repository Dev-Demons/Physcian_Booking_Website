using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class DoctorLanguageRepository : GenericRepository<DoctorLanguage>, IDoctorLanguageService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorLanguageRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
