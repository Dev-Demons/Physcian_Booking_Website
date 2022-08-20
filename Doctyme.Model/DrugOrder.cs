using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugOrder")]
    public class DrugOrder 
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }
        public int? PatientID { get; set; }
        public int? DoctorID { get; set; }
        public int? OrganizationID { get; set; }
        public int? DrugID { get; set; }
        
        public decimal? Quantity { get; set; }
       
        public DateTime? OrderDate { get; set; }
        public int? CouponID { get; set; }
        
        public decimal? DrugPrice { get; set; }
       
        public decimal? CouponDiscount { get; set; }
        
        public decimal? OtherDiscount { get; set; }
       
        public decimal? TotalPrice { get; set; }
       
        public decimal? NetPrice { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
       
        public DateTime? UpdateDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}