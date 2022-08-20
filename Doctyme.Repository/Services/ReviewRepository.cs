using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class ReviewRepository : GenericRepository<Review>, IReviewService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;
        public ReviewRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
