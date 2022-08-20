using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class GenderRepository : GenericRepository<GenderType>, IGenderService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public GenderRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
