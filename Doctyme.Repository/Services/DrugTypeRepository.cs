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
   public class DrugTypeRepository: GenericRepository<DrugType>, IDrugTypeService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DrugTypeRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
