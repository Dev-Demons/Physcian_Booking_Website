using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Doctyme.Model.ViewModels
{
    public class CouponModel : BaseModel,IValidatableObject
    {
        public int DrugCouponID { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganizationID { get; set; }
        public string OrganisationName { get; set; }

        [Required(ErrorMessage ="Coupon Code is required")]
        public string CouponCode { get; set; }

        [Required(ErrorMessage = "Discount Type is required")]
        public int CouponDiscountType { get; set; }

        [Required(ErrorMessage = "Discount Amount is required")]
        [Range(1,int.MaxValue,ErrorMessage ="Invalid Discount Amount")]
        public decimal CouponDiscountAmount { get; set; }

        [Required(ErrorMessage ="StartDate is required")]
        public DateTime CouponStartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required")]
        public DateTime CouponEndDate { get; set; }

        public int TotalRecordCount { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (CouponEndDate < CouponStartDate)
            {
                yield return new ValidationResult("EndDate must be greater than StartDate");
            }
        }
    }
}
