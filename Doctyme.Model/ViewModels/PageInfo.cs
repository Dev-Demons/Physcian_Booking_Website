using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model.ViewModels
{
    public class PageInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Meta> lstMeta { get; set; }
    }
    public class Meta
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Property { get; set; }
        public int PageId { get; set; }
    }
}
