using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.IntegrationTests
{
    [TestFixture]
    public class WarehouseManagementApiTests : IntegrationTests
    {
        private string _datasetLight;

        private const string ValidHopCode = "WENA03";
        private const string NotfoundHopCode = "XXXX01";
        
        [OneTimeSetUp]
        public void Setup()
        {
            _datasetLight = Utilities.LoadDatasetLight();
        }

        #region /warehouse post tests

        [Test]
        public async Task PostWarehouses_AddsNewWarehouseStructure()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            
            // Act
            var response = await Client.PostAsync("/warehouse", content);
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        #endregion

        #region /warehouse get tests

        [Test]
        public async Task GetWarehouses_ImportedDatabase_ReturnsStructure()
        {
            // Arrange
            var postContent = new StringContent(_datasetLight, Encoding.UTF8, "application/json");

            //Act
            await Client.PostAsync("/warehouse", postContent);
            var response = await Client.GetAsync("/warehouse");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseString = await response.Content.ReadAsStringAsync();
            var warehouse = JsonConvert.DeserializeObject<Warehouse>(responseString);
            
            Assert.NotNull(warehouse);
            Assert.NotNull(warehouse.NextHops);
            Assert.Greater(warehouse.NextHops.Count, 0);
        }
        
        #endregion

        #region /warehouse/{code} get tests

        [Test]
        public async Task GetWarehouseWithCode_validHopCode_OkValidHop()
        {
            // Arrange
            var postContent = new StringContent(_datasetLight, Encoding.UTF8, "application/json");

            //Act
            await Client.PostAsync("/warehouse", postContent);
            var response = await Client.GetAsync($"/warehouse/{ValidHopCode}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseString = await response.Content.ReadAsStringAsync();
            var hop = JsonConvert.DeserializeObject<Hop>(responseString);
            
            Assert.NotNull(hop);
            Assert.AreEqual(ValidHopCode, hop.Code);
        }
        
        [Test]
        public async Task GetWarehouseWithCode_invalidHopCode_NotFound()
        {
            // Arrange
            var postContent = new StringContent(_datasetLight, Encoding.UTF8, "application/json");

            //Act
            await Client.PostAsync("/warehouse", postContent);
            var response = await Client.GetAsync($"/warehouse/{NotfoundHopCode}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        #endregion
    }
}