namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnNullableInFacility : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Facility", "FacilityTypeId", "dbo.FacilityType");
            DropIndex("dbo.Facility", new[] { "FacilityTypeId" });
            AlterColumn("dbo.Facility", "FacilityTypeId", c => c.Byte());
            CreateIndex("dbo.Facility", "FacilityTypeId");
            AddForeignKey("dbo.Facility", "FacilityTypeId", "dbo.FacilityType", "FacilityTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Facility", "FacilityTypeId", "dbo.FacilityType");
            DropIndex("dbo.Facility", new[] { "FacilityTypeId" });
            AlterColumn("dbo.Facility", "FacilityTypeId", c => c.Byte(nullable: false));
            CreateIndex("dbo.Facility", "FacilityTypeId");
            AddForeignKey("dbo.Facility", "FacilityTypeId", "dbo.FacilityType", "FacilityTypeId", cascadeDelete: true);
        }
    }
}
