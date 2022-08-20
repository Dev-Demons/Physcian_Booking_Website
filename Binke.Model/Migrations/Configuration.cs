namespace Binke.Model.Migrations
{
    using DBContext;
    using SeedData;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<BinkeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BinkeDbContext context)
        {
            UserSeedData seedData = new UserSeedData(context);
            seedData.SeedAdminUser();
        }
    }
}
