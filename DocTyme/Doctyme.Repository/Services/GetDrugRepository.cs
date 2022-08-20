using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctyme.Model;
using Doctyme.Repository;
using System.Data.SqlClient;
namespace Doctyme.Repository.Services
{
    public class GetDrugRepository:GenericRepository<GetDrugsModal>
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;
        public GetDrugRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public List<GetDrugsModal> getdrugs(string drugname)
        {
         
            if (drugname=="")
            {
                drugname = "''";
            }
            var res = _dbContext.Database.SqlQuery<GetDrugsModal>("exec sp_getdrugdetails "+ drugname).ToList();
            return res;
        }
        public List<getpharmamodel>getpharma(string pharma)
        {
            List<getpharmamodel> res = new List<getpharmamodel>();
            if (pharma=="all")
            {
                res = _dbContext.Database.SqlQuery<getpharmamodel>("exec sp_getpharmadetails " + "''").ToList() ;
            }
            else
            {
                res = _dbContext.Database.SqlQuery<getpharmamodel>("exec sp_getpharmadetails " + pharma).ToList();
            }
            return res;
        }
    }
}
