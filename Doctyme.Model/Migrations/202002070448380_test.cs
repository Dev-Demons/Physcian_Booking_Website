namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {

        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                {
                    AddressId = c.Int(nullable: false, identity: true),
                    ReferenceId = c.Int(nullable: false),
                    PatientId = c.Int(),
                    Address1 = c.String(maxLength: 100),
                    Address2 = c.String(maxLength: 100),
                    CityId = c.Int(nullable: false),
                    StateId = c.Int(nullable: false),
                    CountryId = c.Int(nullable: false),
                    ZipCode = c.String(maxLength: 10),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    Phone = c.String(),
                    Fax = c.String(),
                    Email = c.String(),
                    WebSite = c.String(),
                    ModifiedBy = c.Int(),
                    IsActive = c.Boolean(nullable: false),
                    Country = c.String(),
                    Lat = c.Decimal(precision: 18, scale: 6),
                    Lon = c.Decimal(precision: 18, scale: 6),
                    CityStateZipCodeID = c.Int(nullable: false),
                    UserTypeID = c.Int(),
                })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Organisation", t => t.ReferenceId, cascadeDelete: true)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.ReferenceId)
                .Index(t => t.PatientId);

            CreateTable(
                "dbo.Organisation",
                c => new
                {
                    OrganisationId = c.Int(nullable: false, identity: true),
                    UserId = c.Int(),
                    OrganizationTypeID = c.Int(nullable: false),
                    OrganisationName = c.String(nullable: false, maxLength: 200, unicode: false),
                    NPI = c.String(maxLength: 10),
                    OrganisationSubpart = c.String(maxLength: 10, fixedLength: true),
                    AliasBusinessName = c.String(maxLength: 200),
                    OrganizatonEIN = c.String(maxLength: 15),
                    EnumerationDate = c.DateTime(storeType: "date"),
                    Status = c.String(nullable: false, maxLength: 10, fixedLength: true),
                    AuthorisedOfficialCredential = c.String(maxLength: 10, fixedLength: true),
                    AuthorizedOfficialFirstName = c.String(maxLength: 50, unicode: false),
                    AuthorizedOfficialLastName = c.String(maxLength: 50, unicode: false),
                    AuthorizedOfficialTelephoneNumber = c.String(maxLength: 10, unicode: false),
                    AuthorizedOfficialTitleOrPosition = c.String(maxLength: 50, unicode: false),
                    AuthorizedOfficialNamePrefix = c.String(maxLength: 10, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    EnabledBooking = c.Boolean(),
                    IsActive = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.OrganisationId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.OrganisationType", t => t.OrganizationTypeID, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.OrganizationTypeID);

            CreateTable(
                "dbo.DoctorAffiliation",
                c => new
                {
                    DoctorAffiliationId = c.Int(nullable: false, identity: true),
                    DoctorId = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DoctorAffiliationId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Organisation", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.OrganisationId);

            CreateTable(
                "dbo.Doctor",
                c => new
                {
                    DoctorId = c.Int(nullable: false),
                    UserId = c.Int(),
                    NamePrefix = c.String(maxLength: 10, unicode: false),
                    Credential = c.String(maxLength: 10, unicode: false),
                    FirstName = c.String(nullable: false, maxLength: 50, unicode: false),
                    LastName = c.String(nullable: false, maxLength: 50, unicode: false),
                    MiddleName = c.String(maxLength: 50, unicode: false),
                    Name = c.String(nullable: false, maxLength: 200, unicode: false),
                    Gender = c.String(maxLength: 10, unicode: false),
                    Status = c.String(nullable: false, maxLength: 10, unicode: false),
                    EnumerationDate = c.DateTime(storeType: "date"),
                    NPI = c.String(maxLength: 10, unicode: false),
                    Education = c.String(maxLength: 50, unicode: false),
                    ShortDescription = c.String(maxLength: 300, unicode: false),
                    LongDescription = c.String(),
                    SoleProprietor = c.Boolean(),
                    IsAllowNewPatient = c.Boolean(nullable: false),
                    IsNtPcp = c.Boolean(nullable: false),
                    Language = c.String(maxLength: 1000, unicode: false),
                    OtherNames = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    IsPrimaryCare = c.Boolean(nullable: false),
                    Keywords = c.String(),
                    PracticeStartDate = c.DateTime(),
                    EnabledBooking = c.Boolean(),
                    IsActive = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.DoctorId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.DoctorAgeGroup",
                c => new
                {
                    DoctorAgeGroupId = c.Int(nullable: false, identity: true),
                    AgeGroupId = c.Int(nullable: false),
                    DoctorId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    IsActive = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.DoctorAgeGroupId)
                .ForeignKey("dbo.AgeGroup", t => t.AgeGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.AgeGroupId)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.AgeGroup",
                c => new
                {
                    AgeGroupId = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    IsActive = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.AgeGroupId);

            CreateTable(
                "dbo.DoctorFacilityAffiliation",
                c => new
                {
                    AffiliationId = c.Int(nullable: false, identity: true),
                    DoctorId = c.Int(nullable: false),
                    FacilityId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.AffiliationId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.DoctorSpeciality",
                c => new
                {
                    DoctorSpecialityId = c.Int(nullable: false, identity: true),
                    SpecialityId = c.Short(nullable: false),
                    DoctorId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    IsPrimary = c.Boolean(),
                    IsSecondary = c.Boolean(),
                })
                .PrimaryKey(t => t.DoctorSpecialityId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Speciality", t => t.SpecialityId, cascadeDelete: true)
                .Index(t => t.SpecialityId)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.Speciality",
                c => new
                {
                    SpecialityId = c.Short(nullable: false, identity: true),
                    SpecialityName = c.String(maxLength: 100, unicode: false),
                    Description = c.String(maxLength: 2000, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    TaxonomyID = c.String(),
                })
                .PrimaryKey(t => t.SpecialityId);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Prefix = c.String(maxLength: 6, unicode: false),
                    Suffix = c.String(maxLength: 6, unicode: false),
                    FirstName = c.String(maxLength: 50, unicode: false),
                    MiddleName = c.String(maxLength: 50, unicode: false),
                    LastName = c.String(maxLength: 50, unicode: false),
                    Gender = c.String(maxLength: 10, unicode: false),
                    ProfilePicture = c.String(maxLength: 50, unicode: false),
                    DateOfBirth = c.DateTime(),
                    PhoneExt = c.String(maxLength: 6, unicode: false),
                    FaxNumber = c.String(maxLength: 20, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    LastLogin = c.DateTime(),
                    LastResetPassword = c.DateTime(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    RegisterViewModel = c.String(),
                    Uniquekey = c.String(),
                    UserTypeId = c.Int(nullable: false),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserType", t => t.UserTypeId, cascadeDelete: true)
                .Index(t => t.UserTypeId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.Int(nullable: false),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Review",
                c => new
                {
                    ReviewId = c.Long(nullable: false, identity: true),
                    ReferenceId = c.Int(),
                    Description = c.String(maxLength: 1000, unicode: false),
                    Rating = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    ApplicationUser_Id = c.Int(),
                    Doctor_DoctorId = c.Int(),
                })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.Doctor", t => t.Doctor_DoctorId)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Doctor_DoctorId);

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    RoleId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.UserType",
                c => new
                {
                    UserTypeId = c.Int(nullable: false, identity: true),
                    UserTypeName = c.String(),
                    Description = c.String(maxLength: 100, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.UserTypeId);

            CreateTable(
                "dbo.Featured",
                c => new
                {
                    FeaturedId = c.Int(nullable: false, identity: true),
                    ReferenceId = c.Int(nullable: false),
                    ProfileImage = c.Int(),
                    Description = c.String(maxLength: 1000, unicode: false),
                    StartDate = c.DateTime(nullable: false, storeType: "date"),
                    EndDate = c.DateTime(storeType: "date"),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.FeaturedId)
                .ForeignKey("dbo.Doctor", t => t.ReferenceId, cascadeDelete: true)
                .Index(t => t.ReferenceId);

            CreateTable(
                "dbo.SocialMedia",
                c => new
                {
                    SocialMediaId = c.Int(nullable: false, identity: true),
                    ReferenceId = c.Int(),
                    Facebook = c.String(maxLength: 215, unicode: false),
                    Twitter = c.String(maxLength: 215, unicode: false),
                    LinkedIn = c.String(maxLength: 215, unicode: false),
                    Instagram = c.String(maxLength: 215, unicode: false),
                    Youtube = c.String(maxLength: 215, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                    Doctor_DoctorId = c.Int(),
                })
                .PrimaryKey(t => t.SocialMediaId)
                .ForeignKey("dbo.Doctor", t => t.Doctor_DoctorId)
                .Index(t => t.Doctor_DoctorId);

            CreateTable(
                "dbo.OrganisationType",
                c => new
                {
                    OrganizationTypeID = c.Int(nullable: false, identity: true),
                    Org_Type_Name = c.String(),
                    Org_Type_Description = c.String(),
                    Org_Type_Taxonomy = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.OrganizationTypeID);

            CreateTable(
                "dbo.AddressType",
                c => new
                {
                    AddressTypeId = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 50, fixedLength: true),
                    Description = c.String(nullable: false, maxLength: 100),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => new { t.AddressTypeId, t.Name, t.Description, t.CreatedDate });

            CreateTable(
                "dbo.AdvertisementLocation",
                c => new
                {
                    AdvertisementLocationId = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200, unicode: false),
                    Description = c.String(maxLength: 1000, unicode: false),
                    CreatedBy = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.AdvertisementLocationId);

            CreateTable(
                "dbo.Advertisement",
                c => new
                {
                    AdvertisementId = c.Int(nullable: false),
                    StartDate = c.DateTime(nullable: false, storeType: "date"),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    ReferenceId = c.Int(),
                    EndDate = c.DateTime(storeType: "date"),
                    AdvertisementLocationId = c.Int(),
                    TotalImpressions = c.Int(),
                    PaymentTypeId = c.Int(),
                    UpdatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                })
                .PrimaryKey(t => t.AdvertisementId)
                .ForeignKey("dbo.AdvertisementLocation", t => t.AdvertisementLocationId)
                .ForeignKey("dbo.PaymentType", t => t.PaymentTypeId)
                .Index(t => t.AdvertisementLocationId)
                .Index(t => t.PaymentTypeId);

            CreateTable(
                "dbo.PaymentType",
                c => new
                {
                    PaymentTypeId = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200, unicode: false),
                    Description = c.String(maxLength: 1000, unicode: false),
                    CreatedBy = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.PaymentTypeId);

            CreateTable(
                "dbo.AmenityOption",
                c => new
                {
                    AmenityId = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 100, unicode: false),
                    Description = c.String(),
                    IsOption = c.Boolean(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.AmenityId);

            CreateTable(
                "dbo.Attachment",
                c => new
                {
                    AttachmentId = c.Int(nullable: false),
                    FilePath = c.String(maxLength: 200, unicode: false),
                    Name = c.String(maxLength: 50, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    Description = c.String(maxLength: 1000, unicode: false),
                    UpdatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                })
                .PrimaryKey(t => t.AttachmentId);

            CreateTable(
                "dbo.BoardCertifications",
                c => new
                {
                    BoardCertificationId = c.Short(nullable: false, identity: true),
                    CertificationName = c.String(maxLength: 100),
                    SpecialityId = c.Short(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.BoardCertificationId);

            CreateTable(
                "dbo.DoctorBoardCertification",
                c => new
                {
                    DoctorBoardCertificationId = c.Int(nullable: false, identity: true),
                    BoardCertificationId = c.Short(nullable: false),
                    DoctorId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DoctorBoardCertificationId)
                .ForeignKey("dbo.BoardCertifications", t => t.BoardCertificationId, cascadeDelete: true)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.BoardCertificationId)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.CityStateZip",
                c => new
                {
                    CityStateZipCodeID = c.Int(nullable: false, identity: true),
                    ZipCode = c.String(maxLength: 10, unicode: false),
                    City = c.String(maxLength: 100, unicode: false),
                    State = c.String(maxLength: 10, unicode: false),
                    LocationType = c.String(maxLength: 50, unicode: false),
                    Lat = c.Decimal(precision: 18, scale: 2),
                    Long = c.Decimal(precision: 18, scale: 2),
                    Xaxis = c.Decimal(precision: 18, scale: 2),
                    Yaxis = c.Decimal(precision: 18, scale: 2),
                    Zaxis = c.Decimal(precision: 18, scale: 2),
                    WorldRegion = c.String(maxLength: 100, unicode: false),
                    Country = c.String(maxLength: 10, unicode: false),
                    LocationText = c.String(maxLength: 200, unicode: false),
                    Location = c.String(maxLength: 100, unicode: false),
                    Decommissioned = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.CityStateZipCodeID);

            CreateTable(
                "dbo.Coupon",
                c => new
                {
                    DrugCouponID = c.Int(nullable: false, identity: true),
                    OrganizationID = c.Int(),
                    CouponCode = c.String(maxLength: 50),
                    CouponDiscountType = c.Int(),
                    CouponDiscountAmount = c.Decimal(storeType: "money"),
                    CouponStartDate = c.DateTime(storeType: "date"),
                    CouponEndDate = c.DateTime(storeType: "date"),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(storeType: "date"),
                    UpdateDate = c.DateTime(storeType: "date"),
                    CreateBy = c.Int(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugCouponID);

            CreateTable(
                "dbo.DocOrgInsurances",
                c => new
                {
                    DocOrgInsuranceId = c.Int(nullable: false, identity: true),
                    ReferenceId = c.Int(nullable: false),
                    InsurancePlanId = c.Int(nullable: false),
                    InsuranceIdentifierId = c.String(maxLength: 20, unicode: false),
                    StateId = c.String(),
                    CreatedDate = c.DateTime(),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedBy = c.Int(),
                    ModifiedBy = c.Int(),
                    UserTypeId = c.Int(),
                })
                .PrimaryKey(t => t.DocOrgInsuranceId)
                .ForeignKey("dbo.InsurancePlan", t => t.InsurancePlanId, cascadeDelete: true)
                .Index(t => t.InsurancePlanId);

            CreateTable(
                "dbo.InsurancePlan",
                c => new
                {
                    InsurancePlanId = c.Int(nullable: false),
                    InsuranceTypeId = c.Int(nullable: false),
                    InsuranceProviderId = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200, unicode: false),
                    Description = c.String(maxLength: 1000, unicode: false),
                    CreatedBy = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.InsurancePlanId)
                .ForeignKey("dbo.InsuranceType", t => t.InsuranceTypeId)
                .Index(t => t.InsuranceTypeId);

            CreateTable(
                "dbo.InsuranceType",
                c => new
                {
                    InsuranceTypeId = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 10, unicode: false),
                    Description = c.String(maxLength: 200, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    UpdatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.InsuranceTypeId);

            CreateTable(
                "dbo.DocOrgStateLicenses",
                c => new
                {
                    DocOrgStateLicense1 = c.Int(nullable: false, identity: true),
                    ReferenceId = c.Int(nullable: false),
                    HealthCareProviderTaxonomyCode = c.String(),
                    ProviderLicenseNumber = c.String(),
                    ProviderLicenseNumberStateCode = c.String(),
                    HealthcareProviderPrimaryTaxonomySwitch = c.Boolean(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(),
                    CreatedBy = c.Int(),
                    UpdatedDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                    UserTypeId = c.Int(),
                })
                .PrimaryKey(t => t.DocOrgStateLicense1);

            CreateTable(
                "dbo.DocOrgTaxonomy",
                c => new
                {
                    DocOrgTaxonomyID = c.Int(nullable: false, identity: true),
                    ReferenceID = c.Int(nullable: false),
                    TaxonomyID = c.Int(nullable: false),
                    UserTypeId = c.Int(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DocOrgTaxonomyID);

            CreateTable(
                "dbo.DoctorGenders",
                c => new
                {
                    DoctorGenderId = c.Int(nullable: false, identity: true),
                    GenderTypeId = c.Int(nullable: false),
                    DoctorId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(),
                    CreatedBy = c.Int(),
                    ModifiedBy = c.Int(),
                    IsActive = c.Boolean(),
                })
                .PrimaryKey(t => t.DoctorGenderId)
                .ForeignKey("dbo.GenderTypes", t => t.GenderTypeId, cascadeDelete: true)
                .Index(t => t.GenderTypeId);

            CreateTable(
                "dbo.DoctorLanguage",
                c => new
                {
                    DoctorLanguageId = c.Int(nullable: false, identity: true),
                    LanguageId = c.Short(nullable: false),
                    DoctorId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DoctorLanguageId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Language", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.LanguageId)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.Language",
                c => new
                {
                    LanguageId = c.Short(nullable: false, identity: true),
                    LanguageName = c.String(maxLength: 100, unicode: false),
                    LanguageCode = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.LanguageId);

            CreateTable(
                "dbo.DrugDetail",
                c => new
                {
                    DrugDetailId = c.Int(nullable: false, identity: true),
                    DrugId = c.Int(nullable: false),
                    DrugTypeId = c.Int(nullable: false),
                    Description = c.String(maxLength: 1000, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugDetailId)
                .ForeignKey("dbo.Drug", t => t.DrugId)
                .ForeignKey("dbo.DrugType", t => t.DrugTypeId)
                .Index(t => t.DrugId)
                .Index(t => t.DrugTypeId);

            CreateTable(
                "dbo.Drug",
                c => new
                {
                    DrugId = c.Int(nullable: false, identity: true),
                    DrugType_LookUpID = c.Int(nullable: false),
                    DrugStatus_LookUpID = c.Int(),
                    DrugName = c.String(nullable: false, maxLength: 200, unicode: false),
                    GenericID = c.Int(),
                    IsGeneric = c.Boolean(),
                    ShortDescription = c.String(maxLength: 300, unicode: false),
                    Description = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedBy = c.Int(),
                    UpdatedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugId);

            CreateTable(
                "dbo.DrugType",
                c => new
                {
                    DrugTypeId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100, unicode: false),
                    Description = c.String(maxLength: 1000, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugTypeId);

            CreateTable(
                "dbo.DrugImage",
                c => new
                {
                    DrugImageID = c.Int(nullable: false, identity: true),
                    DrugID = c.Int(),
                    DrugStrength_LookUpID = c.Int(),
                    ImageFile = c.String(maxLength: 100),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(),
                    CreateBy = c.Int(),
                    UpdatedDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugImageID);

            CreateTable(
                "dbo.DrugManufacturer_LookUp",
                c => new
                {
                    DrugManufacturer_LookUpId = c.Int(nullable: false, identity: true),
                    CompanyName = c.String(),
                    CityStateZipCodeId = c.Int(),
                    CreateDate = c.DateTime(),
                    UpdateDate = c.DateTime(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugManufacturer_LookUpId);

            CreateTable(
                "dbo.DrugManufacturer",
                c => new
                {
                    DrugManufacturerID = c.Int(nullable: false, identity: true),
                    DrugID = c.Int(),
                    DrugManufacturerLookUpID = c.Int(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(),
                    CreatedBy = c.Int(),
                    updateDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugManufacturerID);

            CreateTable(
                "dbo.DrugOrder",
                c => new
                {
                    OrderID = c.Int(nullable: false, identity: true),
                    PatientID = c.Int(),
                    DoctorID = c.Int(),
                    OrganizationID = c.Int(),
                    DrugID = c.Int(),
                    Quantity = c.Decimal(precision: 18, scale: 2),
                    OrderDate = c.DateTime(),
                    CouponID = c.Int(),
                    DrugPrice = c.Decimal(precision: 18, scale: 2),
                    CouponDiscount = c.Decimal(precision: 18, scale: 2),
                    OtherDiscount = c.Decimal(precision: 18, scale: 2),
                    TotalPrice = c.Decimal(precision: 18, scale: 2),
                    NetPrice = c.Decimal(precision: 18, scale: 2),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    UpdateDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.OrderID);

            CreateTable(
                "dbo.DrugStatus_LookUp",
                c => new
                {
                    DrugStatus_LookUpID = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 200),
                    Description = c.String(),
                    IsActive = c.Boolean(),
                    IsADeleted = c.Boolean(),
                    CreatedBy = c.Int(),
                    CreatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                    UpdatedDate = c.DateTime(),
                })
                .PrimaryKey(t => t.DrugStatus_LookUpID);

            CreateTable(
                "dbo.DrugStrength",
                c => new
                {
                    DrugStrengthID = c.Int(nullable: false, identity: true),
                    DrugID = c.Int(),
                    DrugStrengthLookUpID = c.Int(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(),
                    UpdateDate = c.DateTime(),
                    CreateBy = c.Int(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugStrengthID);

            CreateTable(
                "dbo.DrugStrength_LookUp",
                c => new
                {
                    DrugStrength_LookUpID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100),
                    Description = c.String(maxLength: 100),
                    CreatedDate = c.DateTime(),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedBy = c.Int(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugStrength_LookUpID);

            CreateTable(
                "dbo.DrugSymptoms",
                c => new
                {
                    DrugSymptomsID = c.Int(nullable: false, identity: true),
                    DrugID = c.Int(),
                    DrugSymptomsLookUpID = c.Int(),
                    IsMoreCommon = c.Boolean(),
                    IsLessCommon = c.Boolean(),
                    IsNotKnown = c.Boolean(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(),
                    CreatedBy = c.Int(),
                    UpdateDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugSymptomsID);

            CreateTable(
                "dbo.DrugSymptoms_LookUp",
                c => new
                {
                    DrugSymptoms_LookUpID = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 100),
                    Description = c.String(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(),
                    CreatedBy = c.Int(),
                    UpdateDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugSymptoms_LookUpID);

            CreateTable(
                "dbo.DrugTabs",
                c => new
                {
                    DrugTabsID = c.Int(nullable: false, identity: true),
                    DrugID = c.Int(),
                    DrugTabs_LookUpID = c.Int(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDate = c.DateTime(),
                    CreateBy = c.Int(),
                    UpdatedDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugTabsID);

            CreateTable(
                "dbo.DrugTabs_LookUp",
                c => new
                {
                    DrugTabs_LookUpID = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 100),
                    Description = c.String(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedDatee = c.DateTime(),
                    CreateBy = c.Int(),
                    UpdatedDate = c.DateTime(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.DrugTabs_LookUpID);

            CreateTable(
                "dbo.DrugType_LookUp",
                c => new
                {
                    DrugType_LookUpID = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 100),
                    Description = c.String(maxLength: 100),
                    CreatedDate = c.DateTime(),
                    UpdateDate = c.DateTime(),
                    CreateBy = c.Int(),
                    ModifiedBy = c.Int(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                })
                .PrimaryKey(t => t.DrugType_LookUpID);

            CreateTable(
                "dbo.ErrorLogs",
                c => new
                {
                    LogId = c.Long(nullable: false, identity: true),
                    Source = c.String(maxLength: 50),
                    TargetSite = c.String(maxLength: 50),
                    Type = c.String(maxLength: 50),
                    Message = c.String(),
                    Stack = c.String(),
                    InnerExceptionMessage = c.String(),
                    InnerStackTrace = c.String(),
                    LogDate = c.DateTime(nullable: false),
                    AppType = c.String(),
                })
                .PrimaryKey(t => t.LogId);

            CreateTable(
                "dbo.Experience",
                c => new
                {
                    ExperienceId = c.Int(nullable: false, identity: true),
                    DoctorId = c.Int(nullable: false),
                    Designation = c.String(maxLength: 100, unicode: false),
                    Organization = c.String(maxLength: 100, unicode: false),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.ExperienceId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.FeaturedDoctor",
                c => new
                {
                    FeaturedDoctorId = c.Int(nullable: false, identity: true),
                    DoctorId = c.Int(nullable: false),
                    ProfilePicture = c.String(maxLength: 50, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.FeaturedDoctorId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.FeaturedSpeciality",
                c => new
                {
                    FeaturedSpecialityId = c.Int(nullable: false, identity: true),
                    SpecialityId = c.Short(nullable: false),
                    Description = c.String(maxLength: 150, unicode: false),
                    ProfilePicture = c.String(maxLength: 50, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.FeaturedSpecialityId)
                .ForeignKey("dbo.Speciality", t => t.SpecialityId, cascadeDelete: true)
                .Index(t => t.SpecialityId);

            CreateTable(
                "dbo.GenderTypes",
                c => new
                {
                    GenderTypeId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 50),
                    Description = c.String(nullable: false, maxLength: 100),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreatedBy = c.Int(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.GenderTypeId);

            CreateTable(
                "dbo.InsuranceProviders",
                c => new
                {
                    InsProviderId = c.Int(nullable: false, identity: true),
                    InsCompanyName = c.String(maxLength: 200, unicode: false),
                    InsAddress = c.String(maxLength: 200, unicode: false),
                    InsCity = c.String(maxLength: 200, unicode: false),
                    InsState = c.String(maxLength: 10, unicode: false),
                    InsZipCode = c.String(maxLength: 10, unicode: false),
                    InsPhone = c.String(maxLength: 12, unicode: false),
                    InsFax = c.String(maxLength: 10, unicode: false),
                    InsEmail = c.String(maxLength: 200, unicode: false),
                    InsWebSite = c.String(maxLength: 200, unicode: false),
                    SocialMediaId = c.Int(),
                    CreateDate = c.DateTime(),
                    CreateBy = c.Int(),
                    UpdateDate = c.DateTime(),
                    UpdateBy = c.Int(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                })
                .PrimaryKey(t => t.InsProviderId);

            CreateTable(
                "dbo.OpeningHour",
                c => new
                {
                    OpeningHourId = c.Int(nullable: false, identity: true),
                    WeekDay = c.Int(),
                    StartDateTime = c.DateTime(),
                    CalendarDate = c.DateTime(),
                    SlotDuration = c.Int(),
                    IsHoliday = c.Boolean(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    EndDateTime = c.DateTime(),
                    CreatedDate = c.DateTime(),
                    UpdatedBy = c.Int(),
                    CreatedBy = c.Int(),
                    ModifiedBy = c.Int(),
                    ReferenceID = c.Int(),
                })
                .PrimaryKey(t => t.OpeningHourId);

            CreateTable(
                "dbo.OrganizationAmenityOption",
                c => new
                {
                    OrganizationAmenityOptionID = c.Int(nullable: false, identity: true),
                    OrganizationID = c.Int(nullable: false),
                    AmenityOptionID = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.OrganizationAmenityOptionID)
                .ForeignKey("dbo.AmenityOption", t => t.AmenityOptionID, cascadeDelete: true)
                .ForeignKey("dbo.Organisation", t => t.OrganizationID, cascadeDelete: true)
                .Index(t => t.OrganizationID)
                .Index(t => t.AmenityOptionID);

            CreateTable(
                "dbo.PatientHealthCondition",
                c => new
                {
                    PatientHealthConditionID = c.Int(nullable: false, identity: true),
                    PatientID = c.Int(),
                    Name = c.String(maxLength: 100),
                    Description = c.String(),
                    IsAllergy = c.Boolean(),
                    IsHealthCondition = c.Boolean(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    CreateDate = c.DateTime(storeType: "date"),
                    UpdatedDate = c.DateTime(storeType: "date"),
                    CreateBy = c.Int(),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.PatientHealthConditionID);

            CreateTable(
                "dbo.Patient",
                c => new
                {
                    PatientId = c.Int(nullable: false, identity: true),
                    UserId = c.Int(nullable: false),
                    PrimaryInsurance = c.String(maxLength: 100, unicode: false),
                    SecondaryInsurance = c.String(maxLength: 100, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.PatientId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Qualification",
                c => new
                {
                    QualificationId = c.Int(nullable: false, identity: true),
                    Institute = c.String(maxLength: 200, unicode: false),
                    Degree = c.String(maxLength: 100, unicode: false),
                    PassingYear = c.Short(nullable: false),
                    Notes = c.String(maxLength: 200, unicode: false),
                    DoctorId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.QualificationId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.SiteImage",
                c => new
                {
                    SiteImageId = c.Int(nullable: false, identity: true),
                    ReferenceId = c.Int(),
                    UserTypeID = c.Int(),
                    ImagePath = c.String(nullable: false, maxLength: 100, unicode: false),
                    IsProfile = c.Boolean(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.SiteImageId);

            CreateTable(
                "dbo.Slot",
                c => new
                {
                    SlotId = c.Int(nullable: false, identity: true),
                    SlotDate = c.DateTime(nullable: false),
                    SlotTime = c.DateTime(nullable: false),
                    ReferenceId = c.Int(nullable: false),
                    BookedFor = c.Int(),
                    IsBooked = c.Boolean(nullable: false),
                    IsEmailReminder = c.Boolean(nullable: false),
                    IsTextReminder = c.Boolean(nullable: false),
                    IsInsuranceChanged = c.Boolean(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    InsurancePlanId = c.Int(nullable: false),
                    AddressId = c.Int(nullable: false),
                    Description = c.String(maxLength: 1000, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    UpdatedBy = c.Int(),
                })
                .PrimaryKey(t => t.SlotId);

            CreateTable(
                "dbo.Taxonomy",
                c => new
                {
                    TaxonomyID = c.Int(nullable: false, identity: true),
                    Taxonomy_Code = c.String(maxLength: 200, unicode: false),
                    Taxonomy_Type = c.String(maxLength: 200, unicode: false),
                    Taxonomy_Level = c.String(maxLength: 200, unicode: false),
                    Specialization = c.String(unicode: false),
                    Description = c.String(unicode: false),
                    ParentID = c.Int(),
                    CreateDate = c.DateTime(storeType: "date"),
                    UpdateDate = c.DateTime(storeType: "date"),
                    UpdateBy = c.Int(),
                    CreateBy = c.Int(),
                    IsActive = c.Boolean(),
                    IsDeleted = c.Boolean(),
                    SpecialtyText = c.String(unicode: false),
                    IsSpecialty = c.Boolean(),
                })
                .PrimaryKey(t => t.TaxonomyID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Qualification", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Patient", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Address", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.OrganizationAmenityOption", "OrganizationID", "dbo.Organisation");
            DropForeignKey("dbo.OrganizationAmenityOption", "AmenityOptionID", "dbo.AmenityOption");
            DropForeignKey("dbo.DoctorGenders", "GenderTypeId", "dbo.GenderTypes");
            DropForeignKey("dbo.FeaturedSpeciality", "SpecialityId", "dbo.Speciality");
            DropForeignKey("dbo.FeaturedDoctor", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Experience", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DrugDetail", "DrugTypeId", "dbo.DrugType");
            DropForeignKey("dbo.DrugDetail", "DrugId", "dbo.Drug");
            DropForeignKey("dbo.DoctorLanguage", "LanguageId", "dbo.Language");
            DropForeignKey("dbo.DoctorLanguage", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.InsurancePlan", "InsuranceTypeId", "dbo.InsuranceType");
            DropForeignKey("dbo.DocOrgInsurances", "InsurancePlanId", "dbo.InsurancePlan");
            DropForeignKey("dbo.DoctorBoardCertification", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorBoardCertification", "BoardCertificationId", "dbo.BoardCertifications");
            DropForeignKey("dbo.Advertisement", "PaymentTypeId", "dbo.PaymentType");
            DropForeignKey("dbo.Advertisement", "AdvertisementLocationId", "dbo.AdvertisementLocation");
            DropForeignKey("dbo.Address", "ReferenceId", "dbo.Organisation");
            DropForeignKey("dbo.Organisation", "OrganizationTypeID", "dbo.OrganisationType");
            DropForeignKey("dbo.DoctorAffiliation", "OrganisationId", "dbo.Organisation");
            DropForeignKey("dbo.DoctorAffiliation", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.SocialMedia", "Doctor_DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Review", "Doctor_DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Featured", "ReferenceId", "dbo.Doctor");
            DropForeignKey("dbo.AspNetUsers", "UserTypeId", "dbo.UserType");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Review", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Organisation", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Doctor", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DoctorSpeciality", "SpecialityId", "dbo.Speciality");
            DropForeignKey("dbo.DoctorSpeciality", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorFacilityAffiliation", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorAgeGroup", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorAgeGroup", "AgeGroupId", "dbo.AgeGroup");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Qualification", new[] { "DoctorId" });
            DropIndex("dbo.Patient", new[] { "UserId" });
            DropIndex("dbo.OrganizationAmenityOption", new[] { "AmenityOptionID" });
            DropIndex("dbo.OrganizationAmenityOption", new[] { "OrganizationID" });
            DropIndex("dbo.FeaturedSpeciality", new[] { "SpecialityId" });
            DropIndex("dbo.FeaturedDoctor", new[] { "DoctorId" });
            DropIndex("dbo.Experience", new[] { "DoctorId" });
            DropIndex("dbo.DrugDetail", new[] { "DrugTypeId" });
            DropIndex("dbo.DrugDetail", new[] { "DrugId" });
            DropIndex("dbo.DoctorLanguage", new[] { "DoctorId" });
            DropIndex("dbo.DoctorLanguage", new[] { "LanguageId" });
            DropIndex("dbo.DoctorGenders", new[] { "GenderTypeId" });
            DropIndex("dbo.InsurancePlan", new[] { "InsuranceTypeId" });
            DropIndex("dbo.DocOrgInsurances", new[] { "InsurancePlanId" });
            DropIndex("dbo.DoctorBoardCertification", new[] { "DoctorId" });
            DropIndex("dbo.DoctorBoardCertification", new[] { "BoardCertificationId" });
            DropIndex("dbo.Advertisement", new[] { "PaymentTypeId" });
            DropIndex("dbo.Advertisement", new[] { "AdvertisementLocationId" });
            DropIndex("dbo.SocialMedia", new[] { "Doctor_DoctorId" });
            DropIndex("dbo.Featured", new[] { "ReferenceId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Review", new[] { "Doctor_DoctorId" });
            DropIndex("dbo.Review", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "UserTypeId" });
            DropIndex("dbo.DoctorSpeciality", new[] { "DoctorId" });
            DropIndex("dbo.DoctorSpeciality", new[] { "SpecialityId" });
            DropIndex("dbo.DoctorFacilityAffiliation", new[] { "DoctorId" });
            DropIndex("dbo.DoctorAgeGroup", new[] { "DoctorId" });
            DropIndex("dbo.DoctorAgeGroup", new[] { "AgeGroupId" });
            DropIndex("dbo.Doctor", new[] { "UserId" });
            DropIndex("dbo.DoctorAffiliation", new[] { "OrganisationId" });
            DropIndex("dbo.DoctorAffiliation", new[] { "DoctorId" });
            DropIndex("dbo.Organisation", new[] { "OrganizationTypeID" });
            DropIndex("dbo.Organisation", new[] { "UserId" });
            DropIndex("dbo.Address", new[] { "PatientId" });
            DropIndex("dbo.Address", new[] { "ReferenceId" });
            DropTable("dbo.Taxonomy");
            DropTable("dbo.Slot");
            DropTable("dbo.SiteImage");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Qualification");
            DropTable("dbo.Patient");
            DropTable("dbo.PatientHealthCondition");
            DropTable("dbo.OrganizationAmenityOption");
            DropTable("dbo.OpeningHour");
            DropTable("dbo.InsuranceProviders");
            DropTable("dbo.GenderTypes");
            DropTable("dbo.FeaturedSpeciality");
            DropTable("dbo.FeaturedDoctor");
            DropTable("dbo.Experience");
            DropTable("dbo.ErrorLogs");
            DropTable("dbo.DrugType_LookUp");
            DropTable("dbo.DrugTabs_LookUp");
            DropTable("dbo.DrugTabs");
            DropTable("dbo.DrugSymptoms_LookUp");
            DropTable("dbo.DrugSymptoms");
            DropTable("dbo.DrugStrength_LookUp");
            DropTable("dbo.DrugStrength");
            DropTable("dbo.DrugStatus_LookUp");
            DropTable("dbo.DrugOrder");
            DropTable("dbo.DrugManufacturer");
            DropTable("dbo.DrugManufacturer_LookUp");
            DropTable("dbo.DrugImage");
            DropTable("dbo.DrugType");
            DropTable("dbo.Drug");
            DropTable("dbo.DrugDetail");
            DropTable("dbo.Language");
            DropTable("dbo.DoctorLanguage");
            DropTable("dbo.DoctorGenders");
            DropTable("dbo.DocOrgTaxonomy");
            DropTable("dbo.DocOrgStateLicenses");
            DropTable("dbo.InsuranceType");
            DropTable("dbo.InsurancePlan");
            DropTable("dbo.DocOrgInsurances");
            DropTable("dbo.Coupon");
            DropTable("dbo.CityStateZip");
            DropTable("dbo.DoctorBoardCertification");
            DropTable("dbo.BoardCertifications");
            DropTable("dbo.Attachment");
            DropTable("dbo.AmenityOption");
            DropTable("dbo.PaymentType");
            DropTable("dbo.Advertisement");
            DropTable("dbo.AdvertisementLocation");
            DropTable("dbo.AddressType");
            DropTable("dbo.OrganisationType");
            DropTable("dbo.SocialMedia");
            DropTable("dbo.Featured");
            DropTable("dbo.UserType");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Review");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Speciality");
            DropTable("dbo.DoctorSpeciality");
            DropTable("dbo.DoctorFacilityAffiliation");
            DropTable("dbo.AgeGroup");
            DropTable("dbo.DoctorAgeGroup");
            DropTable("dbo.Doctor");
            DropTable("dbo.DoctorAffiliation");
            DropTable("dbo.Organisation");
            DropTable("dbo.Address");
        }
    }
}
