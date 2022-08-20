using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Binke.Model;

namespace Binke.Repository.Interface
{
    public interface IErrorLogService
    {
        IEnumerable<ErrorLog> GetAll();
        IEnumerable<ErrorLog> GetAll(Expression<Func<ErrorLog, bool>> predicate, params Expression<Func<ErrorLog, object>>[] includeProperties);
        ErrorLog GetById(object id);
        ErrorLog GetSingle(Expression<Func<ErrorLog, bool>> predicate, params Expression<Func<ErrorLog, object>>[] includeProperties);
        void InsertData(ErrorLog model);
        void UpdateData(ErrorLog model);
        void DeleteData(ErrorLog model);
        void DeleteData(IEnumerable<ErrorLog> model);
        void SaveData();
    }

}
