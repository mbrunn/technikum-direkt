using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.IntegrationTests
{
    [TestFixture]
    public class SenderApiTests : IntegrationTests, IDisposable
    { 
        private string _datasetLight;
        
        private readonly Recipient _recipient1 = new Recipient
        {
            Name = "Michael Brunner",
            Street = "Dresdner Straße 38-44",
            PostalCode = "1200",
            City = "Wien",
            Country = "AT"
        };

        private readonly Recipient _recipient2 = new Recipient
        {
            Name = "Benjamin Ableidinger",
            Street = "Floridsdorfer Hauptstraße 43",
            PostalCode = "1210",
            City = "Wien",
            Country = "AT"
        };

        private Parcel _validParcel;
        private Parcel _invalidParcel;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _datasetLight = Utilities.LoadDatasetLight();
            
            _validParcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2
            };
            
            _invalidParcel = new Parcel
            {
                Weight = -2.0f,
                Sender = _recipient1,
                Recipient = _recipient2
            };
        }

        #region /parcel post tests

        [Test]
        public async Task PostSubmitParcel_validParcel_OkValidTrackingId()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("/warehouse", content);
            var response = await Client.PostAsync("/parcel", parcelContent);
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            
            var responseString = await response.Content.ReadAsStringAsync();
            var parcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(responseString);
            
            Assert.NotNull(parcelInfo);
            Assert.IsNotEmpty(parcelInfo.TrackingId);
        }
        
        [Test]
        public async Task PostSubmitParcel_invalidParcel_BadRequest()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_invalidParcel), Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("/warehouse", content);
            var response = await Client.PostAsync("/parcel", parcelContent);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        #endregion
    }
}