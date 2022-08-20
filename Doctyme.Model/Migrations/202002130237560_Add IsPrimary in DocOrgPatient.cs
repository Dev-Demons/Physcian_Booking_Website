namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsPrimaryinDocOrgPatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocOrgPatient", "IsPrimary", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DocOrgPatient", "IsPrimary");
        }
    }
}
