using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Doctyme.Model;

namespace Binke.ViewModels
{
    public class UserAndAddress : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string Zipcode { get; set; }
        public string Address { get; set; }
    }
}
