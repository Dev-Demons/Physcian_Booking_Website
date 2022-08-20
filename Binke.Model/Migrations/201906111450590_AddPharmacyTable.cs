namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPharmacyTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pharmacy",
                c => new
                    {
                        PharmacyId = c.Int(nullable: false, identity: true),
                        PharmacyName = c.String(maxLength: 50, unicode: false),
                        UserId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.PharmacyId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Address", "PharmacyId", c => c.Int());
            AddColumn("dbo.Review", "PharmacyId", c => c.Int());
            CreateIndex("dbo.Address", "PharmacyId");
            CreateIndex("dbo.Review", "PharmacyId");
            AddForeignKey("dbo.Address", "PharmacyId", "dbo.Pharmacy", "PharmacyId");
            AddForeignKey("dbo.Review", "PharmacyId", "dbo.Pharmacy", "PharmacyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Review", "PharmacyId", "dbo.Pharmacy");
            DropForeignKey("dbo.Pharmacy", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Address", "PharmacyId", "dbo.Pharmacy");
            DropIndex("dbo.Review", new[] { "PharmacyId" });
            DropIndex("dbo.Pharmacy", new[] { "UserId" });
            DropIndex("dbo.Address", new[] { "PharmacyId" });
            DropColumn("dbo.Review", "PharmacyId");
            DropColumn("dbo.Address", "PharmacyId");
            DropTable("dbo.Pharmacy");
        }
    }
}
