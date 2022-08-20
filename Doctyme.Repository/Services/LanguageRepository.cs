using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using System.Collections.Generic;
using System.Linq;

namespace Doctyme.Repository.Services
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public LanguageRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<DrpLanguageModel> GeLanguageDropDownList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<DrpLanguageModel>(spName, paraObjects).ToList();
        }
    }

}
