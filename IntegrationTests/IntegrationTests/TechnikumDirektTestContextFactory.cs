using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TechnikumDirekt.DataAccess.Sql;

namespace IntegrationTests
{
    public class TechnikumDirektTestContextFactory : IDesignTimeDbContextFactory<TechnikumDirektContext>
    {
        public TechnikumDirektContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            
            var optionsBuilder = new DbContextOptionsBuilder<TechnikumDirektContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("TechnikumDirektTestDatabase"),
                x =>
                {
                    x.UseNetTopologySuite();
                    x.MigrationsAssembly("TechnikumDirekt.DataAccess.Sql");
                });

            return new TechnikumDirektContext(optionsBuilder.Options);
        }
    }

}