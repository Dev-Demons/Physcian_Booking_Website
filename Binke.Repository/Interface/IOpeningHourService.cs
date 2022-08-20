using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IOpeningHourService
    {
        IEnumerable<OpeningHour> GetAll();
        IEnumerable<OpeningHour> GetAll(Expression<Func<OpeningHour, bool>> predicate, params Expression<Func<OpeningHour, object>>[] includeProperties);
        OpeningHour GetById(object id);
        int GetCount(Expression<Func<OpeningHour, bool>> predicate, params Expression<Func<OpeningHour, object>>[] includeProperties);
        OpeningHour GetSingle(Expression<Func<OpeningHour, bool>> predicate, params Expression<Func<OpeningHour, object>>[] includeProperties);
        void InsertData(OpeningHour model);
        void UpdateData(OpeningHour model);
        void DeleteData(OpeningHour model);
        void SaveData();
    }

}
