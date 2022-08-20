using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Repository.Interface
{
   public interface IBlogService
    {
        DataSet GetQueryResult(BlogItem model);
        DataTable GetCategories();
        DataSet GetArticleId(BlogItem model);
        DataTable GetMenuCategories();
    }
}
