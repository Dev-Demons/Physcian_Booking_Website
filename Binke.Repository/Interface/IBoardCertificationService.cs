using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IBoardCertificationService
    {
        IEnumerable<BoardCertification> GetAll();
        IEnumerable<BoardCertification> GetAll(Expression<Func<BoardCertification, bool>> predicate, params Expression<Func<BoardCertification, object>>[] includeProperties);
        BoardCertification GetById(object id);
        int GetCount(Expression<Func<BoardCertification, bool>> predicate, params Expression<Func<BoardCertification, object>>[] includeProperties);
        BoardCertification GetSingle(Expression<Func<BoardCertification, bool>> predicate, params Expression<Func<BoardCertification, object>>[] includeProperties);
        void InsertData(BoardCertification model);
        void UpdateData(BoardCertification model);
        void DeleteData(BoardCertification model);
        void SaveData();
    }
    
}
