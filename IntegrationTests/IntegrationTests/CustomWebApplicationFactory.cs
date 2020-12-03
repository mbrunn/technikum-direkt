using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> 
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.UseConfiguration(config);
            
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TechnikumDirektContext>));

                services.Remove(descriptor);

                services.AddDbContext<ITechnikumDirektContext, TechnikumDirektContext>(options =>
                {
                    options.UseSqlServer(config.GetConnectionString("TechnikumDirektTestDatabase"),
                        x =>
                        {
                            x.UseNetTopologySuite();
                            x.MigrationsAssembly("TechnikumDirekt.DataAccess.Sql");
                        });
                });
            });
        }
    }
}