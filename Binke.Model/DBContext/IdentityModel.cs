using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Binke.Model.DBContext
{
    public class UserRole : IdentityUserRole<int>
    {
    }

    public class UserClaim : IdentityUserClaim<int>
    {
    }

    public class UserLogin : IdentityUserLogin<int>
    {
    }

    public class Role : IdentityRole<int, UserRole>
    {
        public Role() { }
        public Role(string name) { Name = name; }
    }

    public class UserStore : UserStore<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore(BinkeDbContext context)
            : base(context)
        {
        }
    }

    public class RoleStore : RoleStore<Role, int, UserRole>
    {
        public RoleStore(BinkeDbContext context)
            : base(context)
        {
        }
    }

    public class ApplicationUser : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {

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
        public virtual ICollection<Facility> Facilitys { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
        public virtual ICollection<Pharmacy> Pharmacies { get; set; }
        public virtual ICollection<SeniorCare> SeniorCares { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        [NotMapped]
        public string HeaderName { get; set; }

        [NotMapped]
        public string LoginHeader { get; set; }

        public string RegisterViewModel { get; set; }

        public string Uniquekey { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            var currentUser = manager.FindById(userIdentity.GetUserId<int>());
            var userRole = manager.GetRoles(currentUser.Id).FirstOrDefault() ?? "";
            userIdentity.AddClaims(new[]
            {
                new Claim("UserId",Id.ToString()),
                new Claim("UserRole",userRole??""),
                new Claim("FirstName",FirstName??""),
                new Claim("HeaderName",HeaderName??""),
                new Claim("LoginHeader",LoginHeader??""),
                new Claim("ProfilePicture",ProfilePicture??"\\Uploads\\ProfilePic\\doctor.jpg"),
            });
            return userIdentity;
        }

    }
}