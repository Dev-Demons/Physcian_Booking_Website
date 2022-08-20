namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctorImageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorImage",
                c => new
                    {
                        DoctorImageId = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        ImagePath = c.String(maxLength: 100, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DoctorImageId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorImage", "DoctorId", "dbo.Doctor");
            DropIndex("dbo.DoctorImage", new[] { "DoctorId" });
            DropTable("dbo.DoctorImage");
        }
    }
}
