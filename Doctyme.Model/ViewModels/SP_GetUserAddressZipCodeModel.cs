using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model.ViewModels
{
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public class SP_GetUserAddressZipCodeModel:ApplicationUser
    {
        public string City { get; set; }
        public string ZipCode { get; set; }

    }
}
