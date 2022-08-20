namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddrugtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Drug",
                c => new
                    {
                        DrugId = c.Int(nullable: false, identity: true),
                        DrugName = c.String(),
                        Description = c.String(),
                        UnitoryPrice = c.Single(nullable: false),
                        SellingPrice = c.Single(nullable: false),
                        ManufactureName = c.String(),
                        ExpiryDate = c.String(),
                        PharmacyId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DrugId)
                .ForeignKey("dbo.Pharmacy", t => t.PharmacyId, cascadeDelete: true)
                .Index(t => t.PharmacyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Drug", "PharmacyId", "dbo.Pharmacy");
            DropIndex("dbo.Drug", new[] { "PharmacyId" });
            DropTable("dbo.Drug");
        }
    }
}
