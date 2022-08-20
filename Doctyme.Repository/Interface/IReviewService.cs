using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Repository.Interface
{
    public interface IReviewService
    {
        IEnumerable<Review> GetAll();
        IEnumerable<Review> GetAll(Expression<Func<Review, bool>> predicate, params Expression<Func<Review, object>>[] includeProperties);
        Review GetById(object id);
        int GetCount(Expression<Func<Review, bool>> predicate, params Expression<Func<Review, object>>[] includeProperties);
        Review GetSingle(Expression<Func<Review, bool>> predicate, params Expression<Func<Review, object>>[] includeProperties);
        void InsertData(Review model);
        void UpdateData(Review model);
        void DeleteData(Review model);
        void SaveData();
    }
}
