using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class SocialMediaRepository : GenericRepository<SocialMedia>, ISocialMediaService
    {
        private readonly BinkeDbContext _dbContext;

        public SocialMediaRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
