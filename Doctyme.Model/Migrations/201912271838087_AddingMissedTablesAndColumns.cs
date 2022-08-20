namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingMissedTablesAndColumns : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Amenity", newName: "AmenityOption");
            DropForeignKey("dbo.Address", "CityId", "dbo.City");
            DropForeignKey("dbo.Address", "StateId", "dbo.State");
            DropForeignKey("dbo.City", "StateId", "dbo.State");
            DropForeignKey("dbo.Address", "CountryId", "dbo.Country");
            DropIndex("dbo.Address", new[] { "CityId" });
            DropIndex("dbo.Address", new[] { "StateId" });
            DropIndex("dbo.Address", new[] { "CountryId" });
            DropIndex("dbo.City", new[] { "StateId" });
            CreateTable(
                "dbo.CityStateZip",
                c => new
                    {
                        CityStateZipCodeID = c.Int(nullable: false, identity: true),
                        ZipCode = c.String(),
                        City = c.String(),
                        State = c.String(),
                        LocationType = c.String(),
                        Lat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Long = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Xaxis = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Yaxis = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Zaxis = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorldRegion = c.String(),
                        Country = c.String(),
                        LocationText = c.String(),
                        Location = c.String(),
                        Decommissioned = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CityStateZipCodeID);
            
            CreateTable(
                "dbo.DocOrgTaxonomy",
                c => new
                    {
                        DocOrgTaxonomyID = c.Int(nullable: false, identity: true),
                        ReferenceID = c.Int(nullable: false),
                        TaxonomyID = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DocOrgTaxonomyID);
            
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
                "dbo.DoctorInsurance",
                c => new
                    {
                        DoctorInsuranceID = c.Int(nullable: false, identity: true),
                        DoctorID = c.Int(nullable: false),
                        InsurancePlanID = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DoctorInsuranceID)
                .ForeignKey("dbo.Doctor", t => t.DoctorID, cascadeDelete: true)
                .Index(t => t.DoctorID);
            
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
                "dbo.DrugManufacturer",
                c => new
                    {
                        DrugManufacturerId = c.Int(nullable: false, identity: true),
                        Manufacturer = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DrugManufacturerId);
            
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
                "dbo.OpeningHour",
                c => new
                    {
                        OpeningHourId = c.Int(nullable: false, identity: true),
                        WeekDay = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        DoctorId = c.Int(),
                        OrganisationId = c.Int(),
                    })
                .PrimaryKey(t => t.OpeningHourId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .ForeignKey("dbo.Organisation", t => t.OrganisationId)
                .Index(t => t.DoctorId)
                .Index(t => t.OrganisationId);
            
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
            
            AddColumn("dbo.Address", "CityStateZipCodeID", c => c.Int(nullable: false));
            AddColumn("dbo.AmenityOption", "IsOption", c => c.Boolean(nullable: false));
            AddColumn("dbo.AmenityOption", "IsActive", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Address", "CityStateZipCodeID");
            AddForeignKey("dbo.Address", "CityStateZipCodeID", "dbo.CityStateZip", "CityStateZipCodeID", cascadeDelete: true);
            DropTable("dbo.City");
            DropTable("dbo.State");
            DropTable("dbo.Country");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.CityId);
            
            DropForeignKey("dbo.OrganizationAmenityOption", "OrganizationID", "dbo.Organisation");
            DropForeignKey("dbo.OrganizationAmenityOption", "AmenityOptionID", "dbo.AmenityOption");
            DropForeignKey("dbo.OpeningHour", "OrganisationId", "dbo.Organisation");
            DropForeignKey("dbo.OpeningHour", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Experience", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorLanguage", "LanguageId", "dbo.Language");
            DropForeignKey("dbo.DoctorLanguage", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorInsurance", "DoctorID", "dbo.Doctor");
            DropForeignKey("dbo.DoctorBoardCertification", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.BoardCertification", "SpecialityId", "dbo.Speciality");
            DropForeignKey("dbo.DoctorBoardCertification", "BoardCertificationId", "dbo.BoardCertification");
            DropForeignKey("dbo.Address", "CityStateZipCodeID", "dbo.CityStateZip");
            DropIndex("dbo.OrganizationAmenityOption", new[] { "AmenityOptionID" });
            DropIndex("dbo.OrganizationAmenityOption", new[] { "OrganizationID" });
            DropIndex("dbo.OpeningHour", new[] { "OrganisationId" });
            DropIndex("dbo.OpeningHour", new[] { "DoctorId" });
            DropIndex("dbo.Experience", new[] { "DoctorId" });
            DropIndex("dbo.DoctorLanguage", new[] { "DoctorId" });
            DropIndex("dbo.DoctorLanguage", new[] { "LanguageId" });
            DropIndex("dbo.DoctorInsurance", new[] { "DoctorID" });
            DropIndex("dbo.BoardCertification", new[] { "SpecialityId" });
            DropIndex("dbo.DoctorBoardCertification", new[] { "DoctorId" });
            DropIndex("dbo.DoctorBoardCertification", new[] { "BoardCertificationId" });
            DropIndex("dbo.Address", new[] { "CityStateZipCodeID" });
            DropColumn("dbo.AmenityOption", "IsActive");
            DropColumn("dbo.AmenityOption", "IsOption");
            DropColumn("dbo.Address", "CityStateZipCodeID");
            DropTable("dbo.OrganizationAmenityOption");
            DropTable("dbo.OpeningHour");
            DropTable("dbo.Experience");
            DropTable("dbo.DrugManufacturer");
            DropTable("dbo.Language");
            DropTable("dbo.DoctorLanguage");
            DropTable("dbo.DoctorInsurance");
            DropTable("dbo.BoardCertification");
            DropTable("dbo.DoctorBoardCertification");
            DropTable("dbo.DocOrgTaxonomy");
            DropTable("dbo.CityStateZip");
            CreateIndex("dbo.City", "StateId");
            CreateIndex("dbo.Address", "CountryId");
            CreateIndex("dbo.Address", "StateId");
            CreateIndex("dbo.Address", "CityId");
            AddForeignKey("dbo.Address", "CountryId", "dbo.Country", "CountryId");
            AddForeignKey("dbo.City", "StateId", "dbo.State", "StateId");
            AddForeignKey("dbo.Address", "StateId", "dbo.State", "StateId");
            AddForeignKey("dbo.Address", "CityId", "dbo.City", "CityId");
            RenameTable(name: "dbo.AmenityOption", newName: "Amenity");
        }
    }
}
