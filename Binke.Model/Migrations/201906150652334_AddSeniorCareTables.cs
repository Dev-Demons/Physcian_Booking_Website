namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeniorCareTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SeniorCare",
                c => new
                    {
                        SeniorCareId = c.Int(nullable: false, identity: true),
                        SeniorCareName = c.String(maxLength: 50, unicode: false),
                        Summary = c.String(),
                        Description = c.String(),
                        Amenities = c.String(),
                        UserId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.SeniorCareId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SeniorCareImage",
                c => new
                    {
                        SeniorCareImageId = c.Int(nullable: false, identity: true),
                        SeniorCareId = c.Int(nullable: false),
                        ImagePath = c.String(maxLength: 100, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.SeniorCareImageId)
                .ForeignKey("dbo.SeniorCare", t => t.SeniorCareId, cascadeDelete: true)
                .Index(t => t.SeniorCareId);
            
            AddColumn("dbo.Review", "SeniorCareId", c => c.Int());
            AddColumn("dbo.OpeningHour", "SeniorCareId", c => c.Int());
            AddColumn("dbo.SocialMedia", "SeniorCareId", c => c.Int());
            CreateIndex("dbo.Review", "SeniorCareId");
            CreateIndex("dbo.OpeningHour", "SeniorCareId");
            CreateIndex("dbo.SocialMedia", "SeniorCareId");
            AddForeignKey("dbo.OpeningHour", "SeniorCareId", "dbo.SeniorCare", "SeniorCareId");
            AddForeignKey("dbo.Review", "SeniorCareId", "dbo.SeniorCare", "SeniorCareId");
            AddForeignKey("dbo.SocialMedia", "SeniorCareId", "dbo.SeniorCare", "SeniorCareId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SocialMedia", "SeniorCareId", "dbo.SeniorCare");
            DropForeignKey("dbo.SeniorCareImage", "SeniorCareId", "dbo.SeniorCare");
            DropForeignKey("dbo.Review", "SeniorCareId", "dbo.SeniorCare");
            DropForeignKey("dbo.SeniorCare", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OpeningHour", "SeniorCareId", "dbo.SeniorCare");
            DropIndex("dbo.SocialMedia", new[] { "SeniorCareId" });
            DropIndex("dbo.SeniorCareImage", new[] { "SeniorCareId" });
            DropIndex("dbo.OpeningHour", new[] { "SeniorCareId" });
            DropIndex("dbo.SeniorCare", new[] { "UserId" });
            DropIndex("dbo.Review", new[] { "SeniorCareId" });
            DropColumn("dbo.SocialMedia", "SeniorCareId");
            DropColumn("dbo.OpeningHour", "SeniorCareId");
            DropColumn("dbo.Review", "SeniorCareId");
            DropTable("dbo.SeniorCareImage");
            DropTable("dbo.SeniorCare");
        }
    }
}
