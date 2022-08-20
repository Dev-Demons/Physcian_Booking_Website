namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePatientTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Patient", "NPI");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Patient", "NPI", c => c.Int(nullable: false));
        }
    }
}
