using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface ISiteImageService
    {
        IEnumerable<SiteImage> GetAll();
    }
}
