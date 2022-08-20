using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.Api
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        public string Prefix { get; set; }

        public string Suffix { get; set; }

        public string FirstName { get; set; }

        public string FullName { get; set; }

        public string ProfilePicture { get; set; }

        public string Email { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public bool IsAllowNewPatient { get; set; }

        public bool IsNtPcp { get; set; }

        public bool IsPrimaryCare { get; set; }

        public string Npi { get; set; }

        public string Education { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public List<short> Speciality { get; set; }
        //public bool IsActive { get; set; }
        public int TotalRows { get; set; }
        
    }
}