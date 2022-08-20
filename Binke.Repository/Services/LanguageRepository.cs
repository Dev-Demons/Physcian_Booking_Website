using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageService
    {
        private readonly BinkeDbContext _dbContext;

        public LanguageRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
