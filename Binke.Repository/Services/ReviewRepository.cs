using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class ReviewRepository : GenericRepository<Review>, IReviewService
    {
        private readonly BinkeDbContext _dbContext;
        public ReviewRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
