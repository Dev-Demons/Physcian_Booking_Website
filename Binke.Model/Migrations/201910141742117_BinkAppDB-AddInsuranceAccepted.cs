namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BinkAppDBAddInsuranceAccepted : DbMigration
    {
        public override void Up()
        {

            CreateTable(
              "dbo.InsuranceAccepted",
              c => new
              {
                  InsuranceAcceptedId = c.Int(nullable: false, identity: true),
                  Name = c.String(maxLength: 150, unicode: false),
                  CreatedDate = c.DateTime(nullable: false),
                  UpdatedDate = c.DateTime(),
                  IsEnable =  c.Boolean(nullable: false, defaultValue: true),
                  IsActive = c.Boolean(nullable: false),
                  IsDeleted = c.Boolean(nullable: false),
                  CreatedBy = c.Int(nullable: false),
                  ModifiedBy = c.Int(),
              })
              .PrimaryKey(t => t.InsuranceAcceptedId);
            

        }
        
        public override void Down()
        {
            AddColumn("dbo.InsuranceAccepted", "DoctorId", c => c.Int(nullable: false));
            CreateIndex("dbo.InsuranceAccepted", "DoctorId");
            AddForeignKey("dbo.InsuranceAccepted", "DoctorId", "dbo.Doctor", "DoctorId", cascadeDelete: true);
        }
    }
}
