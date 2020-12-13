using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql;
using TechnikumDirekt.Services;

namespace TechnikumDirekt.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> 
        : WebApplicationFactory<TStartup> where TStartup: class
    {

        private readonly IConfigurationRoot _configurationRoot;
        
        public CustomWebApplicationFactory()
        {
            _configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            
            ClientOptions.AllowAutoRedirect = true;
            var uriString = _configurationRoot.GetSection("TestingUrls").GetValue<string>("TestingEnvUrl");
            if (uriString != string.Empty)
            {
                ClientOptions.BaseAddress = new Uri(uriString);
            }
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseConfiguration(_configurationRoot);
            
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TechnikumDirektContext>));

                services.Remove(descriptor);
                
                services.AddDbContext<ITechnikumDirektContext, TechnikumDirektContext>(options =>
                {
                    options.UseSqlServer(_configurationRoot.GetConnectionString("TechnikumDirektTestDatabase"),
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