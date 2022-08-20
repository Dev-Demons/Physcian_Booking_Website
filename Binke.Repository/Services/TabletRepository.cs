using Binke.Model;
using Binke.Model.DBContext;
using Binke.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Repository.Services
{
   public class TabletRepository: GenericRepository<Tablet>, ITabletService
    {
        private readonly BinkeDbContext _dbContext;

        public TabletRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
