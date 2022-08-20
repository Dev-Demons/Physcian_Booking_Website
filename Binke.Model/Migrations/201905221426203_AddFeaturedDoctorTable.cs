namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFeaturedDoctorTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeaturedDoctor",
                c => new
                    {
                        FeaturedDoctorId = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        ProfilePicture = c.String(maxLength: 50, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FeaturedDoctorId)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeaturedDoctor", "DoctorId", "dbo.Doctor");
            DropIndex("dbo.FeaturedDoctor", new[] { "DoctorId" });
            DropTable("dbo.FeaturedDoctor");
        }
    }
}
