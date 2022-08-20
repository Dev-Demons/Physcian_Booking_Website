using System.Collections.Generic;
using System.Linq;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Binke.Repository.Services
{
    public class TempSlotBookingRepository : GenericRepository<TempSlotBooking>, ITempSlotBookingService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public TempSlotBookingRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SpSlotViewModel> GetSlotList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<SpSlotViewModel>(spName, paraObjects).ToList();
        }
    }

}
