using Doctyme.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Binke.Repository.Services
{
   public class TabletRepository: GenericRepository<Tablet>, ITabletService
    {
        private readonly Doctyme.Model.Doctyme _dbContext;

        public TabletRepository(Doctyme.Model.Doctyme dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
