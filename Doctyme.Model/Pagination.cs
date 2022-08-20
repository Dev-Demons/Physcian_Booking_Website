using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    public class Pagination
    {
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string SortColumnName { get; set; }
        public string SortDirection { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }

    }
}
