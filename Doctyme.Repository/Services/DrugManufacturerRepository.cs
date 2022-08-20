using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Repository.Services
{
   public class DrugManufacturerRepository: GenericRepository<DrugManufacturer>, IDrugManufacturerService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DrugManufacturerRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
