using Binke.Model.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace Binke.Model.DBContext
{
    public class BinkeDbContext : IdentityDbContext<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>
    {
        public BinkeDbContext()
            : base("BinkAppDB")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BinkeDbContext, Configuration>());
        }

        #region DbSet

        public DbSet<Address> Address { get; set; }

        public DbSet<BoardCertification> BoardCertifications { get; set; }

        public DbSet<City> Citys { get; set; }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorBoardCertification> DoctorBoardCertifications { get; set; }
        public DbSet<DoctorFacilityAffiliation> DoctorFacilityAffiliations { get; set; }
        public DbSet<DoctorInsuranceAccepted> DoctorInsuranceAccepteds { get; set; }
        public DbSet<AgeGroup> DoctorAgeGroups { get; set; }
        public DbSet<DoctorImage> DoctorImages { get; set; }
        public DbSet<DoctorLanguage> DoctorLanguages { get; set; }
        public DbSet<DoctorSpeciality> DoctorSpecialitys { get; set; }

        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Experience> Experiences { get; set; }


        public DbSet<Facility> Facilitys { get; set; }
        public DbSet<FacilityType> FacilityTypes { get; set; }
        public DbSet<FacilityOption> FacilityOptions { get; set; }
        public DbSet<FeaturedDoctor> FeaturedDoctors { get; set; }
        public DbSet<FeaturedSpeciality> FeaturedSpecialities { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<OpeningHour> OpeningHours { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<Slot> Slots { get; set; }
        public DbSet<SlotBooking> SlotBookings { get; set; }
        public DbSet<TempSlotBooking> TempSlotBookings { get; set; }
        public DbSet<SocialMedia> SocialMediaLinks { get; set; }
        public DbSet<Speciality> Specialitys { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<DrugDetail> DrugDetails { get; set; }
        public DbSet<Tablet> Tablets { get; set; }
        public DbSet<DrugQuantity> DrugQuantitys { get; set; }
        public DbSet<DrugType> DrugTypes { get; set; }
        public DbSet<DrugManufacturer> DrugManufacturers { get; set; }
        public DbSet<DrugPharmacyDetail> DrugPharmacyDetails { get; set; }

        #endregion

        public static BinkeDbContext Create()
        {
            return new BinkeDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Role>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Role>().Property(x => x.Name).HasMaxLength(20);
            modelBuilder.Entity<UserClaim>().Property(x => x.ClaimType).HasMaxLength(50);
            modelBuilder.Entity<UserClaim>().Property(x => x.ClaimValue).HasMaxLength(200);

            modelBuilder.Entity<ApplicationUser>().Property(x => x.Email).HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.UserName).HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(20);
        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added
                            //|| x.State == EntityState.Deleted
                            || x.State == EntityState.Modified);

            foreach (var entry in modifiedEntries)
            {
                var entityobj = entry.Entity;
                string entityName = entry.Entity.GetType().Name;
                if (!entityobj.HasProperty("CreatedDate") || !entityobj.HasProperty("CreatedBy") || !entityobj.HasProperty("UpdatedDate") || !entityobj.HasProperty("ModifiedBy")) continue;
                var userId = Thread.CurrentPrincipal.Identity.GetUserId<int>();

                var now = DateTime.UtcNow;
                string historyTable = entityName + "History";
                switch (entry.State)
                {
                    case EntityState.Added:
                        {

                            base.Entry(entityobj).Property("CreatedBy").CurrentValue = userId;
                            base.Entry(entityobj).Property("CreatedDate").CurrentValue = now;
                            base.Entry(entityobj).Property("ModifiedBy").IsModified = false;
                            base.Entry(entityobj).Property("UpdatedDate").IsModified = false;
                            break;
                        }
                    case EntityState.Modified:
                        {
                            base.Entry(entityobj).Property("ModifiedBy").CurrentValue = userId;
                            base.Entry(entityobj).Property("UpdatedDate").CurrentValue = now;
                            base.Entry(entityobj).Property("CreatedBy").IsModified = false;
                            base.Entry(entityobj).Property("CreatedDate").IsModified = false;
                            break;
                        }
                    case EntityState.Deleted:
                        {
                            break;
                        }
                }
            }

            return base.SaveChanges();
        }
    }

    public static class CheckCloumn
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }
    }
}
