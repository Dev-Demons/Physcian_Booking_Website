namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSlotBookingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SlotBooking",
                c => new
                    {
                        SlotBookingId = c.Int(nullable: false, identity: true),
                        SlotId = c.Int(nullable: false),
                        PatientId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.SlotBookingId)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Slot", t => t.SlotId, cascadeDelete: true)
                .Index(t => t.SlotId)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SlotBooking", "SlotId", "dbo.Slot");
            DropForeignKey("dbo.SlotBooking", "PatientId", "dbo.Patient");
            DropIndex("dbo.SlotBooking", new[] { "PatientId" });
            DropIndex("dbo.SlotBooking", new[] { "SlotId" });
            DropTable("dbo.SlotBooking");
        }
    }
}
