using Binke.Model;
using Binke.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface ISeniorCareService
    {
        IEnumerable<SeniorCare> GetAll();
        IEnumerable<SeniorCare> GetAll(Expression<Func<SeniorCare, bool>> predicate, params Expression<Func<SeniorCare, object>>[] includeProperties);
        SeniorCare GetById(object id);
        int GetCount(Expression<Func<SeniorCare, bool>> predicate, params Expression<Func<SeniorCare, object>>[] includeProperties);
        SeniorCare GetSingle(Expression<Func<SeniorCare, bool>> predicate, params Expression<Func<SeniorCare, object>>[] includeProperties);
        void InsertData(SeniorCare model);
        void InsertData(IEnumerable<SeniorCare> model);
        void UpdateData(SeniorCare model);
        void UpdateData(IEnumerable<SeniorCare> model);
        void DeleteData(SeniorCare model);
        void SaveData();
    }

    public interface ISeniorCareImageService
    {
        IEnumerable<SeniorCareImage> GetAll();
        IEnumerable<SeniorCareImage> GetAll(Expression<Func<SeniorCareImage, bool>> predicate, params Expression<Func<SeniorCareImage, object>>[] includeProperties);
        SeniorCareImage GetById(object id);
        int GetCount(Expression<Func<SeniorCareImage, bool>> predicate, params Expression<Func<SeniorCareImage, object>>[] includeProperties);
        SeniorCareImage GetSingle(Expression<Func<SeniorCareImage, bool>> predicate, params Expression<Func<SeniorCareImage, object>>[] includeProperties);
        void InsertData(SeniorCareImage model);
        void InsertData(IEnumerable<SeniorCareImage> model);
        void UpdateData(SeniorCareImage model);
        void UpdateData(IEnumerable<SeniorCareImage> model);
        void DeleteData(SeniorCareImage model);
        void SaveData();
    }

}
