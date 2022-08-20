using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface ITempSlotBookingService
    {
        IEnumerable<TempSlotBooking> GetAll();
        IEnumerable<TempSlotBooking> GetAll(Expression<Func<TempSlotBooking, bool>> predicate, params Expression<Func<TempSlotBooking, object>>[] includeProperties);
        TempSlotBooking GetById(object id);
        int GetCount(Expression<Func<TempSlotBooking, bool>> predicate, params Expression<Func<TempSlotBooking, object>>[] includeProperties);
        TempSlotBooking GetSingle(Expression<Func<TempSlotBooking, bool>> predicate, params Expression<Func<TempSlotBooking, object>>[] includeProperties);
        void InsertData(TempSlotBooking model);
        void InsertData(IEnumerable<TempSlotBooking> model);
        void UpdateData(TempSlotBooking model);
        void UpdateData(IEnumerable<TempSlotBooking> model);
        void DeleteData(TempSlotBooking model);
        void SaveData();

        IList<SpSlotViewModel> GetSlotList(string spName, object[] paraObjects);
    }

}
