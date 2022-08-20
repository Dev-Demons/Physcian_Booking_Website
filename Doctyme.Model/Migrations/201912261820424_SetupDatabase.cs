namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(nullable: false),
                        Address1 = c.String(maxLength: 100, unicode: false),
                        Address2 = c.String(maxLength: 100, unicode: false),
                        CityId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                        ZipCode = c.String(maxLength: 10, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.City", t => t.CityId)
                .ForeignKey("dbo.State", t => t.StateId)
                .ForeignKey("dbo.Country", t => t.CountryId)
                .ForeignKey("dbo.Organisation", t => t.ReferenceId, cascadeDelete: true)
                .Index(t => t.ReferenceId)
                .Index(t => t.CityId)
                .Index(t => t.StateId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.City",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CityName = c.String(nullable: false, maxLength: 50, unicode: false),
                        StateId = c.Int(nullable: false),
                        County = c.String(maxLength: 50, unicode: false),
                        Longitude = c.String(maxLength: 20),
                        Latitude = c.String(maxLength: 20),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CityId)
                .ForeignKey("dbo.State", t => t.StateId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.State",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        StateCode = c.String(nullable: false, maxLength: 5, unicode: false),
                        StateName = c.String(nullable: false, maxLength: 50, unicode: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.StateId);
            
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        CountryCode = c.String(nullable: false, maxLength: 5, unicode: false),
                        CountryName = c.String(nullable: false, maxLength: 50, unicode: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Organisation",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        OrganizationTypeID = c.Int(nullable: false),
                        OrganisationName = c.String(nullable: false, maxLength: 200, unicode: false),
                        OrganisationSubpart = c.String(maxLength: 10, fixedLength: true),
                        EnumerationDate = c.DateTime(storeType: "date"),
                        Status = c.String(nullable: false, maxLength: 10, fixedLength: true),
                        AuthorisedOfficialCredential = c.String(maxLength: 10, fixedLength: true),
                        AuthorizedOfficialFirstName = c.String(maxLength: 50, unicode: false),
                        AuthorizedOfficialLastName = c.String(maxLength: 50, unicode: false),
                        AuthorizedOfficialTelephoneNumber = c.String(maxLength: 10, unicode: false),
                        AuthorizedOfficialTitleOrPosition = c.String(maxLength: 10, unicode: false),
                        AuthorizedOfficialNamePrefix = c.String(maxLength: 10, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
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
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
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
                "dbo.OrganisationType",
                c => new
                    {
                        OrganizationTypeID = c.Int(nullable: false, identity: true),
                        Org_Type_Name = c.String(),
                        Org_Type_Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.OrganizationTypeID);
            
            CreateTable(
                "dbo.AddressPurpose",
                c => new
                    {
                        AddressPurposeId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 10, fixedLength: true),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => new { t.AddressPurposeId, t.Name, t.CreatedDate, t.CreatedBy });
            
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
                .PrimaryKey(t => new { t.AdvertisementId, t.StartDate, t.CreatedDate, t.CreatedBy, t.IsActive, t.IsDeleted })
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
                "dbo.Amenity",
                c => new
                    {
                        AmenityId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
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
                        FilePath = c.String(nullable: false, maxLength: 200, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 1000, unicode: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => new { t.AttachmentId, t.FilePath, t.Name, t.CreatedDate, t.CreatedBy, t.IsActive, t.IsDeleted });
            
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
                        DrugName = c.String(maxLength: 100, unicode: false),
                        ShortDescription = c.String(maxLength: 200, unicode: false),
                        Description = c.String(),
                        ManufactureId = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.SocialMediaId);
            
            CreateTable(
                "dbo.Taxonomy",
                c => new
                    {
                        TaxonomyID = c.Int(nullable: false, identity: true),
                        Taxonomy_Code = c.String(maxLength: 200, unicode: false),
                        Type = c.String(maxLength: 200, unicode: false),
                        Level = c.String(maxLength: 200, unicode: false),
                        Specialization = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        ParentID = c.Int(),
                        CreateDate = c.DateTime(storeType: "date"),
                        UpdateDate = c.DateTime(storeType: "date"),
                        UpdateBy = c.Int(),
                        CreateBy = c.Int(),
                        IsActive = c.Boolean(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.TaxonomyID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.InsurancePlan", "InsuranceTypeId", "dbo.InsuranceType");
            DropForeignKey("dbo.DrugDetail", "DrugTypeId", "dbo.DrugType");
            DropForeignKey("dbo.DrugDetail", "DrugId", "dbo.Drug");
            DropForeignKey("dbo.Advertisement", "PaymentTypeId", "dbo.PaymentType");
            DropForeignKey("dbo.Advertisement", "AdvertisementLocationId", "dbo.AdvertisementLocation");
            DropForeignKey("dbo.Address", "ReferenceId", "dbo.Organisation");
            DropForeignKey("dbo.Organisation", "OrganizationTypeID", "dbo.OrganisationType");
            DropForeignKey("dbo.DoctorAffiliation", "OrganisationId", "dbo.Organisation");
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
            DropForeignKey("dbo.DoctorAgeGroup", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorAgeGroup", "AgeGroupId", "dbo.AgeGroup");
            DropForeignKey("dbo.DoctorAffiliation", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Address", "CountryId", "dbo.Country");
            DropForeignKey("dbo.City", "StateId", "dbo.State");
            DropForeignKey("dbo.Address", "StateId", "dbo.State");
            DropForeignKey("dbo.Address", "CityId", "dbo.City");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.InsurancePlan", new[] { "InsuranceTypeId" });
            DropIndex("dbo.DrugDetail", new[] { "DrugTypeId" });
            DropIndex("dbo.DrugDetail", new[] { "DrugId" });
            DropIndex("dbo.Advertisement", new[] { "PaymentTypeId" });
            DropIndex("dbo.Advertisement", new[] { "AdvertisementLocationId" });
            DropIndex("dbo.Featured", new[] { "ReferenceId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Review", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "UserTypeId" });
            DropIndex("dbo.DoctorSpeciality", new[] { "DoctorId" });
            DropIndex("dbo.DoctorSpeciality", new[] { "SpecialityId" });
            DropIndex("dbo.DoctorAgeGroup", new[] { "DoctorId" });
            DropIndex("dbo.DoctorAgeGroup", new[] { "AgeGroupId" });
            DropIndex("dbo.Doctor", new[] { "UserId" });
            DropIndex("dbo.DoctorAffiliation", new[] { "OrganisationId" });
            DropIndex("dbo.DoctorAffiliation", new[] { "DoctorId" });
            DropIndex("dbo.Organisation", new[] { "OrganizationTypeID" });
            DropIndex("dbo.Organisation", new[] { "UserId" });
            DropIndex("dbo.City", new[] { "StateId" });
            DropIndex("dbo.Address", new[] { "CountryId" });
            DropIndex("dbo.Address", new[] { "StateId" });
            DropIndex("dbo.Address", new[] { "CityId" });
            DropIndex("dbo.Address", new[] { "ReferenceId" });
            DropTable("dbo.Taxonomy");
            DropTable("dbo.SocialMedia");
            DropTable("dbo.Slot");
            DropTable("dbo.SiteImage");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.InsuranceType");
            DropTable("dbo.InsurancePlan");
            DropTable("dbo.ErrorLogs");
            DropTable("dbo.DrugType");
            DropTable("dbo.Drug");
            DropTable("dbo.DrugDetail");
            DropTable("dbo.Attachment");
            DropTable("dbo.Amenity");
            DropTable("dbo.PaymentType");
            DropTable("dbo.Advertisement");
            DropTable("dbo.AdvertisementLocation");
            DropTable("dbo.AddressPurpose");
            DropTable("dbo.OrganisationType");
            DropTable("dbo.Featured");
            DropTable("dbo.UserType");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Review");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Speciality");
            DropTable("dbo.DoctorSpeciality");
            DropTable("dbo.AgeGroup");
            DropTable("dbo.DoctorAgeGroup");
            DropTable("dbo.Doctor");
            DropTable("dbo.DoctorAffiliation");
            DropTable("dbo.Organisation");
            DropTable("dbo.Country");
            DropTable("dbo.State");
            DropTable("dbo.City");
            DropTable("dbo.Address");
        }
    }
}
