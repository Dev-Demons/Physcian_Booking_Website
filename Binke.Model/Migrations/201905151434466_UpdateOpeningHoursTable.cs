namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOpeningHoursTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OpeningHour", "SlotId", "dbo.Slot");
            DropIndex("dbo.OpeningHour", new[] { "SlotId" });
            AddColumn("dbo.OpeningHour", "DayWishSlot", c => c.String());
            DropColumn("dbo.OpeningHour", "SlotId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OpeningHour", "SlotId", c => c.Int(nullable: false));
            DropColumn("dbo.OpeningHour", "DayWishSlot");
            CreateIndex("dbo.OpeningHour", "SlotId");
            AddForeignKey("dbo.OpeningHour", "SlotId", "dbo.Slot", "SlotId", cascadeDelete: true);
        }
    }
}
