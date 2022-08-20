using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class SiteImageRepository : GenericRepository<SiteImage>, ISiteImageService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public SiteImageRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
