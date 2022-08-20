namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactUs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactUs",
                c => new
                    {
                        ContactUsID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                        Subject = c.String(maxLength: 100, unicode: false),
                        Department = c.String(maxLength: 100, unicode: false),
                        Message = c.String(maxLength: 1000),
                        DateSubmit = c.DateTime(nullable: false, storeType: "date"),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(),
                        IsActive = c.Boolean(),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ContactUsID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ContactUs");
        }
    }
}
