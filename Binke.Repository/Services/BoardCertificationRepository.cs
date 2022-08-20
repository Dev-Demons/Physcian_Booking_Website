using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class BoardCertificationRepository : GenericRepository<BoardCertification>, IBoardCertificationService
    {
        private readonly BinkeDbContext _dbContext;

        public BoardCertificationRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
