using System;

namespace Doctyme.Model.ViewModels
{
    public class SpAllTypeViewModel
    {
        public int Id { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int TotalRecords { get; set; }
    }
}
