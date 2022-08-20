namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFacilityTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorFacilityAffiliation", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DoctorFacilityAffiliation", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.DoctorFacilityAffiliation", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DoctorFacilityAffiliation", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.DoctorFacilityAffiliation", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.DoctorFacilityAffiliation", "ModifiedBy", c => c.Int());
            AddColumn("dbo.Facility", "FacilityName", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facility", "FacilityName");
            DropColumn("dbo.DoctorFacilityAffiliation", "ModifiedBy");
            DropColumn("dbo.DoctorFacilityAffiliation", "CreatedBy");
            DropColumn("dbo.DoctorFacilityAffiliation", "IsDeleted");
            DropColumn("dbo.DoctorFacilityAffiliation", "IsActive");
            DropColumn("dbo.DoctorFacilityAffiliation", "UpdatedDate");
            DropColumn("dbo.DoctorFacilityAffiliation", "CreatedDate");
        }
    }
}
