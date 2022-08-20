namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class multipleagegroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
              "dbo.DoctorAgeGroup",
              c => new
              {
                  DoctorAgeGroupId = c.Int(nullable: false, identity: true),
                  AgeGroupId = c.Int(nullable: false),
                  DoctorId = c.Int(nullable: false),
                  CreatedDate = c.DateTime(nullable: false),
                  UpdatedDate = c.DateTime(),
                  IsActive = c.Boolean(nullable: false),
                  IsDeleted = c.Boolean(nullable: false),
                  CreatedBy = c.Int(nullable: false),
                  ModifiedBy = c.Int(),
              })
              .PrimaryKey(t => t.DoctorAgeGroupId)
              .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
              .ForeignKey("dbo.AgeGroup", t => t.AgeGroupId, cascadeDelete: true)
              .Index(t => t.AgeGroupId)
              .Index(t => t.DoctorId);
        }
        
        public override void Down()
        {
        }
    }
}
