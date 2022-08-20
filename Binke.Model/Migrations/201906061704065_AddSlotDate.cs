namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSlotDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slot", "SlotDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Slot", "SlotDate");
        }
    }
}
