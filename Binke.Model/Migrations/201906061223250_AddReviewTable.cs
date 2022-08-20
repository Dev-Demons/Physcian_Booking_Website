namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Review",
                c => new
                    {
                        ReviewId = c.Long(nullable: false, identity: true),
                        varchar = c.String(maxLength: 50),
                        ReviewText = c.String(),
                        Rating = c.Int(nullable: false),
                        DoctorId = c.Int(),
                        FacilityId = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .ForeignKey("dbo.Facility", t => t.FacilityId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.FacilityId)
                .Index(t => t.CreatedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Review", "CreatedBy", "dbo.AspNetUsers");
            DropForeignKey("dbo.Review", "FacilityId", "dbo.Facility");
            DropForeignKey("dbo.Review", "DoctorId", "dbo.Doctor");
            DropIndex("dbo.Review", new[] { "CreatedBy" });
            DropIndex("dbo.Review", new[] { "FacilityId" });
            DropIndex("dbo.Review", new[] { "DoctorId" });
            DropTable("dbo.Review");
        }
    }
}
