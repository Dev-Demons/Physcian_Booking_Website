using System.Collections.Generic;
using System.Linq;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class DoctorInsuranceRepository : GenericRepository<DocOrgInsurances>, IDoctorInsuranceService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorInsuranceRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<DrpKeyValueModel> GetDrpInsuranceList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<DrpKeyValueModel>(spName, paraObjects).ToList();
        }
    }
}
