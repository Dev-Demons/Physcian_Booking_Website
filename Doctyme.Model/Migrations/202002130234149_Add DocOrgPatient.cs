namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocOrgPatient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocOrgPatient",
                c => new
                    {
                        DocOrgPatientID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        ReferenceID = c.Int(nullable: false),
                        IsActive = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        CreateBy = c.Int(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DocOrgPatientID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DocOrgPatient");
        }
    }
}
