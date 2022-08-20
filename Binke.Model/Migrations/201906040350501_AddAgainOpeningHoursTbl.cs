namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgainOpeningHoursTbl : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.OpeningHour", "WeekDay", c => c.Int(nullable: false));
            //AddColumn("dbo.OpeningHour", "StartTime", c => c.DateTime(nullable: false));
            //AddColumn("dbo.OpeningHour", "EndTime", c => c.DateTime(nullable: false));
            //DropColumn("dbo.OpeningHour", "DayWishSlot");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.OpeningHour", "DayWishSlot", c => c.String());
            //DropColumn("dbo.OpeningHour", "EndTime");
            //DropColumn("dbo.OpeningHour", "StartTime");
            //DropColumn("dbo.OpeningHour", "WeekDay");
        }
    }
}
