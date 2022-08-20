namespace Binke.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSeniorCareTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Address", "SeniorCareId", c => c.Int());
            CreateIndex("dbo.Address", "SeniorCareId");
            AddForeignKey("dbo.Address", "SeniorCareId", "dbo.SeniorCare", "SeniorCareId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Address", "SeniorCareId", "dbo.SeniorCare");
            DropIndex("dbo.Address", new[] { "SeniorCareId" });
            DropColumn("dbo.Address", "SeniorCareId");
        }
    }
}
