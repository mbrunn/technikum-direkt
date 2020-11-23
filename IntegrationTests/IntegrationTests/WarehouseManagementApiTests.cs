using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.Services.Models;

namespace IntegrationTests
{
    [TestFixture]
    public class WarehouseManagementApiTests : IntegrationTests, IDisposable
    {
        private ITechnikumDirektContext _testingDb;
        
        [OneTimeSetUp]
        public void Setup()
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
            /*await _testingDb.Database.ExecuteSqlRawAsync(
                $"DELETE FROM {_testingDb.Model.FindEntityType(typeof(Hop)).GetTableName()}");*/
            
            //Act
            var response = await Client.GetAsync("/warehouse");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            Assert.AreEqual("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
            
            var responseString = response.Content.ReadAsStringAsync().Result;
            var objectResult = JsonConvert.DeserializeObject<ObjectResult>(responseString);
            var error = JsonConvert.DeserializeObject<Error>(objectResult.Value.ToString());

            Assert.AreEqual("No hierarchy loaded yet.", error.ErrorMessage);
        }
    }
}