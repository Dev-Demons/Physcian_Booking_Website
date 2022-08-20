using Doctyme.Model;
using Doctyme.Repository.Interface;


namespace Doctyme.Repository.Services
{
    public class DoctorImageRepository : GenericRepository<SiteImage>, IDoctorImageService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorImageRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
