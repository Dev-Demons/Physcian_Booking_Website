namespace Doctyme.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingLanguageCodeColumnInLanguage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Language", "LanguageCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Language", "LanguageCode");
        }
    }
}
