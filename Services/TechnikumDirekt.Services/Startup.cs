using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TechnikumDirekt.BusinessLogic;
using TechnikumDirekt.BusinessLogic.FluentValidation;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql;
using TechnikumDirekt.ServiceAgents;
using TechnikumDirekt.ServiceAgents.Interfaces;
using TechnikumDirekt.Services.Controllers;

namespace TechnikumDirekt.Services
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private readonly IWebHostEnvironment _hostingEnv;

        private IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _hostingEnv = env;
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddMvc(options =>
                {
                    options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();
                    options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddXmlSerializerFormatters();

            services.AddAutoMapper(typeof(Startup));

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("1", new OpenApiInfo
                    {
                        Version = "1",
                        Title = "Parcel Logistics Service",
                        Description = "Parcel Logistics Service (ASP.NET Core 3.1)",
                        Contact = new OpenApiContact
                        {
                            Name = "SKS",
                            Url = new Uri("http://www.technikum-wien.at/"),
                            Email = ""
                        }
                    });
                    c.CustomSchemaIds(type => type.FullName);
                });

            services.AddTransient<IHopRepository, HopRepository>();
            services.AddTransient<IWarehouseRepository, WarehouseRepository>();
            services.AddTransient<IParcelRepository, ParcelRepository>();

            services.AddTransient<IWarehouseLogic, WarehouseLogic>();
            services.AddTransient<ITrackingLogic, TrackingLogic>();
            
            services.AddTransient<IGeoEncodingAgent, OsmGeoEncodingAgent>();
            services.AddTransient<ILogisticsPartnerAgent, TransferParcelToPartnerAgent>();

            services.AddDbContext<ITechnikumDirektContext, TechnikumDirektContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TechnikumDirektDatabase"),
                    x =>
                    {
                        x.UseNetTopologySuite();
                        x.MigrationsAssembly("TechnikumDirekt.DataAccess.Sql");
                    }));

            //other validators are also added with this command.
            services.AddControllers().AddFluentValidation(config =>
                config.RegisterValidatorsFromAssemblyContaining<WarehouseValidator>());

            services.AddLogging();

            services.AddHttpClient("osm", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetSection("ApiUrls").GetValue<string>("OsmApiUrl"));
                c.DefaultRequestHeaders.Add("User-Agent", "TechnikumDirektApi");
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRouting();

            //TODO: Uncomment this if you need wwwroot folder
            // app.UseStaticFiles();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //TODO: Either use the SwaggerGen generated Swagger contract (generated from C# classes)
                c.SwaggerEndpoint("/swagger/1/swagger.json", "Parcel Logistics Service");

                //TODO: Or alternatively use the original Swagger contract that's included in the static files
                // c.SwaggerEndpoint("/swagger-original.json", "Parcel Logistics Service Original");
            });

            //TODO: Use Https Redirection
            // app.UseHttpsRedirection();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO: Enable production exception handling (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }
        }
    }
}