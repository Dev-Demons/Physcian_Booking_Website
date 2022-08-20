namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctorSpeciality : DbMigration
    {
        public override void Up()
        {
            CreateTable(
               "dbo.DoctorInsurance",
               c => new
               {
                   DoctorInsuranceId = c.Int(nullable: false, identity: true),
                   InsuranceId = c.Int(nullable: false),
                   DoctorId = c.Int(nullable: false),
                   CreatedDate = c.DateTime(nullable: false),
                   UpdatedDate = c.DateTime(),
                   IsActive = c.Boolean(nullable: false),
                   IsDeleted = c.Boolean(nullable: false),
                   CreatedBy = c.Int(nullable: false),
                   ModifiedBy = c.Int(),
               })
               .PrimaryKey(t => t.DoctorInsuranceId)
               .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
               .ForeignKey("dbo.InsuranceAccepted", t => t.InsuranceId, cascadeDelete: true)
               .Index(t => t.InsuranceId)
               .Index(t => t.DoctorId);

        }
        public override void Down()
        {
        }
    }
}
