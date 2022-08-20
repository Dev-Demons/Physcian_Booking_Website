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
   public class DrugQuantityRepository: GenericRepository<DrugQuantity>, IDrugQuantityService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DrugQuantityRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
