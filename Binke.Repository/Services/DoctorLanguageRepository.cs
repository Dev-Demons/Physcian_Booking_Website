using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DoctorLanguageRepository : GenericRepository<DoctorLanguage>, IDoctorLanguageService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorLanguageRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
