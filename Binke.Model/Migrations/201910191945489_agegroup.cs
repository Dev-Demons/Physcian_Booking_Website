namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agegroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AgeGroup",
                c => new
                {
                    AgeGroupId = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 150, unicode: false),
                    CreatedDate = c.DateTime(nullable: false),
                    UpdatedDate = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                    ModifiedBy = c.Int(),
                })
                .PrimaryKey(t => t.AgeGroupId);
        }
        
        public override void Down()
        {
           
        }
    }
}
