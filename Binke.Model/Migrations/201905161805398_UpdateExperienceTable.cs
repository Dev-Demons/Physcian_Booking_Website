namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateExperienceTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Experience", "CityId", c => c.Int());
            AddColumn("dbo.Experience", "StateId", c => c.Int());
            CreateIndex("dbo.Experience", "CityId");
            CreateIndex("dbo.Experience", "StateId");
            AddForeignKey("dbo.Experience", "CityId", "dbo.City", "CityId");
            AddForeignKey("dbo.Experience", "StateId", "dbo.State", "StateId");
            DropColumn("dbo.Experience", "City");
            DropColumn("dbo.Experience", "State");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Experience", "State", c => c.String(maxLength: 30, unicode: false));
            AddColumn("dbo.Experience", "City", c => c.String(maxLength: 20, unicode: false));
            DropForeignKey("dbo.Experience", "StateId", "dbo.State");
            DropForeignKey("dbo.Experience", "CityId", "dbo.City");
            DropIndex("dbo.Experience", new[] { "StateId" });
            DropIndex("dbo.Experience", new[] { "CityId" });
            DropColumn("dbo.Experience", "StateId");
            DropColumn("dbo.Experience", "CityId");
        }
    }
}
