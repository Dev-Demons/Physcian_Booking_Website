using Binke.Model.DBContext;
using Binke.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Binke.Repository.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BinkeDbContext _dbContext;
        private readonly DbSet<T> _table;

        public GenericRepository(BinkeDbContext dbcontext)
        {
            _dbContext = dbcontext;
            _table = _dbContext.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return _table.AsEnumerable(); //.AsNoTracking(); //
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.Where(predicate).AsEnumerable();
        }
        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.SingleOrDefault(predicate);
        }

        public T GetById(object id)
        {
            return _table.Find(id);
        }
        public int GetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.Count(predicate);
        }
        public void InsertData(T obj)
        {
            _table.Add(obj);
        }
        public void InsertData(IEnumerable<T> obj)
        {
            _table.AddRange(obj);
        }
        public void UpdateData(T obj)
        {
            _table.Attach(obj);
            _dbContext.Entry(obj).State = EntityState.Modified;
        }
        public void UpdateData(IEnumerable<T> obj)
        {
            foreach (var item in obj)
            {
                _table.Attach(item);
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        public void DeleteData(T obj)
        {
            _table.Remove(obj);
        }
        public void DeleteData(IEnumerable<T> obj)
        {
            _table.RemoveRange(obj);
        }
        public void SaveData()
        {
            _dbContext.SaveChanges();
        }

    }
}
