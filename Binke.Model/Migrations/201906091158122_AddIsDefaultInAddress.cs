namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsDefaultInAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Address", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Address", "IsDefault");
        }
    }
}
