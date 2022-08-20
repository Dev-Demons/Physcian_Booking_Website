using System.Collections.Generic;
using System.Linq;
using Binke.Model;
using Binke.Model.DBContext;
using Binke.Model.ViewModels;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class TempSlotBookingRepository : GenericRepository<TempSlotBooking>, ITempSlotBookingService
    {
        private readonly BinkeDbContext _dbContext;

        public TempSlotBookingRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SpSlotViewModel> GetSlotList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<SpSlotViewModel>(spName, paraObjects).ToList();
        }
    }

}
