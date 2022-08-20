namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFacilityOptionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FacilityOption",
                c => new
                    {
                        FacilityOptionId = c.Byte(nullable: false, identity: true),
                        FacilityOptionName = c.String(maxLength: 50, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FacilityOptionId);
            
            AddColumn("dbo.FacilityType", "FacilityOptionId", c => c.Byte());
            CreateIndex("dbo.FacilityType", "FacilityOptionId");
            AddForeignKey("dbo.FacilityType", "FacilityOptionId", "dbo.FacilityOption", "FacilityOptionId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FacilityType", "FacilityOptionId", "dbo.FacilityOption");
            DropIndex("dbo.FacilityType", new[] { "FacilityOptionId" });
            DropColumn("dbo.FacilityType", "FacilityOptionId");
            DropTable("dbo.FacilityOption");
        }
    }
}
