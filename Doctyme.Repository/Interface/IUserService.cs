﻿
using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.SqlClient;
using Doctyme.Model.ViewModels;

namespace Doctyme.Repository.Interface
{
    public interface IUserService
    {
        IEnumerable<ApplicationUser> GetAll();
        IEnumerable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>> predicate, params Expression<Func<ApplicationUser, object>>[] includeProperties);
        ApplicationUser GetById(object id);
        int GetCount(Expression<Func<ApplicationUser, bool>> predicate, params Expression<Func<ApplicationUser, object>>[] includeProperties);
        ApplicationUser GetSingle(Expression<Func<ApplicationUser, bool>> predicate, params Expression<Func<ApplicationUser, object>>[] includeProperties);
        void InsertData(ApplicationUser model);
        void UpdateData(ApplicationUser model);
        void DeleteData(ApplicationUser model);
        void SaveData();

        string GetQuerySingleResult(string query);
        IList<SP_GetUserAddressZipCodeModel> GetUserAddressCityStateZip(string spName, List<SqlParameter> parameters);
    }

}
