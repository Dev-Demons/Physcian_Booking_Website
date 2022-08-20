using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IBoardCertificationService
    {
        IEnumerable<BoardCertifications> GetAll();
        IEnumerable<BoardCertifications> GetAll(Expression<Func<BoardCertifications, bool>> predicate, params Expression<Func<BoardCertifications, object>>[] includeProperties);
        BoardCertifications GetById(object id);
        int GetCount(Expression<Func<BoardCertifications, bool>> predicate, params Expression<Func<BoardCertifications, object>>[] includeProperties);
        BoardCertifications GetSingle(Expression<Func<BoardCertifications, bool>> predicate, params Expression<Func<BoardCertifications, object>>[] includeProperties);
        void InsertData(BoardCertifications model);
        void UpdateData(BoardCertifications model);
        void DeleteData(BoardCertifications model);
        void SaveData();
    }
    
}
