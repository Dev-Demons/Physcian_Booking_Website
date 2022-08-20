using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IStateService
    {
        IEnumerable<State> GetAll();
        IEnumerable<State> GetAll(Expression<Func<State, bool>> predicate, params Expression<Func<State, object>>[] includeProperties);
        State GetById(object id);
        int GetCount(Expression<Func<State, bool>> predicate, params Expression<Func<State, object>>[] includeProperties);
        State GetSingle(Expression<Func<State, bool>> predicate, params Expression<Func<State, object>>[] includeProperties);
        void InsertData(State model);
        void UpdateData(State model);
        void DeleteData(State model);
        void SaveData();
    }

}
