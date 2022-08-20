namespace Doctyme.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Doctyme.Model.Migrations;

    public partial class DoctymeDbContext : IdentityDbContext<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>
    {
        public DoctymeDbContext()
            : base("name=Doctyme")
        {
            Database.SetInitializer<DoctymeDbContext>(null);
        }

        public static DoctymeDbContext Create()
        {
            return new DoctymeDbContext();
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AdvertisementLocation> AdvertisementLocations { get; set; }
        public virtual DbSet<AgeGroup> AgeGroups { get; set; }
        public virtual DbSet<AmenityOption> AmenityOptions { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        //public virtual DbSet<City> Cities { get; set; }
        //public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<DoctorAffiliation> DoctorAffiliations { get; set; }
        public virtual DbSet<DoctorAgeGroup> DoctorAgeGroups { get; set; }
        public virtual DbSet<DoctorSpeciality> DoctorSpecialities { get; set; }
        public virtual DbSet<Drug> Drugs { get; set; }
        public virtual DbSet<DrugDetail> DrugDetails { get; set; }
        public virtual DbSet<DrugType> DrugTypes { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<Featured> Featureds { get; set; }
        public virtual DbSet<InsurancePlan> InsurancePlans { get; set; }
        public virtual DbSet<InsuranceType> InsuranceTypes { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<SiteImage> SiteImages { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<SocialMedia> SocialMedias { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        //public virtual DbSet<State> States { get; set; }
        public virtual DbSet<CityStateZip> CityStateZips { get; set; }
        public virtual DbSet<DocOrgTaxonomy> DocOrgTaxonomis { get; set; }
        public virtual DbSet<Taxonomy> Taxonomies { get; set; }
        public virtual DbSet<AddressType> AddressPurposes { get; set; }
        public virtual DbSet<Advertisement> Advertisements { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<DoctorLanguage> DoctorLanguages { get; set; }
        public virtual DbSet<Experience> Experiences { get; set; }
        public virtual DbSet<DoctorBoardCertification> DoctorBoardCertifications { get; set; }
        public virtual DbSet<BoardCertifications> BoardCertifications { get; set; }
        public virtual DbSet<DrugManufacturer> DrugManufacturers { get; set; }
        public virtual DbSet<OpeningHour> OpeningHours { get; set; }
        public virtual DbSet<OrganizationAmenityOption> OrganizationAmenityOptions { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<FeaturedSpeciality> FeaturedSpeciality { get; set; }
        public virtual DbSet<FeaturedDoctor> FeaturedDoctor { get; set; }
        public virtual DbSet<Qualification> Qualification { get; set; }
        public virtual DbSet<InsuranceProvider> InsuranceProvider { get; set; }
        public virtual DbSet<DocOrgStateLicense> DocOrgStateLicense { get; set; }
        public virtual DbSet<DocOrgInsurances> DocOrgInsurance { get; set; }
        //public virtual DbSet<DoctorAffiliation> DoctorAffiliation { get; set; }
        public virtual DbSet<GenderType> GenderType { get; set; }
        public virtual DbSet<DoctorGender> DoctorGender { get; set; }

        public virtual DbSet<Coupon> Coupon { get; set; }
       // public virtual DbSet<Drug> Drug { get; set; }
        public virtual DbSet<DrugImage> DrugImage { get; set; }
       // public virtual DbSet<DrugManufacturer> DrugManufacturer { get; set; }
        public virtual DbSet<DrugManufacturer_LookUp> DrugManufacturer_LookUp { get; set; }
        public virtual DbSet<DrugOrder> DrugOrder { get; set; }
        public virtual DbSet<DrugStatus_LookUp> DrugStatus_LookUp { get; set; }
        public virtual DbSet<DrugStrength> DrugStrength { get; set; }
        public virtual DbSet<DrugStrength_LookUp> DrugStrength_LookUp { get; set; }
        public virtual DbSet<DrugSymptoms> DrugSymptoms { get; set; }
        public virtual DbSet<DrugSymptoms_LookUp> DrugSymptoms_LookUp { get; set; }
        public virtual DbSet<DrugTabs> DrugTabs { get; set; }
        public virtual DbSet<DrugTabs_LookUp> DrugTabs_LookUp { get; set; }
        public virtual DbSet<DrugType_LookUp> DrugType_LookUp { get; set; }
        public virtual DbSet<PatientHealthCondition> PatientHealthCondition { get; set; }
        public virtual DbSet<NewsletterSubscriber> NewsletterSubscriber { get; set; }
        public virtual DbSet<DocOrgPatient> DocOrgPatients { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.Entity<Address>()
                .Property(e => e.Lat)
                .HasPrecision(18, 6);
            modelBuilder.Entity<Address>()
          .Property(e => e.Lon)
          .HasPrecision(18, 6);

            //modelBuilder.Entity<Address>()
            //    .Property(e => e.Address2)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Address>()
            //    .Property(e => e.ZipCode)
            //    .IsUnicode(false);

            modelBuilder.Entity<AdvertisementLocation>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<AdvertisementLocation>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<AmenityOption>()
                .Property(e => e.Name)
                .IsUnicode(false);
            modelBuilder.Entity<Qualification>()
            .Property(e => e.Degree)
            .IsUnicode(false);
            //modelBuilder.Entity<City>()
            //    .Property(e => e.CityName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<City>()
            //    .Property(e => e.County)
            //    .IsUnicode(false);

            //modelBuilder.Entity<City>()
            //    .HasMany(e => e.Addresses)
            //    .WithRequired(e => e.City)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Country>()
            //    .Property(e => e.CountryCode)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Country>()
            //    .Property(e => e.CountryName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Country>()
            //    .HasMany(e => e.Addresses)
            //    .WithRequired(e => e.Country)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.NamePrefix)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.Credential)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.Gender)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.NPI)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.Education)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.ShortDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.Featureds)
                .WithRequired(e => e.Doctor)
                .HasForeignKey(e => e.ReferenceId);

            modelBuilder.Entity<Drug>()
                .Property(e => e.DrugName)
                .IsUnicode(false);

            modelBuilder.Entity<Drug>()
                .Property(e => e.ShortDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Drug>()
                .HasMany(e => e.DrugDetails)
                .WithRequired(e => e.Drug)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DrugDetail>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DrugType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DrugType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DrugType>()
                .HasMany(e => e.DrugDetails)
                .WithRequired(e => e.DrugType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Featured>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<InsurancePlan>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<InsurancePlan>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceType>()
                .HasMany(e => e.InsurancePlans)
                .WithRequired(e => e.InsuranceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .Property(e => e.OrganizationTypeID);
            //.IsUnicode(false);

            modelBuilder.Entity<Organisation>()
                .Property(e => e.OrganisationName)
                .IsUnicode(false);

            modelBuilder.Entity<Organisation>()
                .Property(e => e.OrganisationSubpart)
                .IsFixedLength();

            modelBuilder.Entity<Organisation>()
                .Property(e => e.Status)
                .IsFixedLength();

            modelBuilder.Entity<Organisation>()
                .Property(e => e.AuthorisedOfficialCredential)
                .IsFixedLength();

            modelBuilder.Entity<Organisation>()
                .Property(e => e.AuthorizedOfficialFirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Organisation>()
                .Property(e => e.AuthorizedOfficialLastName)
                .IsUnicode(false);

            modelBuilder.Entity<Organisation>()
                .Property(e => e.AuthorizedOfficialTelephoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Organisation>()
                .Property(e => e.AuthorizedOfficialTitleOrPosition)
                .IsUnicode(false);

            modelBuilder.Entity<Organisation>()
                .Property(e => e.AuthorizedOfficialNamePrefix)
                .IsUnicode(false);

            modelBuilder.Entity<PaymentType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<PaymentType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Review>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<SiteImage>()
                .Property(e => e.ImagePath)
                .IsUnicode(false);

            modelBuilder.Entity<Slot>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .Property(e => e.Facebook)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .Property(e => e.Twitter)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .Property(e => e.LinkedIn)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .Property(e => e.Instagram)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .Property(e => e.Youtube)
                .IsUnicode(false);

            modelBuilder.Entity<Speciality>()
                .Property(e => e.SpecialityName)
                .IsUnicode(false);

            modelBuilder.Entity<Speciality>()
                .Property(e => e.Description)
                .IsUnicode(false);

            //modelBuilder.Entity<State>()
            //    .Property(e => e.StateCode)
            //    .IsUnicode(false);

            //modelBuilder.Entity<State>()
            //    .Property(e => e.StateName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<State>()
            //    .HasMany(e => e.Addresses)
            //    .WithRequired(e => e.State)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<State>()
            //    .HasMany(e => e.Cities)
            //    .WithRequired(e => e.State)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Taxonomy>()
                .Property(e => e.Taxonomy_Code)
                .IsUnicode(false);

            modelBuilder.Entity<Taxonomy>()
                .Property(e => e.Taxonomy_Type)
                .IsUnicode(false);

            modelBuilder.Entity<Taxonomy>()
                .Property(e => e.Taxonomy_Level)
                .IsUnicode(false);

            modelBuilder.Entity<Taxonomy>()
                .Property(e => e.Specialization)
                .IsUnicode(false);

            modelBuilder.Entity<Taxonomy>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<AddressType>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<Attachment>()
                .Property(e => e.FilePath)
                .IsUnicode(false);

            modelBuilder.Entity<Attachment>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Attachment>()
                .Property(e => e.Description)
                .IsUnicode(false);

        }
    }
}
