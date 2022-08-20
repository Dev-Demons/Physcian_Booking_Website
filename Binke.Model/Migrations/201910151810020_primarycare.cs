namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class primarycare : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctor", "IsPrimaryCare", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctor", "IsPrimaryCare");
        }
    }
}
