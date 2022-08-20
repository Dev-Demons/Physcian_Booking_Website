namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFeaturedSpecialityTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeaturedSpeciality",
                c => new
                    {
                        FeaturedSpecialityId = c.Int(nullable: false, identity: true),
                        SpecialityId = c.Short(nullable: false),
                        Description = c.String(maxLength: 150, unicode: false),
                        ProfilePicture = c.String(maxLength: 50, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FeaturedSpecialityId)
                .ForeignKey("dbo.Speciality", t => t.SpecialityId, cascadeDelete: true)
                .Index(t => t.SpecialityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeaturedSpeciality", "SpecialityId", "dbo.Speciality");
            DropIndex("dbo.FeaturedSpeciality", new[] { "SpecialityId" });
            DropTable("dbo.FeaturedSpeciality");
        }
    }
}
