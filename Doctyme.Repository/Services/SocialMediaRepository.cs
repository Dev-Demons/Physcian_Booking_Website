using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class SocialMediaRepository : GenericRepository<SocialMedia>, ISocialMediaService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public SocialMediaRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
