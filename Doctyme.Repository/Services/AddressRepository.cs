using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class AddressRepository : GenericRepository<Address>, IAddressService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public AddressRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
