using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class BoardCertificationRepository : GenericRepository<BoardCertifications>, IBoardCertificationService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public BoardCertificationRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
