using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class OpeningHourRepository : GenericRepository<OpeningHour>, IOpeningHourService
    {
        private readonly BinkeDbContext _dbContext;

        public OpeningHourRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
