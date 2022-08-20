using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class NewsletterSubscriberRepository : GenericRepository<NewsletterSubscriber>, INewsletterSubscriberService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public NewsletterSubscriberRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
