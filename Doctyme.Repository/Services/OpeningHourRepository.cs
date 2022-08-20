using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class OpeningHourRepository : GenericRepository<OpeningHour>, IOpeningHourService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public OpeningHourRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
