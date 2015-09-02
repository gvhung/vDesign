using System.Data.Entity.Migrations;

namespace Data.EF
{
    internal sealed class EFContextConfiguration : DbMigrationsConfiguration<CompositeContext>
    {
        public EFContextConfiguration()
        {
            AutomaticMigrationsEnabled = true;

            #if (DEBUG)
            AutomaticMigrationDataLossAllowed = true;
            #elif (RELEASE)
            AutomaticMigrationDataLossAllowed = false;
            #endif  
        }
    }
}
