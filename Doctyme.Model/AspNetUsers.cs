using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    [Table("AspNetUsers")]
    public class AspNetUsers
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar"), MaxLength(6)]
        public string Prefix { get; set; }

        [Column(TypeName = "varchar"), MaxLength(6)]
        public string Suffix { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string MiddleName { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string LastName { get; set; }

        [Column(TypeName = "varchar"), MaxLength(10)]
        public string Gender { get; set; }

        [NotMapped]
        public string FullName => $@"{FirstName} {LastName}";

        [NotMapped]
        public string FullForDoctor => $@"{Prefix} {FirstName} {MiddleName} {LastName}";

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string ProfilePicture { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Column(TypeName = "varchar"), MaxLength(6)]
        public string PhoneExt { get; set; }

        [Column(TypeName = "varchar"), MaxLength(20)]
        public string FaxNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime LastResetPassword { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; }
        public virtual ICollection<Organisation> Organisations { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        [NotMapped]
        public string HeaderName { get; set; }

        [NotMapped]
        public string LoginHeader { get; set; }

        public string RegisterViewModel { get; set; }

        public string Uniquekey { get; set; }
        public int UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public UserType UserType { get; set; }
        public virtual ICollection<Address> Address { get; set; }
    }
}
