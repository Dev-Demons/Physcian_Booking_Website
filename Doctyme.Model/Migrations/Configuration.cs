namespace Doctyme.Model.Migrations
{
    using Doctyme.Model.SeedData;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DoctymeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DoctymeDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //context.Addresses.AddOrUpdate(
          
            //);
            //

            UserSeedData seedData = new UserSeedData(context);
            seedData.SeedAdminUser();
        }
    }
}
