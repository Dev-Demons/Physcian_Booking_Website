using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDoctorImageService
    {
        IEnumerable<DoctorImage> GetAll();
        IEnumerable<DoctorImage> GetAll(Expression<Func<DoctorImage, bool>> predicate, params Expression<Func<DoctorImage, object>>[] includeProperties);
        DoctorImage GetById(object id);
        int GetCount(Expression<Func<DoctorImage, bool>> predicate, params Expression<Func<DoctorImage, object>>[] includeProperties);
        DoctorImage GetSingle(Expression<Func<DoctorImage, bool>> predicate, params Expression<Func<DoctorImage, object>>[] includeProperties);
        void InsertData(DoctorImage model);
        void InsertData(IEnumerable<DoctorImage> model);
        void UpdateData(DoctorImage model);
        void DeleteData(DoctorImage model);
        void SaveData();
    }
}
