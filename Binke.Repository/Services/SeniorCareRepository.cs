using System.Collections.Generic;
using System.Linq;
using Binke.Model;
using Binke.Model.DBContext;
using Binke.Model.ViewModels;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class SeniorCareRepository : GenericRepository<SeniorCare>, ISeniorCareService
    {
        private readonly BinkeDbContext _dbContext;

        public SeniorCareRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

    public class SeniorCareImageRepository : GenericRepository<SeniorCareImage>, ISeniorCareImageService
    {
        private readonly BinkeDbContext _dbContext;

        public SeniorCareImageRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}
