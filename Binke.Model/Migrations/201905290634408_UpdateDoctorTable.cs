namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDoctorTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctor", "IsAllowNewPatient", c => c.Boolean(nullable: false));
            AddColumn("dbo.Doctor", "IsNtPcp", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctor", "IsNtPcp");
            DropColumn("dbo.Doctor", "IsAllowNewPatient");
        }
    }
}
