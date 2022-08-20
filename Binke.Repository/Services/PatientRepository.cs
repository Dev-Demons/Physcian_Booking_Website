using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class PatientRepository : GenericRepository<Patient>, IPatientService
    {
        private readonly BinkeDbContext _dbContext;

        public PatientRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
