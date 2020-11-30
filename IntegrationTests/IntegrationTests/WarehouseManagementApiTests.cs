using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql;
using TechnikumDirekt.Services.Models;
using Hop = TechnikumDirekt.DataAccess.Models.Hop;
using HopArrival = TechnikumDirekt.DataAccess.Models.HopArrival;
using Transferwarehouse = TechnikumDirekt.Services.Models.Transferwarehouse;
using Warehouse = TechnikumDirekt.DataAccess.Models.Warehouse;

namespace IntegrationTests
{
    [TestFixture]
    public class WarehouseManagementApiTests : IntegrationTests
    { 
        [OneTimeSetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task PostWarehouses_AddsNewWarehouseStructure()
        {
            
        }
        
        [Test]
        public async Task GetWarehouseWithCode_validHopCode_OkValidHop()
        {
            
        }
        
        [Test]
        public async Task GetWarehouseWithCode_invalidHopCode_NotFound()
        {
            
        }
        
        [Test]
        public async Task GetWarehouses_EmptyDatabase_NotFound()
        {
            // Arrange
            /*_testingDb.Database.ExecuteSqlRaw(
                $"DELETE FROM {_testingDb.Model.FindEntityType(typeof(Hop)).GetTableName()}");*/

            //Act
            var response = await Client.GetAsync("/warehouse");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseString = await response.Content.ReadAsStringAsync();
            var objectResult = JsonConvert.DeserializeObject<ObjectResult>(responseString);
            var error = JsonConvert.DeserializeObject<Error>(objectResult.Value.ToString());

            Assert.AreEqual("No hierarchy loaded yet.", error.ErrorMessage);
        }

        [Test]
        public async Task GetWarehouses_WithLoadedHierarchy_Ok()
        {
            // Arrange
            var client = Factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ITechnikumDirektContext>();
                        var logger = scopedServices
                            .GetRequiredService<ILogger<WarehouseManagementApiTests>>();

                        try
                        {
                            Utilities.InitializeDbForTests(db);
                            
                            db.Hops.Add(new Warehouse()
                            {
                                Code = "123Ab",
                                Description = "Test Warehouse",
                                HopArrivals = new List<HopArrival>(),
                                HopType = HopType.Warehouse,
                                Level = 0,
                                LocationCoordinates = new Point(42.0, 42.0),
                                LocationName = "Root Warehouse",
                                NextHops = new List<Hop>(),
                                ParentTraveltimeMins = null,
                                ParentWarehouse = null,
                                ParentWarehouseCode = null
                            });
                            db.SaveChanges();
                        }
                        
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An error occurred seeding the " +
                                                "database with test messages. Error: {Message}", ex.Message);
                        }
                    }
                });
            }).CreateClient();
           
            //Act
            var response = await client.GetAsync("/warehouse");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseString = response.Content.ReadAsStringAsync().Result;
            var objectResult = JsonConvert.DeserializeObject<ObjectResult>(responseString);
        }
    }
}