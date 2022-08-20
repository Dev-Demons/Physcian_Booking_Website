using System.Collections.Generic;
using System.Linq;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class SlotRepository : GenericRepository<Slot>, ISlotService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public SlotRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SpSlotViewModel> GetSlotList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<SpSlotViewModel>(spName, paraObjects).ToList();
        }
    }

}
