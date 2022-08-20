using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface ISlotService
    {
        IEnumerable<Slot> GetAll();
        IEnumerable<Slot> GetAll(Expression<Func<Slot, bool>> predicate, params Expression<Func<Slot, object>>[] includeProperties);
        Slot GetById(object id);
        int GetCount(Expression<Func<Slot, bool>> predicate, params Expression<Func<Slot, object>>[] includeProperties);
        Slot GetSingle(Expression<Func<Slot, bool>> predicate, params Expression<Func<Slot, object>>[] includeProperties);
        void InsertData(Slot model);
        void InsertData(IEnumerable<Slot> model);
        void UpdateData(Slot model);
        void UpdateData(IEnumerable<Slot> model);
        void DeleteData(Slot model);
        void SaveData();

        IList<SpSlotViewModel> GetSlotList(string spName, object[] paraObjects);
    }

}
