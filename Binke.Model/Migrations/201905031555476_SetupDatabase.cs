namespace Binke.Model.Migrations
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
                        Address1 = c.String(maxLength: 100, unicode: false),
                        Address2 = c.String(maxLength: 100, unicode: false),
                        CityId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        Country = c.String(maxLength: 20, unicode: false),
                        ZipCode = c.String(maxLength: 10, unicode: false),
                        DoctorId = c.Int(),
                        FacilityId = c.Int(),
                        PatientId = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.City", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.State", t => t.StateId, cascadeDelete: true)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .ForeignKey("dbo.Facility", t => t.FacilityId)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.CityId)
                .Index(t => t.StateId)
                .Index(t => t.DoctorId)
                .Index(t => t.FacilityId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.City",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CityName = c.String(maxLength: 50, unicode: false),
                        StateId = c.Int(),
                        County = c.String(maxLength: 50, unicode: false),
                        Longitude = c.String(maxLength: 20),
                        Latitude = c.String(maxLength: 20),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.CityId)
                .ForeignKey("dbo.State", t => t.StateId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.State",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        StateCode = c.String(maxLength: 5, unicode: false),
                        StateName = c.String(maxLength: 50, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.StateId);
            
            CreateTable(
                "dbo.Doctor",
                c => new
                    {
                        DoctorId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        NPI = c.Int(nullable: false),
                        Education = c.String(),
                        ShortDescription = c.String(maxLength: 300, unicode: false),
                        LongDescription = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DoctorId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
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
                .ForeignKey("dbo.BoardCertification", t => t.BoardCertificationId, cascadeDelete: true)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.BoardCertificationId)
                .Index(t => t.DoctorId);
            
            CreateTable(
                "dbo.BoardCertification",
                c => new
                    {
                        BoardCertificationId = c.Short(nullable: false, identity: true),
                        CertificationName = c.String(maxLength: 100, unicode: false),
                        SpecialityId = c.Short(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.BoardCertificationId)
                .ForeignKey("dbo.Speciality", t => t.SpecialityId, cascadeDelete: true)
                .Index(t => t.SpecialityId);
            
            CreateTable(
                "dbo.Speciality",
                c => new
                    {
                        SpecialityId = c.Short(nullable: false, identity: true),
                        SpecialityName = c.String(maxLength: 100, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.SpecialityId);
            
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
                "dbo.DoctorFacilityAffiliation",
                c => new
                    {
                        AffiliationId = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        FacilityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AffiliationId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Facility", t => t.FacilityId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.FacilityId);
            
            CreateTable(
                "dbo.Facility",
                c => new
                    {
                        FacilityId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        FacilityTypeId = c.Byte(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FacilityId)
                .ForeignKey("dbo.FacilityType", t => t.FacilityTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.FacilityTypeId);
            
            CreateTable(
                "dbo.FacilityType",
                c => new
                    {
                        FacilityTypeId = c.Byte(nullable: false, identity: true),
                        FacilityTypeName = c.String(maxLength: 50, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FacilityTypeId);
            
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
                        Email = c.String(maxLength: 100),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(maxLength: 12),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(maxLength: 50),
                        ClaimValue = c.String(maxLength: 200),
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
                "dbo.Patient",
                c => new
                    {
                        PatientId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        NPI = c.Int(nullable: false),
                        PrimaryInsurance = c.String(maxLength: 100, unicode: false),
                        SecondaryInsurance = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.PatientId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
                        LanguageName = c.String(maxLength: 20, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.LanguageId);
            
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
                        City = c.String(maxLength: 20, unicode: false),
                        State = c.String(maxLength: 30, unicode: false),
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
                "dbo.OpeningHour",
                c => new
                    {
                        OpeningHourId = c.Int(nullable: false, identity: true),
                        SlotId = c.Int(nullable: false),
                        DoctorId = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.OpeningHourId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .ForeignKey("dbo.Slot", t => t.SlotId, cascadeDelete: true)
                .Index(t => t.SlotId)
                .Index(t => t.DoctorId);
            
            CreateTable(
                "dbo.Slot",
                c => new
                    {
                        SlotId = c.Int(nullable: false, identity: true),
                        SlotTime = c.Time(nullable: false, precision: 7),
                        DoctorId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);
            
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
                "dbo.SocialMedia",
                c => new
                    {
                        SocialMediaId = c.Int(nullable: false, identity: true),
                        Facebook = c.String(maxLength: 215, unicode: false),
                        Twitter = c.String(maxLength: 215, unicode: false),
                        LinkedIn = c.String(maxLength: 215, unicode: false),
                        Instagram = c.String(maxLength: 215, unicode: false),
                        DoctorId = c.Int(),
                    })
                .PrimaryKey(t => t.SocialMediaId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .Index(t => t.DoctorId);
            
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
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.SocialMedia", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Qualification", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.OpeningHour", "SlotId", "dbo.Slot");
            DropForeignKey("dbo.Slot", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.OpeningHour", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Experience", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorLanguage", "LanguageId", "dbo.Language");
            DropForeignKey("dbo.DoctorLanguage", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Patient", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Address", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Facility", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Doctor", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Facility", "FacilityTypeId", "dbo.FacilityType");
            DropForeignKey("dbo.DoctorFacilityAffiliation", "FacilityId", "dbo.Facility");
            DropForeignKey("dbo.Address", "FacilityId", "dbo.Facility");
            DropForeignKey("dbo.DoctorFacilityAffiliation", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorBoardCertification", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorSpeciality", "SpecialityId", "dbo.Speciality");
            DropForeignKey("dbo.DoctorSpeciality", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.BoardCertification", "SpecialityId", "dbo.Speciality");
            DropForeignKey("dbo.DoctorBoardCertification", "BoardCertificationId", "dbo.BoardCertification");
            DropForeignKey("dbo.Address", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.City", "StateId", "dbo.State");
            DropForeignKey("dbo.Address", "StateId", "dbo.State");
            DropForeignKey("dbo.Address", "CityId", "dbo.City");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SocialMedia", new[] { "DoctorId" });
            DropIndex("dbo.Qualification", new[] { "DoctorId" });
            DropIndex("dbo.Slot", new[] { "DoctorId" });
            DropIndex("dbo.OpeningHour", new[] { "DoctorId" });
            DropIndex("dbo.OpeningHour", new[] { "SlotId" });
            DropIndex("dbo.Experience", new[] { "DoctorId" });
            DropIndex("dbo.DoctorLanguage", new[] { "DoctorId" });
            DropIndex("dbo.DoctorLanguage", new[] { "LanguageId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Patient", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Facility", new[] { "FacilityTypeId" });
            DropIndex("dbo.Facility", new[] { "UserId" });
            DropIndex("dbo.DoctorFacilityAffiliation", new[] { "FacilityId" });
            DropIndex("dbo.DoctorFacilityAffiliation", new[] { "DoctorId" });
            DropIndex("dbo.DoctorSpeciality", new[] { "DoctorId" });
            DropIndex("dbo.DoctorSpeciality", new[] { "SpecialityId" });
            DropIndex("dbo.BoardCertification", new[] { "SpecialityId" });
            DropIndex("dbo.DoctorBoardCertification", new[] { "DoctorId" });
            DropIndex("dbo.DoctorBoardCertification", new[] { "BoardCertificationId" });
            DropIndex("dbo.Doctor", new[] { "UserId" });
            DropIndex("dbo.City", new[] { "StateId" });
            DropIndex("dbo.Address", new[] { "PatientId" });
            DropIndex("dbo.Address", new[] { "FacilityId" });
            DropIndex("dbo.Address", new[] { "DoctorId" });
            DropIndex("dbo.Address", new[] { "StateId" });
            DropIndex("dbo.Address", new[] { "CityId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ErrorLogs");
            DropTable("dbo.SocialMedia");
            DropTable("dbo.Qualification");
            DropTable("dbo.Slot");
            DropTable("dbo.OpeningHour");
            DropTable("dbo.Experience");
            DropTable("dbo.Language");
            DropTable("dbo.DoctorLanguage");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Patient");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.FacilityType");
            DropTable("dbo.Facility");
            DropTable("dbo.DoctorFacilityAffiliation");
            DropTable("dbo.DoctorSpeciality");
            DropTable("dbo.Speciality");
            DropTable("dbo.BoardCertification");
            DropTable("dbo.DoctorBoardCertification");
            DropTable("dbo.Doctor");
            DropTable("dbo.State");
            DropTable("dbo.City");
            DropTable("dbo.Address");
        }
    }
}
