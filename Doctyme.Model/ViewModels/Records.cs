using System.Collections.Generic;

namespace Doctyme.Model.ViewModels
{
    public class Records
    {
        public int Total { get; set; }
        public List<SearchDrugRecord> SearchDrugRecord { get; set; }
    }
}
