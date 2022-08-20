namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class openinhour : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.OpeningHour", "DoctorID", c => c.Int(nullable: false));
           // AddColumn("dbo.OpeningHour", "OrganizationID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OpeningHour", "OrganizationID");
            DropColumn("dbo.OpeningHour", "DoctorID");
        }
    }
}
