using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class AddressRepository : GenericRepository<Address>, IAddressService
    {
        private readonly BinkeDbContext _dbContext;

        public AddressRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
