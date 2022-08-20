namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBaseModelInPatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patient", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Patient", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.Patient", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Patient", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Patient", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.Patient", "ModifiedBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patient", "ModifiedBy");
            DropColumn("dbo.Patient", "CreatedBy");
            DropColumn("dbo.Patient", "IsDeleted");
            DropColumn("dbo.Patient", "IsActive");
            DropColumn("dbo.Patient", "UpdatedDate");
            DropColumn("dbo.Patient", "CreatedDate");
        }
    }
}
