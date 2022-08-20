using System.Collections.Generic;
using System.Linq;
using Binke.Model;
using Binke.Model.DBContext;
using Binke.Model.ViewModels;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class SlotRepository : GenericRepository<Slot>, ISlotService
    {
        private readonly BinkeDbContext _dbContext;

        public SlotRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SpSlotViewModel> GetSlotList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<SpSlotViewModel>(spName, paraObjects).ToList();
        }
    }

}
