namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateReviewTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Review", name: "varchar", newName: "Title");
            AlterColumn("dbo.Review", "Title", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.Drug", "ExpiryDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Drug", "ExpiryDate", c => c.String());
            AlterColumn("dbo.Review", "Title", c => c.String(maxLength: 50));
            RenameColumn(table: "dbo.Review", name: "Title", newName: "varchar");
        }
    }
}
