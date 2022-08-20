using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class ContactUsRepository : GenericRepository<ContactUs>, IContactUsService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public ContactUsRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
