using System.Collections.Generic;
using System.Linq;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;

namespace Doctyme.Repository.Services
{
    public class DocOrgStateLicensesRepository : GenericRepository<DocOrgStateLicense>, IDocOrgStateLicensesService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DocOrgStateLicensesRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
