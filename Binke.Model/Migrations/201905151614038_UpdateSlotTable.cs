namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSlotTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Slot", "SlotTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Slot", "SlotTime", c => c.Time(nullable: false, precision: 7));
        }
    }
}
